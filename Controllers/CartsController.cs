#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Project.Controllers
{
    public class CartsController : Controller
    {
        private readonly ProjectContext _context;

        public CartsController(ProjectContext context)
        {
            _context = context;
        }


        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var projectContext = _context.Cart.Include(c => c.User);
            return View(await projectContext.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            //var cart = _context.Cart
            //.Include(c => c.User).Where(m => m.ID == id);

            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        

        #region AddProduct

        // GET: Carts/Edit/5
        [HttpPost]
        //Route[("Carts/AddProduct/{Cid}/{Pid}")]
        public async Task<IActionResult> AddProduct(int? id, int? ItemId)
        {
            if (id == null)
            {
                return View("Identity/Account/Login");
            }

            var cart = await _context.Cart.FindAsync(id);
            var prod = await _context.Product.FindAsync(ItemId);

            cart.Products.Add(prod);
            ViewData["UserID"] = new SelectList(_context.Set<User>(), "ID", "Email", cart.UserID);
            return View(cart);
        }

        [HttpGet]
        public async Task<IActionResult> AddProductToCart(int cartId, int productId)
        {
            // Check if cartId and productId are provided
            if (cartId == 0 || productId == 0)
            {
                return BadRequest("Cart ID and Product ID must be provided.");
            }

            // Attempt to fetch the cart from the database using cartId
            var cart = await _context.Cart
                .Include(c => c.Products) // Include the Products list to avoid lazy loading
                .FirstOrDefaultAsync(c => c.ID == cartId);

            // Check if the cart is found
            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            // Fetch the product from the database using productId
            var product = await _context.Product.FindAsync(productId);

            // Check if the product is found
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // Add the product to the cart
            cart.Products.Add(product);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the cart index or another appropriate action
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        //Route[("Carts/AddProduct/{Cid}/{Pid}")]
       
        public IActionResult AddToCart(int productId)
        {
            // Retrieve the cart from session
            var sessionCart = HttpContext.Session.GetString("cart");

            // Deserialize the cart from session or create a new one
            Cart cart;
            if (sessionCart != null)
            {
                cart = JsonConvert.DeserializeObject<Cart>(sessionCart);
            }
            else
            {
                cart = new Cart();
                cart.Products = new List<Product>();
            }

            // Retrieve the product from the database
            var product = _context.Product.Find(productId);
            if (product == null)
            {
                // Handle the case where the product doesn't exist
                return NotFound();
            }

            // Add the product to the cart
            cart.Products.Add(product);

            // Update the cart in session
            var updatedCartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("cart", updatedCartJson);

            // Redirect to the cart view or another appropriate action
            return RedirectToAction("Show");
        }


        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int cartId)
        {
            // Retrieve the cart from the database
            var cart = await _context.Cart.FindAsync(cartId);
            if (cart == null)
            {
                return NotFound(); // Handle the case where the cart is not found
            }

            // Retrieve the product from the database
            var product = await _context.Product.FindAsync(productId);
            if (product == null)
            {
                return NotFound(); // Handle the case where the product is not found
            }

            // Add the product to the cart
            cart.Products.Add(product);

            // Update the database
            _context.Update(cart);
            await _context.SaveChangesAsync();

            // Redirect to the cart page or show a confirmation message
            return RedirectToAction("Show", "Carts");
        }


        public IActionResult Show()
        {
            // Retrieve the cart from session
            var sessionCart = HttpContext.Session.GetString("cart");

            // Deserialize the cart from session or create a new one
            Cart cart;
            if (sessionCart != null)
            {
                cart = JsonConvert.DeserializeObject<Cart>(sessionCart);
            }
            else
            {
                cart = new Cart();
                cart.Products = new List<Product>();
            }

            // Pass the cart to the view
            return View(cart);
        }


        public async Task<IActionResult> Payment()

        {
            var cart = (Cart)JsonConvert.DeserializeObject<Cart>(HttpContext.Session.GetString("cart"));
            return View(cart);

        }

        [HttpGet]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            // Retrieve the cart from session
            var sessionCart = HttpContext.Session.GetString("cart");
            if (sessionCart == null)
            {
                // Handle the case where no cart is found in session
                return NotFound("Cart not found.");
            }

            // Deserialize the cart from session
            Cart cart = JsonConvert.DeserializeObject<Cart>(sessionCart);

            // Find the product in the database
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                // Handle the case where the product does not exist in the database
                return NotFound("Product not found.");
            }

            // Find the product in the cart's product list
            var productInCart = cart.Products.FirstOrDefault(p => p.ID == id);

            if (productInCart == null)
            {
                // Handle the case where the product is not found in the cart's product list
                return NotFound("Product not found in cart.");
            }

            // Remove the product from the cart's product list
            cart.Products.Remove(productInCart);

            // Update the cart in the session
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cart));

            // Redirect to the cart view
            return RedirectToAction("Show");
        }


        #endregion





        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Set<User>(), "ID", "Email");

            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TotalPrice,Status,UserID")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                cart.Products = new List<Product>();
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Set<User>(), "ID", "Email", cart.UserID);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Set<User>(), "ID", "Email", cart.UserID);
            return View(cart);
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TotalPrice,Status,UserID")] Cart cart)
        {
            if (id != cart.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Cart.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Set<User>(), "ID", "Email", cart.UserID);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Cart.FindAsync(id);
            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.ID == id);
        }


        

    }


}

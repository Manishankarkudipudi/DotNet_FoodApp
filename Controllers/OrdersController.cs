#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project.Data;
using Project.Models;

namespace Project.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ProjectContext _context;

        public OrdersController(ProjectContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Order.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> FinalizeOrder(Order order)
        {
            var cart = (Cart)JsonConvert.DeserializeObject<Cart>(HttpContext.Session.GetString("cart"));
            order.Products = new List<Product>();
            foreach (Product item in cart.Products)
            {
                var product = _context.Product.Find(item.ID);
                order.Products.Add(product);
            }
            order.Status = DeliveryStatus.Pending;
            order.TotalPrice = cart.TotalPrice;

            if (ModelState.IsValid)
            {
                _context.Order.Add(order);
                await _context.SaveChangesAsync();
                // Redirect to FinalizeOrder view with order object as model
                return RedirectToAction("FinalizeOrder", new { orderId = order.ID });
            }
            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TotalPrice,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Order.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TotalPrice,Status")] Order order)
        {
            if (id != order.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Order.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.ID))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Checkout
        public IActionResult Checkout()
        {
            // Retrieve the cart from the session
            var cartJson = HttpContext.Session.GetString("cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                // If the cart is not found in the session, redirect to the cart page
                return RedirectToAction("Show", "Carts");
            }

            // Deserialize the cart from JSON
            var cart = JsonConvert.DeserializeObject<Cart>(cartJson);

            // Create a new Order object and populate its properties based on the cart
            var order = new Order
            {
                TotalPrice = cart.TotalPrice,
                Products = new List<Product>(),
                // Initialize other Order properties if needed
            };

            foreach (var item in cart.Products)
            {
                var product = _context.Product.Find(item.ID);
                if (product != null)
                {
                    order.Products.Add(product);
                }
            }

            // Return the Checkout view with the order as the model
            return View("Checkout", order);
        }

        


        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.ID == id);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Project.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly ProjectContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ProjectContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin
        // Lists all products
      

        public async Task<IActionResult> ListProducts()
        {
            // Fetch all products from the database
            var products = await _context.Product.ToListAsync();
            // Return the view with the list of products
            return View(products);
        }


        // GET: Admin/Create
        // Renders the product creation form
        public IActionResult Create()
        {
            return View();
        }

      


        // GET: Admin/Edit/5
        // Renders the product edit form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the product with the given ID
            var model = await _context.Product.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


        



        // GET: Admin/Delete/5
        // Renders the product delete confirmation view
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the product with the given ID
            var model = await _context.Product.FirstOrDefaultAsync(m => m.ID == id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListProducts));
        }






        // Utility method to check if a product exists
        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ID == id);
        }

        // Method to check if a user is in the "Admin" role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Description, Price, Category, Photo")] Product model)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload for Photo if provided
                if (model.Photo != null)
                {
                    // Define the folder path where you want to save the image
                    string uploadFolder = Path.Combine("wwwroot", "template", "images");

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // Generate a unique file name for the uploaded image
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Photo.FileName);

                    // Full path where the image will be saved
                    string filePath = Path.Combine(uploadFolder, fileName);

                    // Save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Photo.CopyToAsync(fileStream);
                    }

                    // Save the file path to the model.PhotoSrc property
                    // Here, only save the filename (e.g., image.jpg)
                    model.PhotoSrc = fileName;
                }

                // Add the new product to the context
                _context.Product.Add(model);
                // Save changes to the database
                await _context.SaveChangesAsync();

                // Set a success message in TempData
                TempData["SuccessMessage"] = "Product created successfully.";

                // Redirect to the ListProducts action
              //  return RedirectToAction(nameof(ListProducts));
            }

            // If the model is not valid, return to the view with the model
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edited(int id, [Bind("ID, Name, Description, Price, Category, PhotoSrc, Photo")] Product model)
        {
            if (id != model.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Product.FindAsync(id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Handle file upload if a new photo is provided
                    if (model.Photo != null)
                    {
                        // Define the folder path where you want to save the image
                        string uploadFolder = Path.Combine("wwwroot", "template", "images");

                        // Ensure the directory exists
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        // Generate a unique filename for the uploaded image
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Photo.FileName);

                        // Full path where the image will be saved
                        string filePath = Path.Combine(uploadFolder, fileName);

                        // Save the file to the server
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Photo.CopyToAsync(fileStream);
                        }

                        // Update the PhotoSrc property only if a new photo is uploaded
                        existingProduct.PhotoSrc = fileName;
                    }

                    // Update other product properties
                    existingProduct.Name = model.Name;
                    existingProduct.Description = model.Description;
                    existingProduct.Price = model.Price;
                    existingProduct.Category = model.Category;

                    // Update the product in the database
                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(model.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirect to the ListProducts action to display the updated product list
                return RedirectToAction(nameof(ListProducts));
            }

            // If model state is invalid, return to the Edit view with the model
            return View(model);
        }




    }
}

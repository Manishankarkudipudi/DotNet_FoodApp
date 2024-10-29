using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Project.Data;
using Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Project.Controllers
{
    public class MenuController : Controller
    {
        private readonly ProjectContext _context;

        public MenuController(ProjectContext context)
        {
            _context = context;
        }

        // Action method to display data from the database
        public async Task<IActionResult> Index()
        {
            // Fetch data from the database (e.g., list of products)
            var products = await _context.Product.ToListAsync();

            // Return the view with the data
            return View(products);
        }
    }
}

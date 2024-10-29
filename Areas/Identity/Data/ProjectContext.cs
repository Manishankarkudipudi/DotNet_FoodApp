using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data;

public class ProjectContext : IdentityDbContext<IdentityUser>
{
    public ProjectContext(DbContextOptions<ProjectContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Seed data for the Product table
        builder.Entity<Product>().HasData(
            new Product
            {
                ID = 1,
                Name = "Cheeseburger",
                Description = "A juicy cheeseburger with lettuce, tomato, and cheese",
                Price = (int)8.00m, // Explicitly cast the decimal value to int
                Category = "Food",
                PhotoSrc = "f1.png"
            },
            new Product
            {
                ID = 2,
                Name = "Pizza Margherita",
                Description = "Classic pizza with tomato, mozzarella, and basil",
                Price = (int)12.00m, // Explicitly cast the decimal value to int
                Category = "Food",
                PhotoSrc = "f2.png"
            },
            new Product
            {
                ID = 3,
                Name = "Caesar Salad",
                Description = "Fresh romaine lettuce, croutons, and Caesar dressing",
                Price = (int)7.00m, // Explicitly cast the decimal value to int
                Category = "Food",
                PhotoSrc = "f3.png"
            },
            new Product
            {
                ID = 4,
                Name = "Pasta Carbonara",
                Description = "Creamy pasta with bacon, eggs, and cheese",
                Price = (int)10.00m, // Explicitly cast the decimal value to int
                Category = "Food",
                PhotoSrc = "f4.png"
            },
            new Product
            {
                ID = 5,
                Name = "Grilled Chicken",
                Description = "Grilled chicken breast with herbs and spices",
                Price = (int)15.00m, // Explicitly cast the decimal value to int
                Category = "Food",
                PhotoSrc = "f5.png"
            }
        );
    }



    public DbSet<Project.Models.Product> Product { get; set; }

    public DbSet<Project.Models.Category> Category { get; set; }

    public DbSet<Project.Models.Order> Order { get; set; }

    public DbSet<Project.Models.Cart> Cart { get; set; }

    public DbSet<Project.Models.User> User { get; set; }
}

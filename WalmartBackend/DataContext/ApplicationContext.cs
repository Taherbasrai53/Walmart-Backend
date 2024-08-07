using Microsoft.EntityFrameworkCore;
using WalmartBackend.Models;

namespace WalmartBackend.DataContext
{
    public class ApplicationContext:DbContext
    {
        public DbSet<User> Users { get; set; }  
        public DbSet<Product> Products {  get; set; }   

    }
}

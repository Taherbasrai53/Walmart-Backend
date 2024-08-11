using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalmartBackend.Models
{
    public class Product
    {
        [Key]public int ProductId {  get; set; }    
        public string ProductName { get; set; }
        public string ProductImage {  get; set; }  
        public int ProductInventory {  get; set; }
        public float ProductPrice { get; set; }
    }

    public class AddProductModel
    {
        public string ProductName { get; set; }
        public IFormFile? ProductImage { get; set; }
        public string ProductInventory { get; set; }
        public string ProductPrice { get; set; }
    }
}

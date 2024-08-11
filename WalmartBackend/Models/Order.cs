using System.ComponentModel.DataAnnotations;

namespace WalmartBackend.Models
{
    public class Order
    {
        [Key] public int OrderId { get; set; }   
        public int UserId { get; set; }
        public bool IsOpen {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CheckoutAt { get; set; }
    }
}

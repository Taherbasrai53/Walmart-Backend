namespace WalmartBackend.Models
{
    public class OrderItems
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId {  get; set; }
        public int Quantity { get; set; }
    }

    public class OrderItemsResponse
    {
        public int Id { get; set; } 
        public int OrderId { get; set; }        
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductImage {  get; set; }   
        public string ProductName { get; set; }
        public float ProductPrice { get; set; } 
    }
}

namespace WalmartBackend.Models
{
    public class Trolly
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
    }

    public class TrollyResponse
    {
        public int TrollyId { get; set; }
        public int StoreId { get; set; }
        public string StoreName {  get; set; }  
        public int OrderId {  get; set; }   
    }
}

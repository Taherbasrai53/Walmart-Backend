namespace WalmartBackend.Models
{
    public class UserTrollies
    {
        public int Id { get; set; }
        public int TrollyId { get; set; }
        public int UserId { get; set; }
        public DateTime SatrtedAt { get; set; }
        = DateTime.UtcNow;
        public DateTime EndedAt { get; set; }
        public bool IsActive {  get; set; }
    }
}

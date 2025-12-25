namespace _202Project.models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? FName { get; set; } = string.Empty;
        public string? LName { get; set; } = string.Empty;
        public int? Phone { get; set; }

    }
}
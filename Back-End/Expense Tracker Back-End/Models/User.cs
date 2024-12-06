namespace Expense_Tracker_Back_End.Models
{
    public class User
    {
        public int Id { get; set; } // Unique identifier for the user
        public string? ImagePath { get; set; } // Path to the user's profile image
        public string Name { get; set; } // Full name of the user
        public string Email { get; set; } // Email address
        public string Password { get; set; } // Password for authentication
        public DateTime BirthDate { get; set; } // Date of birth

        // Navigation properties
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}

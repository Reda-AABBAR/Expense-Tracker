namespace Expense_Tracker_Back_End.Models
{
    public class Notification
    {
        public int Id { get; set; } // Unique identifier for the notification
        public string Message { get; set; } // Notification message
        public DateTime CreatedAt { get; set; } // Timestamp for the notification

        // Navigation property
        public int UserId { get; set; } // Foreign key to User
        public User User { get; set; }
    }
}

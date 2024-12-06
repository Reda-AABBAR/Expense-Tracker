namespace Expense_Tracker_Back_End.Models
{
    public class Budget
    {
        public int Id { get; set; } // Unique identifier for the budget
        public string Name { get; set; } // Name of the budget
        public decimal BudgetAmount { get; set; } // Total budget amount
        public DateTime StartDate { get; set; } // Start date of the budget
        public DateTime EndDate { get; set; } // End date of the budget

        // Navigation properties
        public int UserId { get; set; } // Foreign key to User
        public User User { get; set; }
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}

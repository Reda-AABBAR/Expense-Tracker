namespace Expense_Tracker_Back_End.Models
{
    public class Expense
    {
        public int Id { get; set; } // Unique identifier for the expense
        public decimal Amount { get; set; } // Amount of the expense
        public DateTime Date { get; set; } // Date of the expense
        public string Description { get; set; } // Description of the expense

        // Navigation properties
        public int BudgetId { get; set; } // Foreign key to Budget
        public Budget Budget { get; set; }

        public int ExpenseTypeId { get; set; } // Foreign key to ExpenseType
        public ExpenseType Type { get; set; }
    }
}

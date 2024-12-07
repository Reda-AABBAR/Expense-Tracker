namespace Expense_Tracker_Back_End.dtos
{
    public class ExpenseRequest
    {
        public decimal mount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public ExpenseType type { get; set; }
    }
}

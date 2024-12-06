namespace Expense_Tracker_Back_End.dtos
{
    public class ExpenseRequest
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int ExpenseTypeId { get; set; }
    }
}

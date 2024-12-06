namespace Expense_Tracker_Back_End.dtos
{
    public class BudgetRequest
    {
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}


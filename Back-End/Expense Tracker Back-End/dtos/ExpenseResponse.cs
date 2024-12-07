namespace Expense_Tracker_Back_End.dtos
{
    public class ExpenseResponse
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal mount { get; set; }
        public DateTime Date { get; set; }
        public ExpenseType Type { get; set; }
    }
    public class ExpenseType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}

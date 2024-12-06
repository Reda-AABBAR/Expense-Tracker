using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker_Back_End.Data;
using Expense_Tracker_Back_End.dtos;
using Expense_Tracker_Back_End.Models;

namespace Expense_Tracker_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpenseController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{budgetId}")]
        public IActionResult CreateExpense(int budgetId, [FromBody] ExpenseRequest expenseRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
            var budget = _context.Budgets.Include(b => b.Expenses).FirstOrDefault(b => b.Id == budgetId);
            if (budget == null)
                return NotFound($"Budget with ID {budgetId} not found.");

            
            var totalExpenses = budget.Expenses.Sum(e => e.Amount);

            
            if (totalExpenses + expenseRequest.Amount > budget.BudgetAmount)
            {
                return BadRequest("Expense exceeds the available budget.");
            }

            
            var expense = new Expense
            {
                Amount = expenseRequest.Amount,
                Date = expenseRequest.Date,
                Description = expenseRequest.Description,
                BudgetId = budgetId,
                ExpenseTypeId = expenseRequest.ExpenseTypeId
            };

            // Add the expense to the database
            _context.Expenses.Add(expense);
            _context.SaveChanges();

            // Check if the total expenses exceed 80% of the budget and create a notification
            if (totalExpenses + expenseRequest.Amount >= 0.8m * budget.BudgetAmount)
            {
                var notification = new Notification
                {
                    Message = "You have reached 80% of your budget.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = budget.UserId
                };
                _context.Notifications.Add(notification);
                _context.SaveChanges();
            }
            return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, expense);
        }

        [HttpGet("{id}")]
        public IActionResult GetExpenseById(int id)
        {
            var expense = _context.Expenses
                .Include(e => e.Budget)
                .Include(e => e.Type)
                .FirstOrDefault(e => e.Id == id);

            if (expense == null)
                return NotFound($"Expense with ID {id} not found.");

            return Ok(expense);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateExpense(int id, [FromBody] ExpenseRequest expenseRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var expense = _context.Expenses.Include(e => e.Budget).FirstOrDefault(e => e.Id == id);
            if (expense == null)
                return NotFound($"Expense with ID {id} not found.");

            // Calculate the current total expenses before update
            var totalExpensesBefore = expense.Budget.Expenses.Sum(e => e.Amount);

            // Update the expense details
            expense.Amount = expenseRequest.Amount;
            expense.Date = expenseRequest.Date;
            expense.Description = expenseRequest.Description;
            expense.ExpenseTypeId = expenseRequest.ExpenseTypeId;

            // Save the changes
            _context.Expenses.Update(expense);
            _context.SaveChanges();

            // Recalculate the total expenses after the update
            var totalExpensesAfter = expense.Budget.Expenses.Sum(e => e.Amount);

            // Check if the updated total expenses exceed the budget
            if (totalExpensesAfter > expense.Budget.BudgetAmount)
            {
                return BadRequest("Updated expense exceeds the available budget.");
            }

            // Check if the updated total expenses reach 80% of the budget and create a notification
            if (totalExpensesAfter >= 0.8m * expense.Budget.BudgetAmount && totalExpensesBefore < 0.8m * expense.Budget.BudgetAmount)
            {
                var notification = new Notification
                {
                    Message = "You have reached 80% of your budget after the update.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = expense.Budget.UserId // Assuming the UserId is available in the Budget entity
                };
                _context.Notifications.Add(notification);
                _context.SaveChanges();
            }

            return Ok(expense);
        }
    }
}

using Expense_Tracker_Back_End.Data;
using Expense_Tracker_Back_End.dtos;
using Expense_Tracker_Back_End.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BudgetController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{userID}")]
        public IActionResult CreateBudget([FromRoute] int userID, [FromBody] BudgetRequest budgetRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if the user exists
            var user = _context.Users.Include(u => u.Budgets).FirstOrDefault(u => u.Id == userID);
            if (user == null)
                return NotFound($"User with ID {userID} not found.");

            // Create a new Budget entity from the BudgetRequest
            var budget = new Budget
            {
                Name = budgetRequest.Name,
                BudgetAmount = budgetRequest.Budget,
                StartDate = budgetRequest.StartDate,
                EndDate = budgetRequest.EndDate,
                UserId = userID,
                User = user
            };

            // Save the budget
            _context.Budgets.Add(budget);

            // Add the budget to the user's collection
            user.Budgets.Add(budget);

            // Persist changes to the database
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetBudgetById), new { id = budget.Id }, new
            {
                id = budget.Id,
                name = budget.Name,
                budget = budget.BudgetAmount,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                expenses = budget.Expenses,
            });
        }

        // Get a budget by ID
        [HttpGet("{id}")]
        public IActionResult GetBudgetById(int id)
        {
            var budget = _context.Budgets.Include(b => b.User).FirstOrDefault(b => b.Id == id);
            if (budget == null)
                return NotFound($"Budget with ID {id} not found.");

            return Ok(new
            {
                id = budget.Id,
                name = budget.Name,
                budget = budget.BudgetAmount,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                expenses = budget.Expenses,
            });
        }


        [HttpGet("user/{userId}")]
        public IActionResult GetAllBudgetsByUserId(int userId)
        {
            var user = _context.Users.Include(u => u.Budgets).FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound($"User with ID {userId} not found.");

            var budgets = user.Budgets.Select(b => new
            {
                id = b.Id,
                name = b.Name,
                budget = b.BudgetAmount,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                expenses = b.Expenses
            }).ToList();

            return Ok(budgets);
        }
    }
}
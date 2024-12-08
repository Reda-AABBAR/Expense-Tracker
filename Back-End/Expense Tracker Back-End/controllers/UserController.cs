using Expense_Tracker_Back_End.Data;
using Expense_Tracker_Back_End.dtos;
using Expense_Tracker_Back_End.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Expense_Tracker_Back_End.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.BirthDate = updatedUser.BirthDate;
            user.Password = updatedUser.Password;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully." });
        }

        [HttpPut("updateImage/{id}")]
        public async Task<IActionResult> UpdateUserImage(int id, [FromForm] string imagePath)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            user.ImagePath = imagePath;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpGet("dashboard/{email}")]
        public async Task<IActionResult> GetUserDashboardData([FromRoute] string email)
        {
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return NotFound(new { message = "User not found." });


            var budgets = await _context.Budgets
                .Where(b => b.UserId == user.Id)
                .Include(b => b.Expenses) 
                .ThenInclude(e => e.Type) 
                .ToListAsync();


            if (budgets.Count == 0)
            {
                return Ok(new
                {
                    nbr_budget = 0,
                    nbr_expenses = 0,
                    most_expensive_expense = (object)null,
                    chart = new { }
                });
            }

            
            var nbrBudget = budgets.Count;

            var expenses = budgets.SelectMany(b => b.Expenses).ToList();

            var nbrExpenses = expenses.Count;

            // Find the most expensive expense
            var mostExpensiveExpense = expenses
                .OrderByDescending(e => e.Amount)
                .FirstOrDefault();

            // Create the chart data (group expenses by type and count them)
            var chartData = (expenses.IsNullOrEmpty())? 
                null : 
                expenses
                .GroupBy(e => e.Type.Name)
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

            // Prepare the response
            var response = new
            {
                nbr_budget = nbrBudget,
                nbr_expenses = nbrExpenses,
                most_expensive_expense = new ExpenseResponse
                {
                    Id = mostExpensiveExpense.Id,
                    Description = mostExpensiveExpense.Description,
                    mount = mostExpensiveExpense.Amount,
                    Date = mostExpensiveExpense.Date,
                    Type = mostExpensiveExpense.Type == null ? null : new dtos.ExpenseType { Id = mostExpensiveExpense.Type.Id, Name = mostExpensiveExpense.Type.Name }
                },
                chart = chartData
            };

            return Ok(response);
        }



    }
}

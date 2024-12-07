using Microsoft.AspNetCore.Mvc;
using Expense_Tracker_Back_End.Data;
using Expense_Tracker_Back_End.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Expense_Tracker_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseTypeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpenseTypeController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public IActionResult GetAllExpenseTypes()
        {
            var expenseTypes = _context.ExpenseTypes.ToList();
            return Ok(expenseTypes);
        }

        [HttpPost]
        public IActionResult CreateExpenseType([FromBody] ExpenseType expenseType)
        {
            if (string.IsNullOrWhiteSpace(expenseType.Name))
                return BadRequest("Expense type name cannot be empty.");

            if (IsExistByName(expenseType.Name))
                return Conflict($"Expense type with name '{expenseType.Name}' already exists.");

            var newExpenseType = new ExpenseType
            {
                Name = expenseType.Name.Trim()
            };

            _context.ExpenseTypes.Add(newExpenseType);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetByName), new { name = newExpenseType.Name }, newExpenseType);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateExpenseType(int id, [FromBody] ExpenseType expenseType)
        {
            if (string.IsNullOrWhiteSpace(expenseType.Name))
                return BadRequest("Expense type name cannot be empty.");

            var existingExpenseType = _context.ExpenseTypes.FirstOrDefault(e => e.Id == id);
            if (existingExpenseType == null)
                return NotFound($"Expense type with ID {id} not found.");

            if (IsExistByName(expenseType.Name) && !existingExpenseType.Name.Equals(expenseType.Name, StringComparison.OrdinalIgnoreCase))
                return Conflict($"Expense type with name '{expenseType.Name}' already exists.");

            existingExpenseType.Name = expenseType.Name.Trim();
            _context.ExpenseTypes.Update(existingExpenseType);
            _context.SaveChanges();

            return Ok(existingExpenseType);
        }

        [HttpGet("name/{name}")]
        public IActionResult GetByName(string name)
        {
            var expenseType = _context.ExpenseTypes
                .FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (expenseType == null)
                return NotFound($"Expense type with name '{name}' not found.");

            return Ok(expenseType);
        }

        [HttpGet("exists/{name}")]
        public IActionResult IsExistByNameApi(string name)
        {
            var exists = IsExistByName(name);
            return Ok(new { Exists = exists });
        }

        private bool IsExistByName(string name)
        {
            return _context.ExpenseTypes.Any(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}

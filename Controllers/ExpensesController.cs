using CoinKeeper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CoinKeeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration? _config;

        public ExpensesController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpense()
        {
            var expenses = await _context.Expenses
                .Include(u => u.ExpenseCat)
                .Select(u => new Expense
                {
                    Id = u.Id,
                    Cost = u.Cost,
                    DateCost = u.DateCost,
                    Comment = u.Comment,
                    IdExpenseCat= u.IdExpenseCat,
                    ExpenseCat = u.ExpenseCat
                })
                .ToListAsync();

            return expenses;

        }

        [HttpGet("report")]
        public async Task<IActionResult> GetManifestsByDateRange(int idExpenseCat, DateTime startDate, DateTime endDate)
        {

            try
            {
                using (var dbContext = _context)
                {
                    if (idExpenseCat != 0)
                    {
                        var query = @"
                                    SELECT *  
                                    FROM Expenses 
                                    WHERE IdExpenseCat=@IdExpenseCat AND DateCost >= @StartDate AND DateCost <= @EndDate";

                        var expenses = await dbContext.Expenses.FromSqlRaw(query,
                            new SqlParameter("@IdExpenseCat", idExpenseCat),
                            new SqlParameter("@StartDate", startDate),
                            new SqlParameter("@EndDate", endDate))
                            .ToListAsync();

                        foreach (var expense in expenses)
                        {
                            var existingExpense = await dbContext.ExpenseCats.FindAsync(expense.IdExpenseCat);
                            if (existingExpense != null)
                            {
                                expense.ExpenseCat = existingExpense;
                            }
                        }



                        return Ok(expenses);
                    }
                    else
                    {
                        var query = @"
                                    SELECT *  
                                    FROM Expenses 
                                    WHERE DateCost >= @StartDate AND DateCost <= @EndDate";

                        var expenses = await dbContext.Expenses.FromSqlRaw(query,
                            new SqlParameter("@StartDate", startDate),
                            new SqlParameter("@EndDate", endDate))
                            .ToListAsync();

                        foreach (var expense in expenses)
                        {
                            var existingExpense = await dbContext.ExpenseCats.FindAsync(expense.IdExpenseCat);
                            if (existingExpense != null)
                            {
                                expense.ExpenseCat = existingExpense;
                            }
                        }



                        return Ok(expenses);
                    }

                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateExpense([FromBody] Expense expense)
        {
            try
            {
                _context.Expenses.Add(expense);
                _context.SaveChanges();

                return Ok("Gasto agregado con éxito!");
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }
    }
}

using CoinKeeper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoinKeeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseCategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration? _config;

        public ExpenseCategoryController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseCat>>> GetExpenseCat()
        {

            return await _context.ExpenseCats.ToListAsync();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<User>>> PostExpenseCat(ExpenseCat expense)
        {
            _context.ExpenseCats.Add(expense);
            _context.SaveChanges();

            var expenses_ = await _context.ExpenseCats
                .Where(m => m.Id == expense.Id)
                .ToListAsync();
            if (expenses_ == null)
            {
                return NotFound();
            }

            return Ok(expenses_);

        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteExpenseCat(int id)
        {
            try
            {
                var expensecatToDelete = _context.ExpenseCats.Find(id);

                if (expensecatToDelete == null)
                {
                    return NotFound("Consignee not found");
                }

                _context.ExpenseCats.Remove(expensecatToDelete);
                _context.SaveChanges();

                return Ok("Se ha eliminadoel Gasto con éxito");
            }
            catch (Exception ex)
            {
                return BadRequest("Error: No se pudo eliminar el registro. " + ex.Message);
            }
        }

    }
}

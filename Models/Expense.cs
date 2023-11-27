namespace CoinKeeper.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public DateTime DateCost { get; set; }
        public decimal Cost { get; set; }
        public string? Comment { get; set; }

        public int IdExpenseCat { get; set; }   // Foreign key for ExpenseCats

        public ExpenseCat? ExpenseCat { get; set; }    // Navigation property for ExpenseCats

    }
}

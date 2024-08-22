using Domain.Models;

namespace Domain.Interfaces;

public interface ITransactionService
{
    Task AddTransactionAsync(int id, DateTime transactionDate, decimal amount);
    Task<Transaction> GetTransactionAsync(int id);
}

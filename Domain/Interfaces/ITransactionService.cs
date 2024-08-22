using Domain.Models;

namespace Domain.Interfaces;

public interface ITransactionService
{
    void AddTransaction(int id, DateTime transactionDate, decimal amount);
    Transaction GetTransaction(int id);
}

using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;

    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AddTransaction(int id, DateTime transactionDate, decimal amount)
    {
        if (_context.Transactions.Any(t => t.Id == id))
        {
            throw new InvalidOperationException($"Транзакция с Id {id} уже существует.");
        }

        var transaction = new Transaction
        {
            Id = id,
            TransactionDate = transactionDate,
            Amount = amount
        };

        try
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Ошибка при сохранении транзакции в базу данных.", ex);
        }
    }

    public Transaction GetTransaction(int id)
    {
        var transaction = _context.Transactions.Find(id);
        if (transaction == null)
        {
            throw new KeyNotFoundException($"Транзакция с Id {id} не найдена.");
        }
        return transaction;
    }
}

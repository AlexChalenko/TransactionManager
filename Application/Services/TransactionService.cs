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

    public async Task AddTransactionAsync(int id, DateTime transactionDate, decimal amount)
    {
        if (await _context.Transactions.AnyAsync(t => t.Id == id))
        {
            throw new InvalidOperationException($"Транзакция с Id {id} уже существует.");
        }

        var transaction = new Transaction
        {
            Id = id,
            TransactionDate = transactionDate,
            Amount = amount
        };

        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<Transaction> GetTransactionAsync(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null)
        {
            throw new KeyNotFoundException($"Транзакция с Id {id} не найдена.");
        }
        return transaction;
    }
}

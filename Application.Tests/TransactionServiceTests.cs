using Application.Services;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests
{
    public class TransactionServiceTests
    {
        [Fact]
        public async Task AddTransactionAsync_ShouldAddTransaction_WhenTransactionIsValid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AddTransaction_ShouldAddTransaction")
                .Options;

            using var context = new ApplicationDbContext(options);
            var service = new TransactionService(context);

            // Act
            await service.AddTransactionAsync(1, new DateTime(2023, 8, 22), 100.23m);

            // Assert
            var transaction = await context.Transactions.FindAsync(1);
            Assert.NotNull(transaction);
            Assert.Equal(1, transaction.Id);
            Assert.Equal(100.23m, transaction.Amount);
        }

        [Fact]
        public async Task AddTransactionAsync_ShouldThrowInvalidOperationException_WhenTransactionAlreadyExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AddTransaction_ShouldThrowInvalidOperationException")
                .Options;

            using var context = new ApplicationDbContext(options);
            context.Transactions.Add(new Transaction { Id = 1, TransactionDate = DateTime.Now, Amount = 100 });
            await context.SaveChangesAsync();

            var service = new TransactionService(context);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddTransactionAsync(1, DateTime.Now, 200));
            Assert.Equal("Транзакция с Id 1 уже существует.", exception.Message);
        }

        [Fact]
        public async Task GetTransactionAsync_ShouldReturnTransaction_WhenTransactionExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GetTransaction_ShouldReturnTransaction")
                .Options;

            using var context = new ApplicationDbContext(options);
            var transaction = new Transaction { Id = 1, TransactionDate = DateTime.Now, Amount = 100 };
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            var service = new TransactionService(context);

            // Act
            var result = await service.GetTransactionAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetTransactionAsync_ShouldThrowKeyNotFoundException_WhenTransactionDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GetTransaction_ShouldThrowKeyNotFoundException")
                .Options;

            using var context = new ApplicationDbContext(options);
            var service = new TransactionService(context);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetTransactionAsync(99));
            Assert.Equal("Транзакция с Id 99 не найдена.", exception.Message);
        }
    }
}

using Application.DTO;
using Application.Interfaces;
using Application.Services;
using Application.Validators;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Persistence;
using System.Globalization;

namespace TransactionManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=transactions.db"))
                .AddScoped<ITransactionService, TransactionService>()
                .AddTransient<IValidationHandler<TransactionRequest>, IdValidationHandler>()
                .AddTransient<IValidationHandler<TransactionRequest>, DateValidationHandler>()
                .AddTransient<IValidationHandler<TransactionRequest>, AmountValidationHandler>()
                .AddScoped<ValidationPipeline<TransactionRequest>>()
                .BuildServiceProvider();

            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var transactionService = serviceProvider.GetRequiredService<ITransactionService>();
            var validationPipeline = serviceProvider.GetRequiredService<ValidationPipeline<TransactionRequest>>();

            while (true)
            {
                Console.WriteLine("Введите команду (add, get, exit):");
                var command = Console.ReadLine().ToLower();

                switch (command)
                {
                    case "add":
                        await AddTransactionAsync(transactionService, validationPipeline);
                        break;
                    case "get":
                        await GetTransactionAsync(transactionService);
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Неизвестная команда. Попробуйте снова.");
                        break;
                }
            }
        }

        private static async Task AddTransactionAsync(ITransactionService transactionService, ValidationPipeline<TransactionRequest> validationPipeline)
        {
            try
            {
                Console.Write("Введите Id: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                {
                    Console.WriteLine("Ошибка: некорректный формат Id. Id должен быть положительным числом.");
                    return;
                }

                Console.Write("Введите дату: ");
                if (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    Console.WriteLine("Ошибка: некорректный формат даты.");
                    return;
                }

                Console.Write("Введите сумму: ");
                if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
                {
                    Console.WriteLine("Ошибка: некорректный формат суммы. Сумма должна быть положительным числом.");
                    return;
                }

                var request = new TransactionRequest
                {
                    Id = id,
                    TransactionDate = date,
                    Amount = amount
                };

                validationPipeline.Validate(request);

                await transactionService.AddTransactionAsync(id, date, amount);
                Console.WriteLine("[OK]");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка валидации: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении транзакции: {ex.Message}");
            }
        }

        private static async Task GetTransactionAsync(ITransactionService transactionService)
        {
            try
            {
                Console.Write("Введите Id: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                {
                    Console.WriteLine("Ошибка: некорректный формат Id. Id должен быть положительным числом.");
                    return;
                }

                var transaction = await transactionService.GetTransactionAsync(id);
                string json = JsonConvert.SerializeObject(transaction, new JsonSerializerSettings
                {
                    Culture = CultureInfo.InvariantCulture
                });
                Console.WriteLine(json);
                Console.WriteLine("[OK]");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении транзакции: {ex.Message}");
            }
        }
    }
}

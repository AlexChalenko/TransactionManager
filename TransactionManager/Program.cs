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
        static void Main(string[] args)
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
            context.Database.Migrate();

            var transactionService = serviceProvider.GetRequiredService<ITransactionService>();
            var validationPipeline = serviceProvider.GetRequiredService<ValidationPipeline<TransactionRequest>>();

            while (true)
            {
                Console.WriteLine("Введите команду (add, get, exit):");
                var command = Console.ReadLine().ToLower();

                switch (command)
                {
                    case "add":
                        AddTransaction(transactionService, validationPipeline);
                        break;
                    case "get":
                        GetTransaction(transactionService);
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Неизвестная команда. Попробуйте снова.");
                        break;
                }
            }
        }


        private static void AddTransaction(ITransactionService transactionService, ValidationPipeline<TransactionRequest> validationPipeline)
        {
            try
            {
                // Получение и проверка ввода данных
                Console.Write("Введите Id: ");
                int id = int.Parse(Console.ReadLine());

                Console.Write("Введите дату (в формате дд.мм.гггг): ");
                DateTime date = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture);

                Console.Write("Введите сумму: ");
                decimal amount = decimal.Parse(Console.ReadLine());

                // Создание запроса для валидации
                var request = new TransactionRequest
                {
                    Id = id,
                    TransactionDate = date,
                    Amount = amount
                };

                // Валидация
                validationPipeline.Validate(request);

                // Если валидация успешна, добавляем транзакцию
                transactionService.AddTransaction(id, date, amount);
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



        private static void GetTransaction(ITransactionService transactionService)
        {
            try
            {
                Console.Write("Введите Id: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Ошибка: некорректный формат Id.");
                    return;
                }

                var transaction = transactionService.GetTransaction(id);
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

using Application.DTO;
using Application.Validators;
using Xunit;

public class AmountValidationHandlerTests
{
    [Fact]
    public void Handle_ShouldThrowException_WhenAmountIsNegative()
    {
        // Arrange
        var request = new TransactionRequest { Id = 1, TransactionDate = DateTime.Now, Amount = -100 };
        var handler = new AmountValidationHandler();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => handler.Handle(request));
        Assert.Equal("Сумма должна быть положительным числом.", exception.Message);
    }

    [Fact]
    public void Handle_ShouldNotThrowException_WhenAmountIsPositive()
    {
        // Arrange
        var request = new TransactionRequest { Id = 1, TransactionDate = DateTime.Now, Amount = 100 };
        var handler = new AmountValidationHandler();

        // Act & Assert
        var exception = Record.Exception(() => handler.Handle(request));
        Assert.Null(exception);
    }
}

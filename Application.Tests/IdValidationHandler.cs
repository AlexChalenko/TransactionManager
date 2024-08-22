using Application.DTO;
using Application.Validators;
using Xunit;

public class IdValidationHandlerTests
{
    [Fact]
    public void Handle_ShouldThrowException_WhenIdIsNegative()
    {
        // Arrange
        var request = new TransactionRequest { Id = -1, TransactionDate = DateTime.Now, Amount = 100 };
        var handler = new IdValidationHandler();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => handler.Handle(request));
        Assert.Equal("Id должен быть положительным числом.", exception.Message);
    }

    [Fact]
    public void Handle_ShouldNotThrowException_WhenIdIsPositive()
    {
        // Arrange
        var request = new TransactionRequest { Id = 1, TransactionDate = DateTime.Now, Amount = 100 };
        var handler = new IdValidationHandler();

        // Act & Assert
        var exception = Record.Exception(() => handler.Handle(request));
        Assert.Null(exception);
    }
}

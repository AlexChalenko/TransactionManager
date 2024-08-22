using Application.DTO;
using Application.Validators;
using Xunit;

public class DateValidationHandlerTests
{
    [Fact]
    public void Handle_ShouldThrowException_WhenDateIsDefault()
    {
        // Arrange
        var request = new TransactionRequest { Id = 1, TransactionDate = default, Amount = 100 };
        var handler = new DateValidationHandler();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => handler.Handle(request));
        Assert.Equal("Дата не может быть пустой или некорректной.", exception.Message);
    }

    [Fact]
    public void Handle_ShouldNotThrowException_WhenDateIsValid()
    {
        // Arrange
        var request = new TransactionRequest { Id = 1, TransactionDate = DateTime.Now, Amount = 100 };
        var handler = new DateValidationHandler();

        // Act & Assert
        var exception = Record.Exception(() => handler.Handle(request));
        Assert.Null(exception);
    }
}

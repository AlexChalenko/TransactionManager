using Application.DTO;
using Application.Interfaces;
using Application.Validators;
using Moq;
using Xunit;

public class ValidationPipelineTests
{
    [Fact]
    public void Validate_ShouldCallAllHandlers()
    {
        // Arrange
        var mockHandler1 = new Mock<IValidationHandler<TransactionRequest>>();
        var mockHandler2 = new Mock<IValidationHandler<TransactionRequest>>();
        var mockHandler3 = new Mock<IValidationHandler<TransactionRequest>>();

        var handlers = new List<IValidationHandler<TransactionRequest>> { mockHandler1.Object, mockHandler2.Object, mockHandler3.Object };
        var pipeline = new ValidationPipeline<TransactionRequest>(handlers);

        var request = new TransactionRequest { Id = 1, TransactionDate = DateTime.Now, Amount = 100 };

        // Act
        pipeline.Validate(request);

        // Assert
        mockHandler1.Verify(h => h.Handle(request), Times.Once);
        mockHandler2.Verify(h => h.Handle(request), Times.Once);
        mockHandler3.Verify(h => h.Handle(request), Times.Once);
    }

    [Fact]
    public void Validate_ShouldThrowException_WhenOneHandlerFails()
    {
        // Arrange
        var mockHandler1 = new Mock<IValidationHandler<TransactionRequest>>();
        mockHandler1.Setup(h => h.Handle(It.IsAny<TransactionRequest>())).Throws(new ArgumentException("Test Exception"));

        var handlers = new List<IValidationHandler<TransactionRequest>> { mockHandler1.Object };
        var pipeline = new ValidationPipeline<TransactionRequest>(handlers);

        var request = new TransactionRequest { Id = 1, TransactionDate = DateTime.Now, Amount = 100 };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => pipeline.Validate(request));
        Assert.Equal("Test Exception", exception.Message);
    }
}

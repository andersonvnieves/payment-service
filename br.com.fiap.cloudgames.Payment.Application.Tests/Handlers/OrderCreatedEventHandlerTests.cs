using br.com.fiap.cloudgames.Payment.Application.Events;
using br.com.fiap.cloudgames.Payment.Application.Handlers;
using br.com.fiap.cloudgames.Payment.Application.Tests.TestData;
using br.com.fiap.cloudgames.Payment.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Payment.Domain.Aggregates;
using br.com.fiap.cloudgames.Payment.Domain.Exceptions;
using br.com.fiap.cloudgames.Payment.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace br.com.fiap.cloudgames.Payment.Application.Tests.Handlers;

public class OrderCreatedEventHandlerTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly OrderCreatedEventHandler _sut;

    public OrderCreatedEventHandlerTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>(MockBehavior.Strict);
        _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
        _serviceScopeMock = new Mock<IServiceScope>(MockBehavior.Strict);
        _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>(MockBehavior.Strict);

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceScopeMock.Setup(x => x.Dispose());

        _serviceProviderMock.Setup(x => x.GetService(typeof(IPaymentRepository)))
            .Returns(_paymentRepositoryMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IUnitOfWork)))
            .Returns(_unitOfWorkMock.Object);

        _sut = new OrderCreatedEventHandler(_serviceScopeFactoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WhenValidEvent_ShouldCreatePaymentAddAndCommit()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var amount = 150.50m;
        var orderCreatedEvent = ApplicationTestData.ValidOrderCreatedEvent(orderId, userId, amount);

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _paymentRepositoryMock.Setup(x => x.AddAsync(It.Is<Domain.Aggregates.Payment>(p =>
            p.OrderId == orderId &&
            p.UserId == userId &&
            p.Amount.PriceValue == amount
        ))).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        await _sut.HandleAsync(orderCreatedEvent);

        // Assert
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Aggregates.Payment>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        _unitOfWorkMock.Verify(x => x.RollbackAsync(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryThrows_ShouldRollbackAndRethrow()
    {
        // Arrange
        var orderCreatedEvent = ApplicationTestData.ValidOrderCreatedEvent();
        var dbException = new InvalidOperationException("Database insertion error");

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _paymentRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Aggregates.Payment>()))
            .ThrowsAsync(dbException);
        _unitOfWorkMock.Setup(x => x.RollbackAsync()).Returns(Task.CompletedTask);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.HandleAsync(orderCreatedEvent));
        Assert.Same(dbException, ex);

        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Aggregates.Payment>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.RollbackAsync(), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenPriceIsNegative_ShouldRollbackAndThrowDomainException()
    {
        // Arrange
        var negativeAmount = -10.00m;
        var orderCreatedEvent = ApplicationTestData.ValidOrderCreatedEvent(totalAmount: negativeAmount);

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.RollbackAsync()).Returns(Task.CompletedTask);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() => _sut.HandleAsync(orderCreatedEvent));
        Assert.Contains("Price can't be negative.", ex.Errors);

        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Aggregates.Payment>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.RollbackAsync(), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}

using br.com.fiap.cloudgames.Payment.Application.Abstractions;
using br.com.fiap.cloudgames.Payment.Application.Events;
using br.com.fiap.cloudgames.Payment.Application.Publishers;
using br.com.fiap.cloudgames.Payment.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Payment.Application.UseCases.ApprovePayment;
using br.com.fiap.cloudgames.Payment.Domain.Aggregates;
using br.com.fiap.cloudgames.Payment.Domain.Enums;
using br.com.fiap.cloudgames.Payment.Domain.Repositories;
using br.com.fiap.cloudgames.Payment.Domain.ValueObjects;
using Moq;

namespace br.com.fiap.cloudgames.Payment.Application.Tests.UseCases;

public class ApprovePaymentUseCaseTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPaymentProcessedEventPublisher> _eventPublisherMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly ApprovePaymentUseCase _sut;

    public ApprovePaymentUseCaseTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>(MockBehavior.Strict);
        _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _eventPublisherMock = new Mock<IPaymentProcessedEventPublisher>(MockBehavior.Strict);
        _currentUserMock = new Mock<ICurrentUser>(MockBehavior.Strict);

        _sut = new ApprovePaymentUseCase(
            _paymentRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _eventPublisherMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPaymentExistsAndIsPending_ShouldApproveUpdateCommitAndPublish()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userName = "John Doe";
        var userEmail = "john@example.com";
        var amount = new Price(99.99m);

        var payment = Domain.Aggregates.Payment.Create(orderId, userId, amount);

        _currentUserMock.SetupGet(x => x.UserId).Returns(userId);
        _currentUserMock.SetupGet(x => x.Name).Returns(userName);
        _currentUserMock.SetupGet(x => x.Email).Returns(userEmail);

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _paymentRepositoryMock.Setup(x => x.GetByOrderIdAndUserIdAsync(orderId, userId))
            .ReturnsAsync(payment);
        _paymentRepositoryMock.Setup(x => x.UpdateAsync(payment)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
        _eventPublisherMock.Setup(x => x.PublishAsync(It.Is<PaymentProcessedEvent>(e =>
            e.UserId == userId &&
            e.OrderId == orderId &&
            e.PaymentStatus == PaymentStatus.Approved &&
            e.Name == userName &&
            e.Email == userEmail &&
            e.EventId != Guid.Empty
        ))).Returns(Task.CompletedTask);

        // Act
        await _sut.ExecuteAsync(orderId);

        // Assert
        Assert.Equal(PaymentStatus.Approved, payment.Status);
        
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _paymentRepositoryMock.Verify(x => x.GetByOrderIdAndUserIdAsync(orderId, userId), Times.Once);
        _paymentRepositoryMock.Verify(x => x.UpdateAsync(payment), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        _eventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<PaymentProcessedEvent>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPaymentDoesNotExist_ShouldThrowApplicationException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _currentUserMock.SetupGet(x => x.UserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _paymentRepositoryMock.Setup(x => x.GetByOrderIdAndUserIdAsync(orderId, userId))
            .ReturnsAsync((Domain.Aggregates.Payment)null!);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ApplicationException>(() => _sut.ExecuteAsync(orderId));
        Assert.Equal("Payment not found", ex.Message);

        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _paymentRepositoryMock.Verify(x => x.GetByOrderIdAndUserIdAsync(orderId, userId), Times.Once);
        _paymentRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Domain.Aggregates.Payment>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        _eventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<PaymentProcessedEvent>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrows_ShouldRollbackAndRethrow()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var amount = new Price(99.99m);
        var payment = Domain.Aggregates.Payment.Create(orderId, userId, amount);

        _currentUserMock.SetupGet(x => x.UserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _paymentRepositoryMock.Setup(x => x.GetByOrderIdAndUserIdAsync(orderId, userId))
            .ReturnsAsync(payment);
        
        var dbException = new InvalidOperationException("DB error");
        _paymentRepositoryMock.Setup(x => x.UpdateAsync(payment)).ThrowsAsync(dbException);
        _unitOfWorkMock.Setup(x => x.RollbackAsync()).Returns(Task.CompletedTask);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ExecuteAsync(orderId));
        Assert.Same(dbException, ex);

        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _paymentRepositoryMock.Verify(x => x.GetByOrderIdAndUserIdAsync(orderId, userId), Times.Once);
        _paymentRepositoryMock.Verify(x => x.UpdateAsync(payment), Times.Once);
        _unitOfWorkMock.Verify(x => x.RollbackAsync(), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        _eventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<PaymentProcessedEvent>()), Times.Never);
    }
}

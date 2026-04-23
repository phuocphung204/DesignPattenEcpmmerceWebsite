
// using DesignPattern.Application.Features.Users.Commands.CreateAccount;
// using DesignPattern.Domain.Entities.Users;
// using DesignPattern.Domain.Repositories;
// using Moq;

// namespace Test.Application.Commands;

// public class CreateAccountCommandTest
// {
//   [Fact]
//   public async Task Handle_ValidRequest_ReturnsSuccess()
//   {
//     // Arrange
//     var command = CreateAccountCommand(
//       FullName: "John Doe",
//       Email: "john.doe@example.com",
//       // PhoneNumber: "1234567890",
//       Password: "password123",
//       Address: new Address(
//         Street: "123 Main St",
//         City: "Anytown",
//         State: "Anystate",
//         ZipCode: "12345",
//         Country: "USA"
//       )
//     );

//     var unitOfWorkMock = new Mock<IUnitOfWork>();

//     unitOfWorkMock.Setup(u => u.UserRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
//       .ReturnsAsync((User?)null); // Simulate no existing user with the email
//     unitOfWorkMock.Setup(u => u.UserRepository.Create(It.IsAny<User>()));
//     unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
//       .Returns(Task.FromResult(1));

//     var handler = new CreateAccountCommandHandler(unitOfWorkMock.Object);

//     // Act
//     var result = await handler.Handle(command, CancellationToken.None);

//     // Assert
//     Assert.True(result.IsSuccess);
//   }
// }
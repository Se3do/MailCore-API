using System.Threading;
using System.Threading.Tasks;
using MailService.Application.DTOs.Auth;
using MailService.Application.Interfaces.Security;
using MailService.Application.Services;
using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using Moq;

namespace MailService.Application.Tests.Services;

public class AuthServiceTests
{
 private readonly Mock<IUserRepository> _userRepo = new();
 private readonly Mock<IUnitOfWork> _unitOfWork = new();
 private readonly Mock<ITokenGenerator> _tokenGen = new();
 private readonly AuthService _sut;
 private readonly User _user;
 private const string Token = "jwt-token";

 public AuthServiceTests()
 {
 _sut = new AuthService(_userRepo.Object, _unitOfWork.Object, _tokenGen.Object);
 _user = User.Create("Test User", "test@example.com", "password");
 _user.Name = "Test User";
 }

 [Fact]
 public async Task LoginAsync_WithValidCredentials_ReturnsAuthResult()
 {
 _userRepo.Setup(r => r.GetByEmailAsync(_user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(_user);
 _tokenGen.Setup(t => t.Generate(_user)).Returns(Token);

 var result = await _sut.LoginAsync(_user.Email, "password", CancellationToken.None);

 Assert.NotNull(result);
 Assert.Equal(_user.Id, result!.UserId);
 Assert.Equal(Token, result.Token);
 }

 [Fact]
 public async Task LoginAsync_UserNotFound_ReturnsNull()
 {
 _userRepo.Setup(r => r.GetByEmailAsync(_user.Email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

 var result = await _sut.LoginAsync(_user.Email, "password", CancellationToken.None);

 Assert.Null(result);
 }

 [Fact]
 public async Task LoginAsync_InvalidPassword_ReturnsNull()
 {
 var user = User.Create(_user.Name, _user.Email, "otherpassword");
 _userRepo.Setup(r => r.GetByEmailAsync(_user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

 var result = await _sut.LoginAsync(_user.Email, "wrongpassword", CancellationToken.None);

 Assert.Null(result);
 }

 [Fact]
 public async Task RegisterAsync_CreatesUserAndReturnsAuthResult()
 {
 _userRepo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(_user);
 _tokenGen.Setup(t => t.Generate(It.IsAny<User>())).Returns(Token);

 var result = await _sut.RegisterAsync("Test User", "new@example.com", "password", CancellationToken.None);

 Assert.NotNull(result);
 Assert.Equal(Token, result.Token);
 Assert.Equal(_user.Id, result.UserId);
 _userRepo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
 }
}

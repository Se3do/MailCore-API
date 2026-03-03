using System.Threading;
using System.Threading.Tasks;
using MailCore.Application.DTOs.Auth;
using MailCore.Application.Exceptions;
using MailCore.Application.Interfaces.Security;
using MailCore.Application.Services;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Moq;

namespace MailCore.Application.Tests.Services;

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
 }

 // ?? Login ??????????????????????????????????????????????????????????????

 [Fact]
 public async Task LoginAsync_ValidCredentials_ReturnsAuthResult()
 {
 _userRepo.Setup(r => r.GetByEmailAsync(_user.Email, default)).ReturnsAsync(_user);
 _tokenGen.Setup(t => t.Generate(_user)).Returns(Token);

 var result = await _sut.LoginAsync(_user.Email, "password", default);

 Assert.NotNull(result);
 Assert.Equal(_user.Id, result!.UserId);
 Assert.Equal(Token, result.Token);
 }

 [Fact]
 public async Task LoginAsync_UserNotFound_ReturnsNull()
 {
 _userRepo.Setup(r => r.GetByEmailAsync(_user.Email, default)).ReturnsAsync((User?)null);

 var result = await _sut.LoginAsync(_user.Email, "password", default);

 Assert.Null(result);
 }

 [Fact]
 public async Task LoginAsync_WrongPassword_ReturnsNull()
 {
 var user = User.Create(_user.Name, _user.Email, "correct-password");
 _userRepo.Setup(r => r.GetByEmailAsync(_user.Email, default)).ReturnsAsync(user);

 var result = await _sut.LoginAsync(_user.Email, "wrong-password", default);

 Assert.Null(result);
 }

 // ?? Register ???????????????????????????????????????????????????????????

 [Fact]
 public async Task RegisterAsync_NewEmail_CreatesUserAndReturnsToken()
 {
 _userRepo.Setup(r => r.ExistsByEmailAsync("new@example.com", default)).ReturnsAsync(false);
 _userRepo.Setup(r => r.AddAsync(It.IsAny<User>(), default)).ReturnsAsync(_user);
 _tokenGen.Setup(t => t.Generate(It.IsAny<User>())).Returns(Token);

 var result = await _sut.RegisterAsync("New User", "new@example.com", "password", default);

 Assert.NotNull(result);
 Assert.Equal(Token, result.Token);
 _userRepo.Verify(r => r.AddAsync(It.IsAny<User>(), default), Times.Once);
 _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
 }

 [Fact]
 public async Task RegisterAsync_DuplicateEmail_ThrowsValidationException()
 {
 _userRepo.Setup(r => r.ExistsByEmailAsync("dupe@example.com", default)).ReturnsAsync(true);

 await Assert.ThrowsAsync<ValidationException>(
 () => _sut.RegisterAsync("User", "dupe@example.com", "password", default));
 }
}

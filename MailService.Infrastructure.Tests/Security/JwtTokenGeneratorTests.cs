using MailService.Domain.Entities;
using MailService.Infrastructure.Security;
using Xunit;
using System.Collections.Generic;

namespace MailService.Infrastructure.Tests.Security;

public class JwtTokenGeneratorTests
{
 [Fact]
 public void Generate_Returns_Valid_Jwt_Token()
 {
 // Arrange
 var user = new User
 {
 Id = Guid.NewGuid(),
 Email = "test@example.com",
 Name = "Test User"
 };
 var config = new TestConfiguration(new Dictionary<string, string>
 {
 {"Jwt:Secret", "supersecretkeysupersecretkey123!"},
 {"Jwt:Issuer", "TestIssuer"},
 {"Jwt:Audience", "TestAudience"},
 {"Jwt:ExpiryMinutes", "60"}
 });
 var generator = new JwtTokenGenerator(config);

 // Act
 var token = generator.Generate(user);

 // Assert
 Assert.False(string.IsNullOrWhiteSpace(token));
 // Optionally, add more checks for token format, claims, etc.
 }
}

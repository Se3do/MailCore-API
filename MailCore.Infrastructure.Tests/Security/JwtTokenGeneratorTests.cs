using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MailCore.Domain.Entities;
using MailCore.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MailCore.Infrastructure.Tests.Security;

public class JwtTokenGeneratorTests
{
 private const string Secret = "supersecretkeysupersecretkey1234";
 private const string Issuer = "TestIssuer";
 private const string Audience = "TestAudience";

 private static readonly User TestUser = new()
 {
 Id = Guid.NewGuid(),
 Email = "test@example.com",
 Name = "Test User"
 };

 private static readonly JwtTokenGenerator Generator = new(
 new ConfigurationBuilder()
 .AddInMemoryCollection(new Dictionary<string, string?>
 {
 { "Jwt:Secret", Secret },
 { "Jwt:Issuer", Issuer },
 { "Jwt:Audience", Audience },
 { "Jwt:ExpiryMinutes", "60" }
 })
 .Build());

 private static TokenValidationParameters ValidationParams => new()
 {
 ValidateIssuer = true,
 ValidateAudience = true,
 ValidateLifetime = true,
 ValidateIssuerSigningKey = true,
 ValidIssuer = Issuer,
 ValidAudience = Audience,
 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
 ClockSkew = TimeSpan.Zero
 };

 [Fact]
 public void Generate_ReturnsNonEmptyToken()
 {
 Assert.False(string.IsNullOrWhiteSpace(Generator.Generate(TestUser)));
 }

 [Fact]
 public void Generate_Token_PassesSignatureValidation()
 {
 var token = Generator.Generate(TestUser);
 var principal = new JwtSecurityTokenHandler()
 .ValidateToken(token, ValidationParams, out _);

 Assert.NotNull(principal);
 }

 [Fact]
 public void Generate_Token_ContainsNameIdentifierClaim()
 {
 var claims = ParseClaims(Generator.Generate(TestUser));
 Assert.Contains(claims, c =>
 c.Type == ClaimTypes.NameIdentifier &&
 c.Value == TestUser.Id.ToString());
 }

 [Fact]
 public void Generate_Token_ContainsEmailClaim()
 {
 var claims = ParseClaims(Generator.Generate(TestUser));
 Assert.Contains(claims, c =>
 c.Type == JwtRegisteredClaimNames.Email &&
 c.Value == TestUser.Email);
 }

 [Fact]
 public void Generate_Token_ContainsSubClaim()
 {
 var claims = ParseClaims(Generator.Generate(TestUser));
 Assert.Contains(claims, c =>
 c.Type == JwtRegisteredClaimNames.Sub &&
 c.Value == TestUser.Id.ToString());
 }

 [Fact]
 public void Generate_Token_ContainsJtiClaim()
 {
 var claims = ParseClaims(Generator.Generate(TestUser));
 Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Jti);
 }

 [Fact]
 public void Generate_TwoCallsSameUser_ProduceDifferentJti()
 {
 var jti1 = ParseClaims(Generator.Generate(TestUser))
 .First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
 var jti2 = ParseClaims(Generator.Generate(TestUser))
 .First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

 Assert.NotEqual(jti1, jti2);
 }

 [Fact]
 public void Generate_Token_ExpiresInConfiguredMinutes()
 {
 var token = new JwtSecurityTokenHandler().ReadJwtToken(Generator.Generate(TestUser));
 var expectedExpiry = DateTime.UtcNow.AddMinutes(60);

 Assert.True(token.ValidTo <= expectedExpiry.AddSeconds(5));
 Assert.True(token.ValidTo >= expectedExpiry.AddSeconds(-5));
 }

 private static IEnumerable<Claim> ParseClaims(string token) =>
 new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;
}

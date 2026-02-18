using MailService.Application.DTOs.Auth;
using MailService.Application.Interfaces.Security;
using MailService.Application.Interfaces.Services;
using MailService.Domain.Entities;
using MailService.Domain.Interfaces;

namespace MailService.Application.Services
{
    public class AuthService : IAuthService
    {
        IUserRepository _userRepository;
        ITokenGenerator _tokenGenerator;

        public AuthService(IUserRepository userRepository, ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthResultDto?> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (user is null)
                return null;

            if (!user.VerifyPassword(password))
                return null;

            var token = _tokenGenerator.Generate(user);

            return new AuthResultDto(user.Id, token);
        }

        public async Task<AuthResultDto> RegisterAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = User.Create(email, password);

            await _userRepository.AddAsync(user, cancellationToken);

            var token = _tokenGenerator.Generate(user);

            return new AuthResultDto(user.Id, token);
        }
    }
}

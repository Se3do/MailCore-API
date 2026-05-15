using MailCore.Application.DTOs.Auth;
using MailCore.Application.Interfaces.Services;
using MediatR;

namespace MailCore.Application.Commands.Auth.LoginUser;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResultDto?>
{
    private readonly IAuthService _authService;

    public LoginUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResultDto?> Handle(LoginUserCommand command, CancellationToken ct)
    {
        return await _authService.LoginAsync(command.Email, command.Password, ct);
    }
}

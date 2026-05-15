using MailCore.Application.DTOs.Auth;
using MailCore.Application.Interfaces.Services;
using MediatR;

namespace MailCore.Application.Commands.Auth.RegisterUser;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly IAuthService _authService;

    public RegisterUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResultDto> Handle(RegisterUserCommand command, CancellationToken ct)
    {
        return await _authService.RegisterAsync(command.Name, command.Email, command.Password, ct);
    }
}

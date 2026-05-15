using MailCore.Application.DTOs.Auth;
using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Auth.RegisterUser;

public record RegisterUserCommand(string Name, string Email, string Password) : IRequest<AuthResultDto>, ICommand;

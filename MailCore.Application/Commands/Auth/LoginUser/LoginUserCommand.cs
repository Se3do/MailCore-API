using MailCore.Application.DTOs.Auth;
using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Auth.LoginUser;

public record LoginUserCommand(string Email, string Password) : IRequest<AuthResultDto?>, ICommand;

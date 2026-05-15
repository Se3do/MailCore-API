using Asp.Versioning;
using MailCore.API.Contracts.Requests;
using MailCore.Application.Commands.Auth.LoginUser;
using MailCore.Application.Commands.Auth.RegisterUser;
using MailCore.Application.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MailCore.API.Controllers
{
    /// <summary>Handles user authentication operations including registration and login.</summary>
    [Route("api/v{version:apiVersion}/auth")]
    [ApiController]
    [ApiVersion("1.0")]
    [EnableRateLimiting("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>Registers a new user account and returns a JWT token.</summary>
        /// <param name="request">The registration details containing name, email, and password.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>An <see cref="AuthResultDto"/> containing the new user ID and JWT token.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var result = await _sender.Send(new RegisterUserCommand(request.Name, request.Email, request.Password), ct);
            return Ok(result);
        }

        /// <summary>Authenticates a user and returns a JWT token.</summary>
        /// <param name="request">The login credentials containing email and password.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>An <see cref="AuthResultDto"/> containing the user ID and JWT token.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await _sender.Send(new LoginUserCommand(request.Email, request.Password), ct);
            if (result is null)
                return Unauthorized();
            return Ok(result);
        }
    }
}
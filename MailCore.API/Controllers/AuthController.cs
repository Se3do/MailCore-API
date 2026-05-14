using Asp.Versioning;
using FluentValidation;
using MailCore.API.Contracts.Requests;
using MailCore.Application.DTOs.Auth;
using MailCore.Application.Interfaces.Services;
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
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;

        public AuthController(
            IAuthService authService,
            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        // POST /api/auth/register
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
            var validation = await _registerValidator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage);
                return BadRequest(new { error = string.Join("; ", errors) });
            }

            var result = await _authService.RegisterAsync(request.Name, request.Email, request.Password, ct);

            return Ok(result);
        }

        // POST /api/auth/login
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
            var validation = await _loginValidator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage);
                return BadRequest(new { error = string.Join("; ", errors) });
            }

            var result = await _authService.LoginAsync(request.Email, request.Password, ct);
            if (result == null)
            {
                return Unauthorized();
            }
            return Ok(result);
        }
    }
}
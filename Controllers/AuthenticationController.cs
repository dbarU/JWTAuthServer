using JWTAuthServer.Models;
using JWTAuthServer.Models.Response;
using JWTAuthServer.Services.Authenticators;
using JWTAuthServer.Services.HashHelper;
using JWTAuthServer.Services.RefreshTokenRepository;
using JWTAuthServer.Services.TokenValidator;
using JWTAuthServer.Services.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace JWTAuthServer.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly Authenticator _authenticator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly RefreshTokenValidator _refreshTokenValidator;

        public AuthenticationController(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            RefreshTokenValidator refreshTokenValidator,
            Authenticator authenticator
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenValidator = refreshTokenValidator;
            _authenticator = authenticator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest message)
        {
            if (!ModelState.IsValid)
                return ValidateModelState(ModelState);

            var user = await _userRepository.GetByUserNameAsync(message.UserName);
            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            if (!_passwordHasher.VerifyPassword(message.Password, user.PasswordHash))
                return BadRequest(new { message = "Username or password is incorrect" });

            var authResponse = await _authenticator.Authenticate(user);
            if (authResponse == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(authResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest message)
        {
            if (!ModelState.IsValid)
                return ValidateModelState(ModelState);

            if (message.Password != message.ConfirmPassword)
                return BadRequest(new ErrorResponse("Passwords do not match"));

            var userByEmail = await _userRepository.GetByEmailAsync(message.EmailAddress);
            if (userByEmail != null)
                return BadRequest(new ErrorResponse("Email address is already in use"));

            string passHash = _passwordHasher.HashPassword(message.Password);
            var userToRegister = new User
            {
                EmailAddress = message.EmailAddress,
                UserName = message.UserName,
                PasswordHash = passHash
            };

            var registeredUser = await _userRepository.CreateUserAsync(userToRegister);
            if (registeredUser == null)
                return BadRequest(new ErrorResponse("Failed to register user"));

            return Ok(registeredUser);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest message)
        {
            if (!ModelState.IsValid)
                return ValidateModelState(ModelState);
            //TODO: Return detailed explanation of validation errors
            bool isRefreshTokenValid = _refreshTokenValidator.Validate(message.RefreshToken);
            if (isRefreshTokenValid)
                return BadRequest(new ErrorResponse("Invalid refresh token"));

            RefreshToken refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(
                message.RefreshToken
            );
            if (refreshToken == null)
                return NotFound(new ErrorResponse("Token not found"));


            await _refreshTokenRepository.DeleteRefreshTokenAsync(refreshToken.Id);

            User? user = await _userRepository.GetUserByIdAsync(refreshToken.UserId);
            if (refreshToken == null)
                return NotFound(new ErrorResponse("User not found"));

            var authResponse = await _authenticator.Authenticate(user);

            if (authResponse == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(authResponse);
        }

        [Authorize]
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {


            string rawUserId = HttpContext.User.FindFirstValue("id");
            if (!Guid.TryParse(rawUserId, out Guid userId))
            {
                return Unauthorized();
            }

            await _refreshTokenRepository.DeleteAllByUserIdAsync(userId);

            return new NoContentResult();
        }

        public IActionResult ValidateModelState(ModelStateDictionary modelState)
        {
            IEnumerable<string> errorsMessages = modelState.Values.SelectMany(
                v => v.Errors.Select(x => x.ErrorMessage)
            );
            return BadRequest(new ErrorResponse(errorsMessages));
        }
    }
}

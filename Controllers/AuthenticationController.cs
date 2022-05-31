using JWTAuthServer.Models;
using JWTAuthServer.Models.Response;
using JWTAuthServer.Services.Authenticators;
using JWTAuthServer.Services.RefreshTokenRepository;
using JWTAuthServer.Services.TokenValidator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace JWTAuthServer.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        //private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly Authenticator _authenticator;
        //private readonly IPasswordHasher _passwordHasher;
        private readonly RefreshTokenValidator _refreshTokenValidator;

        public AuthenticationController(
            IRefreshTokenRepository refreshTokenRepository,
            RefreshTokenValidator refreshTokenValidator,
            Authenticator authenticator, UserManager<User> userManager, IPasswordHasher<User> passwordHasher)
        {
            //_userRepository = userRepository;
            //_passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenValidator = refreshTokenValidator;
            _authenticator = authenticator;
            _userRepository = userManager;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest message)
        {
            if (!ModelState.IsValid)
                return ValidateModelState(ModelState);

            var user = await _userRepository.FindByNameAsync(message.UserName);
            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            bool isCorrectPassword = await _userRepository.CheckPasswordAsync(user, message.Password);
            if (!isCorrectPassword)
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

            var userByEmail = await _userRepository.FindByEmailAsync(message.EmailAddress);
            if (userByEmail != null)
                return BadRequest(new ErrorResponse("Email address is already in use"));

            var userToRegister = new User
            {
                Email = message.EmailAddress,
                UserName = message.UserName
                ,
                PasswordHash = ""
            };

            var identityResult = await _userRepository.CreateAsync(userToRegister, userToRegister.PasswordHash);
            if (!identityResult.Succeeded)
            {
                IdentityErrorDescriber errorDescriber = new IdentityErrorDescriber();
                var primaryError = identityResult.Errors.FirstOrDefault();
                if (primaryError.Code == nameof(errorDescriber.DuplicateEmail))
                    return BadRequest(new ErrorResponse("Email address is already in use"));

            }
            if (identityResult == null)
                return BadRequest(new ErrorResponse("Failed to register user"));

            return Ok(identityResult);
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

            User? user = await _userRepository.FindByIdAsync(refreshToken.UserId.ToString());
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

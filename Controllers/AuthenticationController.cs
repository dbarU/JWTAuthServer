using System;

namespace JWTAuthServer.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Login(string username, string password)
        {
            var user = _userService.Authenticate(username, password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (message.Password != message.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match" });

            var user = await _userService.CreateUserAsync(
                message.EmailAddress,
                message.UserName,
                message.Password
            );

            if (user == null)
                return BadRequest(new { message = "Username already exists" });

            return Ok(user);
        }
    }
}

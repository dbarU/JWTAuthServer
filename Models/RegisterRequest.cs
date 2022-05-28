using System;

namespace JWTAuthServer.Models
{
    /// <summary>
    /// RegisterRequest is used to register a new user.
    /// </summary>
    /// <value></value>
    public record RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; init; }

        [Required]
        public string UserName { get; init; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }
}

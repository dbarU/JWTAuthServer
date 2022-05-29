using System;
using System.ComponentModel.DataAnnotations;

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
        public string EmailAddress { get; init; } = string.Empty;

        [Required]
        public string UserName { get; init; } = string.Empty;

        [Required]
        public string Password { get; init; } = string.Empty;

        [Required]
        public string ConfirmPassword { get; init; } = string.Empty;
    }  }

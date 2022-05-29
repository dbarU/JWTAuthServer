using System.ComponentModel.DataAnnotations;

namespace JWTAuthServer.Models
{
    public record RefreshRequest([Required] string RefreshToken);

}

using System.Security.Claims;

namespace Wobalization.Api.Models;

public class IdentityModel
{
    public long Id { get; set; }

    public List<Claim> ToClaims()
    {
        return new List<Claim>
        {
            new("Id", Id.ToString()!, ClaimValueTypes.Integer64)
        };
    }
}
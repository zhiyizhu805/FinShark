using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;
//TokenService.cs
namespace api.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        //IConfiguration is an interface provided by .NET Core to access configuration data, such as values from appsettings.json
        public TokenService(IConfiguration config) 
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));// SymmetricSecurityKey is a security key used to encrypt and decrypt the token. This line converts the SigningKey from appsettings.json (a string) into a byte array, then into a SymmetricSecurityKey. This ensures that the token is encrypted in a way that only the server can decode, preventing tampering.
        }
        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>{  //These Claim come from System.Security.Claims.A Claim is a piece of information about the user (like their email or username) that will be embedded in the token.It helps the application identify the user. 
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, user.UserName) //The JwtRegisteredClaimNames class provides standardized claim types, such as Email and GivenName, that are recognized by JWT and help maintain a uniform format.
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature); //SigningCredentials is used to set up the credentials needed to sign the token, ensuring it hasn’t been altered after creation. //Here, SecurityAlgorithms.HmacSha512Signature specifies the signing algorithm to use (HMAC SHA-512 in this case), which is a common algorithm for securely signing JWTs.
            //SecurityTokenDescriptor is used to configure the JWT, setting properties such as below.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), //ClaimsIdentity is a class that groups claims together as an "identity." Here, it combines the user’s claims (like Email and UserName) for the token.//Subject: the identity containing claims.
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"], //determine which applications are allowed to use the token, as specified in appsettings.json.
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler(); //JwtSecurityTokenHandler is a helper class that creates and writes (serializes) the JWT.

            var token = tokenHandler.CreateToken(tokenDescriptor);//CreateToken(tokenDescriptor) uses the properties in tokenDescriptor to generate the token.

            return tokenHandler.WriteToken(token); //WriteToken will return the token in the form of string

        
        }
    }
}
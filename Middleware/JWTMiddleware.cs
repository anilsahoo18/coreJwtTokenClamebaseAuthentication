using coreJwtTokenClamebaseAuthentication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace coreJwtTokenClamebaseAuthentication.Middleware
{
    public class JWTMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public JWTMiddleware(RequestDelegate next, IConfiguration configuration, IUserRepository userRepository)
        {
            _next = next;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
              await  attachAccountToContext(context, token);
            }
            await _next(context);
        }

        private async Task attachAccountToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {

                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                     ValidAudience = _configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
               // string[] strParameters = jwtToken.ToString().Split(':');
                var accountId =  jwtToken.Claims.First(x => x.Type== ClaimTypes.Name).Value;

                // attach account to context on successful jwt validation
                context.Items["User"] = accountId;

            }
            catch
            {
                context.Items["User"] = null;
            }
        }
    }
}

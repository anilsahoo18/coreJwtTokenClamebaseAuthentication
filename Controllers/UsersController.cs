using coreJwtTokenClamebaseAuthentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace coreJwtTokenClamebaseAuthentication.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        private string generatedToken = null;
        int intValid = 0;
        public UsersController(IConfiguration config, ITokenService tokenService, IUserRepository userRepository)
        {
            _config = config;
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(Users userModel)
        {
            try
            {
                List<Outputclass> list = new List<Outputclass>();
                if (string.IsNullOrEmpty(userModel.Name) || string.IsNullOrEmpty(userModel.Password))
                {
                    Outputclass objent = new Outputclass();
                    objent.Status = 1;
                    objent.OutMessage = "User Id Or Password are blank";
                    list.Add(objent);
                    intValid = 1;
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status204NoContent, list);
                }
                IActionResult response = Unauthorized();

                var validUser = _userRepository.GetUser(userModel);
                if (validUser != null)
                {
                    generatedToken = _tokenService.BuildToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), validUser);

                    if (generatedToken != null)
                    {
                        HttpContext.Session.SetString("Token", generatedToken.ToString());
                        Outputclass objent = new Outputclass();
                        objent.Status = 2;
                        objent.OutMessage = "Token :-" + generatedToken;
                        list.Add(objent);
                        intValid = 0;

                        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, list);
                    }
                    else
                    {
                        Outputclass objent = new Outputclass();
                        objent.Status = 1;
                        objent.OutMessage = "Token Not Generated";
                        list.Add(objent);
                        intValid = 1;
                        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status204NoContent, list);
                    }
                }
                else
                {
                    Outputclass objent = new Outputclass();
                    objent.Status = 1;
                    objent.OutMessage = "User Id not valied";
                    list.Add(objent);
                    intValid = 1;
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound, list);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        // [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        //[AuthorizeAttribute]
        [Authorize(Roles = "manager")]
        [HttpGet]
        [Route("Allrecord")]
        public IActionResult Allrecord()
        {
            try
            {
                List<Outputclass> list = new List<Outputclass>();
                string token = HttpContext.Session.GetString("Token");
                if (token == null)
                {
                    Outputclass objent = new Outputclass();
                    objent.Status = 1;
                    objent.OutMessage = "Token Null";
                    list.Add(objent);
                    intValid = 1;
                    return StatusCode(1, list);
                }
                if (!_tokenService.ValidateToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), _config["Jwt:Issuer"].ToString(), token))
                {
                    Outputclass objent = new Outputclass();
                    objent.Status = 1;
                    objent.OutMessage = "Invalied Token";
                    list.Add(objent);
                    intValid = 1;
                    return StatusCode(3, list);
                }
                else
                {
                    Outputclass objent = new Outputclass();
                    objent.Status = 2;
                    objent.OutMessage = "Success";
                    objent.userRecord = _userRepository.Getalluserdata();
                    list.Add(objent);
                    intValid = 1;
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            
        }

    }
}

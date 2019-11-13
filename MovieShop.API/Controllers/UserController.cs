using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieShop.API.DTO;
using MovieShop.Entities;
using MovieShop.Services;

namespace MovieShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _config = configuration;
        }

        [Route("register")]
        [HttpPost]
        public async Task<ActionResult> CreateUserAsync([FromBody]CreateUserDTO createUserDTO)
        {
            if (createUserDTO == null || string.IsNullOrEmpty(createUserDTO.email) || string.IsNullOrEmpty(createUserDTO.password))
            {
                return BadRequest();
            }
            var user = await _userService.CreateUser(createUserDTO.email, createUserDTO.password, createUserDTO.firstName, createUserDTO.lastName);
            if (user == null)
            {
                return BadRequest("UserName already exists");
            }
            return Ok("User Created");
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult> LoginAsync([FromBody]UserLoginDTO userLoginDTO)
        {
            if (userLoginDTO == null || string.IsNullOrEmpty(userLoginDTO.email) || string.IsNullOrEmpty(userLoginDTO.password))
            {
                return BadRequest();
            }
            var user = await _userService.ValidateUser(userLoginDTO.email, userLoginDTO.password);
            if (user == null)
            {
                return BadRequest("UserName or password not correct");
            }
            return Ok(new
            {
                token = GenerateToken(user)
            });
        }

        [HttpGet]
        [Route("{id}/purchases")]
        [Authorize]
        public async Task<ActionResult> GetUserPurchasedMovies(int id)
        {
            var usermovies = await _userService.GetPurchases(id);
            return Ok(usermovies);
        }

        private string GenerateToken(User user)
        {
            //claim some inforation in payload part
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim("alias", user.FirstName[0] + user.LastName[0].ToString()),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenSettings:PrivateKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            // originally set as Now.AddDays
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["TokenSettings:ExpirationDays"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = credentials,
                Issuer = _config["TokenSettings:Issuer"],
                Audience = _config["TokenSettings:Audience"]
            };


            var encodedJwt = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
            return new JwtSecurityTokenHandler().WriteToken(encodedJwt);
        }
    }
}
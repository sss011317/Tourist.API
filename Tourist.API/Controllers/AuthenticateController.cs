using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tourist.API.Dtos;
using Tourist.API.Models;
using Tourist.API.Services;

namespace Tourist.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITouristRouteRepository _touristRouteRepository;
        public AuthenticateController(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
             SignInManager<ApplicationUser> signInManager,
             ITouristRouteRepository touristRouteRepository
            )
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _touristRouteRepository = touristRouteRepository;
        }
        [AllowAnonymous] //指任何人都能訪問，加不加都沒關係
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            //TODO:驗證用戶名密碼
            var loginResult = await _signInManager.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                false, //Persistent
                false //lockout 如果嘗試多次登入都失敗，是否要鎖起來
                );
            if (!loginResult.Succeeded)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(loginDto.Email);
            //TODO:創建JWT
            //header 
            var signingAlgorithm = SecurityAlgorithms.HmacSha256;
            //payload
            var claims = new List<Claim>
            {
                //sub <= jwt內ID的專有名詞 
                new Claim(JwtRegisteredClaimNames.Sub,user.Id),
                //new Claim(ClaimTypes.Role,"Admin")
            };
            var roleNames = await _userManager.GetRolesAsync(user);
            foreach(var rolename in roleNames)
            {
                var roleClaim = new Claim(ClaimTypes.Role,rolename);
                claims.Add(roleClaim);
            }

            //signiture
            var sercetByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
            var signingKey = new SymmetricSecurityKey(sercetByte);
            var signingCredentials = new SigningCredentials(signingKey, signingAlgorithm);

            var token = new JwtSecurityToken(
                issuer:_configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims,
                notBefore : DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials
                );
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            //TODO:return 200 OK + jwt token
            return Ok(tokenStr);
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            //1.使用用戶名建立用戶對象
            var user = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
            };
            //2.hash密碼，保存用戶
            var result = await _userManager.CreateAsync(user,registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            //3.給新用戶初始化購物車
            var shoppingCart = new ShoppingCart()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id
            };
            await _touristRouteRepository.CreateShoppingCart(shoppingCart);
            await _touristRouteRepository.SaveAsync();
            //4.return
            return Ok();
        } 
    }
}

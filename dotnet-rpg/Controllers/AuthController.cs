using AutoMapper;
using dotnet_rpg.Dtos.User;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;
[ApiController, Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;

    public AuthController(IAuthRepository authRepository, IMapper mapper)
    {
        _authRepository = authRepository;
        _mapper = mapper;
    }
    
    [HttpPost("Register")]
    public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto user)
    {
        ServiceResponse<int> response = await _authRepository.Register(_mapper.Map<User>(user), user.Password);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
    
    [HttpPost("Login")]
    public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDto user)
    {
        ServiceResponse<string> response = await _authRepository.Login(user.UserName, user.Password);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
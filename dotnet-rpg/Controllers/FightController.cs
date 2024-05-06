using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

// Leaving authorization for smoother development
// [Authorize]
[ApiController]
[Route("[controller]")]
public class FightController : ControllerBase
{
    private readonly IFightService _fightService;

    public FightController(IFightService fightService)
    {
        _fightService = fightService;
    }

    [HttpPost("WeaponAttack")]
    public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttack(WeaponAttackDto attack)
    {
        return Ok(await _fightService.WeaponAttack(attack));
    }
    
    [HttpPost("SkillAttack")]
    public async Task<ActionResult<ServiceResponse<AttackResultDto>>> SkillAttack(SkillAttackDto attack)
    {
        return Ok(await _fightService.SkillAttack(attack));
    }
    
    [HttpPost("Fight")]
    public async Task<ActionResult<ServiceResponse<FightResultDto>>> AddWeapon(FightRequestDto fightRequest)
    {
        return Ok(await _fightService.Fight(fightRequest));
    }
    
    [HttpGet("HighScore")]
    public async Task<ActionResult<ServiceResponse<List<HighScoreDto>>>> GetHighScore()
    {
        return Ok(await _fightService.GetHighScore());
    }
}
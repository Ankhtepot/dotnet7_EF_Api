using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using dotnet_rpg.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        
        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get()
        {
            return Ok(await _characterService.GetAllCharacters());
            // return BadRequest("No character found"); // For example
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> GetSingle(int id)
        {
            return Ok( await _characterService.GetCharacterById(id));
            // return BadRequest("No character found"); // For example
        }
        
        [HttpPost("AddCharacter")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(AddCharacterDto character)
        {
            return Ok(await _characterService.AddCharacter(character));
        }
        
        [HttpPut("UpdateCharacter")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(UpdateCharacterDto character)
        {
            ServiceResponse<GetCharacterDto> response = await _characterService.UpdateCharacter(character);
            
            if (!response.Success || response.Data == null)
            {
                return NotFound(response);
            }
            
            return Ok(response);
        }
        
        [HttpDelete("DeleteCharacter/{id:int}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = await _characterService.DeleteCharacterWithId(id);
            
            if (!response.Success || response.Data == null)
            {
                return NotFound(response);
            }
            
            return Ok(response);
        }
    }
}
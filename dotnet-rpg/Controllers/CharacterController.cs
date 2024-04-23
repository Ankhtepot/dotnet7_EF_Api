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
        public async Task<ActionResult<List<Character>>> Get()
        {
            return Ok(await _characterService.GetAllCharacters());
            // return BadRequest("No character found"); // For example
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<List<Character>>> GetSingle(int id)
        {
            return Ok( await _characterService.GetCharacterById(id));
            // return BadRequest("No character found"); // For example
        }
        
        [HttpPost("AddCharacter")]
        public async Task<ActionResult<List<Character>>> AddCharacter(Character character)
        {
            return Ok(await _characterService.AddCharacter(character));
        }
    }
}
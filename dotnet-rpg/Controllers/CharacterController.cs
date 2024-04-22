using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private static readonly List<Character> Characters = new() 
        {
            new Character(),
            new Character { Id = 1, Name = "Sam" }
        };

        [HttpGet("GetAll")]
        public ActionResult<List<Character>> Get()
        {
            return Ok(Characters);
            // return BadRequest("No character found"); // For example
        }
        
        [HttpGet("{id:int}")]
        public ActionResult<List<Character>> GetSingle(int id)
        {
            if (id < 0 || id >= Characters.Count)
                return NotFound("Character not found");
            
            return Ok(Characters[id]);
            // return BadRequest("No character found"); // For example
        }
    }
}
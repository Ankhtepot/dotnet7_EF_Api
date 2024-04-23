using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private static readonly List<Character> Characters = new() 
    {
        new Character(),
        new Character { Id = 1, Name = "Sam" }
    };
    
    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
        return new ServiceResponse<List<GetCharacterDto>>()
        {
            Data = Characters,
            Success = true,
            Message = "Get all characters",
        };
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
        Character? character = Characters.FirstOrDefault(c => c.Id == id);
        
        return new ServiceResponse<GetCharacterDto>()
        {
            Data = character,
            Success = character != null,
            Message = character == null ? "Index out of range" : "Get character by id",
        };
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto character)
    {
        Characters.Add(character);
        
        return new ServiceResponse<List<Character>>()
        {
            Data = Characters,
            Success = true,
            Message = "Add character",
        };
    }
}
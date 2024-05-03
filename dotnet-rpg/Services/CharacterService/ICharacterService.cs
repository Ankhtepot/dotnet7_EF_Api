using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services;

public interface ICharacterService
{
    Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters(int id);
    Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id);
    Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto character);
    Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto character);
    Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacterWithId(int id);
}
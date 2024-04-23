namespace dotnet_rpg.Services;

public interface ICharacterService
{
     Task<List<Character>> GetAllCharacters();
    Task<Character> GetCharacterById(int id);
    Task<List<Character>> AddCharacter(Character character);
}
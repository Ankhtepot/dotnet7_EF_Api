namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private static readonly List<Character> Characters = new() 
    {
        new Character(),
        new Character { Id = 1, Name = "Sam" }
    };
    
    public async Task<List<Character>> GetAllCharacters()
    {
        return Characters;
    }

    public async Task<Character> GetCharacterById(int id)
    {
        if (id < 0 || id >= Characters.Count)
        {
            throw new Exception("Character not found");
        };
        
        return Characters[id];
    }

    public async Task<List<Character>> AddCharacter(Character character)
    {
        Characters.Add(character);
        return Characters;
    }
}
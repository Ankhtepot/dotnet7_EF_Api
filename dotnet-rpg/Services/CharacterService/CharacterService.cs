using AutoMapper;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;

    public CharacterService(IMapper mapper, DataContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters(int id)
    {
        List<Character> dbCharacters = await _context.Characters.Where(ch => ch.User!.Id == id).ToListAsync();
        
        return new ServiceResponse<List<GetCharacterDto>>()
        {
            Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList(),
            Success = true,
            Message = "Get all characters",
        };
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
        Character? character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);

        return new ServiceResponse<GetCharacterDto>()
        {
            Data = _mapper.Map<GetCharacterDto>(character),
            Success = character != null,
            Message = character == null ? "Index out of range" : "Get character by id",
        };
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto character)
    {
        Character newCharacter = _mapper.Map<Character>(character);

        _context.Characters.Add(newCharacter);
        await _context.SaveChangesAsync();

        return new ServiceResponse<List<GetCharacterDto>>()
        {
            Data = _mapper.Map<List<GetCharacterDto>>(await _context.Characters.ToListAsync()),
            Success = true,
            Message = "Add character",
        };
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
        try
        {
            Character? character =  await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);

            if (character is null)
            {
                return new ServiceResponse<GetCharacterDto>()
                {
                    Data = null,
                    Success = false,
                    Message = $"Character with Id '{updatedCharacter.Id}' not found",
                };
            }
            
            _mapper.Map(updatedCharacter, character);
            await _context.SaveChangesAsync();
            
            // character.Name = updatedCharacter.Name;
            // character.HitPoints = updatedCharacter.HitPoints;
            // character.Strength = updatedCharacter.Strength;
            // character.Defense = updatedCharacter.Defense;
            // character.Intelligence = updatedCharacter.Intelligence;
            // character.Class = updatedCharacter.Class;

            return new ServiceResponse<GetCharacterDto>()
            {
                Data = _mapper.Map<GetCharacterDto>(character),
                Success = true,
                Message = "Update character",
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<GetCharacterDto>()
            {
                Data = null,
                Success = false,
                Message = e.Message,
            };
        }
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacterWithId(int id)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

        try
        {
            var character = await _context.Characters.FirstAsync(c => c.Id == id);
            
            if (character is null)
            {
                throw new Exception($"Character with Id '{id}' not found");
            }
            
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();

            serviceResponse.Data = _mapper.Map<List<GetCharacterDto>>(await _context.Characters.ToListAsync());
            serviceResponse.Success = true;
            serviceResponse.Message = "Delete character";
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }
        
        return serviceResponse;
    }
}
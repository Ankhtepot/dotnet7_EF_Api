using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
        List<Character> dbCharacters = await _context.Characters.Where(CharacterIdMatchPredicate())
            .Include(c => c.Weapon)
            .Include(c => c.Skills)
            .ToListAsync();

        return new ServiceResponse<List<GetCharacterDto>>()
        {
            Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList(),
            Success = true,
            Message = "Get all characters",
        };
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
        int userId = GetUserId();
        Character? character = await _context.Characters
            .Include(c => c.Weapon)
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == userId);

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
        newCharacter.User = await _context.Users.FirstOrDefaultAsync(UserIdMatchPredicate());

        _context.Characters.Add(newCharacter);
        await _context.SaveChangesAsync();

        return new ServiceResponse<List<GetCharacterDto>>()
        {
            Data = _mapper.Map<List<GetCharacterDto>>(await _context.Characters.Where(CharacterIdMatchPredicate())
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .ToListAsync()),
            Success = true,
            Message = "Add character",
        };
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
        try
        {
            Character? character = await _context.Characters
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);

            if (character is null || character.User!.Id != GetUserId())
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
        ServiceResponse<List<GetCharacterDto>> serviceResponse = new();

        try
        {
            int userId = GetUserId();
            Character character = await _context.Characters.FirstAsync(c => c.Id == id && c.User!.Id == userId);

            if (character is null)
            {
                throw new Exception($"Character with Id '{id}' not found");
            }

            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();

            serviceResponse.Data =
                _mapper.Map<List<GetCharacterDto>>(await _context.Characters.Where(CharacterIdMatchPredicate())
                    .ToListAsync());
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

    public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
    {
        ServiceResponse<GetCharacterDto> response = new();

        try
        {
            int userId = GetUserId();

            Character? character = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId && c.User!.Id == userId);

            if (character is null)
            {
                response.Success = false;
                response.Message = $"Character with Id '{newCharacterSkill.CharacterId}' not found";
                return response;
            }

            Skill? skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);

            if (skill is null)
            {
                response.Success = false;
                response.Message = $"Skill with Id '{newCharacterSkill.SkillId}' not found";
                return response;
            }

            character.Skills!.Add(skill);
            await _context.SaveChangesAsync();

            response.Data = _mapper.Map<GetCharacterDto>(character);
            response.Success = true;
            response.Message = "Added character skill";
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    private int GetUserId() =>
        int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private Expression<Func<Character, bool>> CharacterIdMatchPredicate()
    {
        int userId = GetUserId();
        return ch => ch.User!.Id == userId;
    }

    private Expression<Func<User, bool>> UserIdMatchPredicate()
    {
        int userId = GetUserId();
        return u => u.Id == userId;
    }
}
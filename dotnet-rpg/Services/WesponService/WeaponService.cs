using System.Security.Claims;
using AutoMapper;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.WesponService;

public class WeaponService : IWeaponService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }
    
    public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
    {
        ServiceResponse<GetCharacterDto> response = new();

        try
        {
            Character? character = await _context.Characters
                .FirstOrDefaultAsync(ch => ch.Id == newWeapon.CharacterId &&
                                           ch.User!.Id == int.Parse(_httpContextAccessor.HttpContext!.User
                                               .FindFirstValue(ClaimTypes.NameIdentifier)!));

            if (character == null)
            {
                response.Success = false;
                response.Message = "Character not found.";
                return response;
            }

            Weapon weapon = new()
            {
                Name = newWeapon.Name,
                Damage = newWeapon.Damage,
                Character = character
            };

            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync();
            response.Data = _mapper.Map<GetCharacterDto>(character);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }
}
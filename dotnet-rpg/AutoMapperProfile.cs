using AutoMapper;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Dtos.Skill;
using dotnet_rpg.Dtos.User;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Models;

namespace dotnet_rpg;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Character, GetCharacterDto>();
        CreateMap<AddCharacterDto, Character>();
        CreateMap<UpdateCharacterDto, Character>();
        CreateMap<Character, HighScoreDto>();
        
        CreateMap<User, UserRegisterDto>();
        CreateMap<UserRegisterDto, User>();
        
        CreateMap<Weapon, GetWeaponDto>();
        
        CreateMap<Skill, GetSkillDto>();
    }
}
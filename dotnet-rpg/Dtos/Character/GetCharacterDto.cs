﻿using dotnet_rpg.Dtos.Skill;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Models;

namespace dotnet_rpg.Dtos.Character;

public class GetCharacterDto
{
    public int Id { get; set; }
    public string? Name { get; set; } = "NotSet";
    public int HitPoints { get; set; } = 100;
    public int Strength { get; set; } = 10;
    public int Defense { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public ERpgClass Class { get; set; } = ERpgClass.Warrior;
    public GetWeaponDto? Weapon { get; set; }
    public List<GetSkillDto>? Skills { get; set; }
    public int Fights { get; set; }
    public int Victories { get; set; }
    public int Defeats { get; set; }
}
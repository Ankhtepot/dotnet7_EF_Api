﻿using dotnet_rpg.Dtos.Skill;
using dotnet_rpg.Models;

namespace dotnet_rpg.Dtos.Character;

public class AddCharacterDto
{
    public string? Name { get; set; } = "NotSet";
    public int HitPoints { get; set; } = 100;
    public int Strength { get; set; } = 10;
    public int Defense { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public ERpgClass Class { get; set; } = ERpgClass.Warrior;
    public List<GetSkillDto>? Skills { get; set; }
}
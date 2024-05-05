using System.ComponentModel.DataAnnotations;

namespace dotnet_rpg.Dtos.Skill;

public class GetSkillDto
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public int Damage { get; set; }
}
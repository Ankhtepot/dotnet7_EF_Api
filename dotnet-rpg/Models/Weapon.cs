using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_rpg.Models;

public class Weapon
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public int Damage { get; set; }
    public Character? Character { get; set; }
    public int CharacterId { get; set; }
}
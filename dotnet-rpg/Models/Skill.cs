using System.ComponentModel.DataAnnotations;
using HostingEnvironmentExtensions = Microsoft.Extensions.Hosting.HostingEnvironmentExtensions;

namespace dotnet_rpg.Models;

public class Skill
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public int Damage { get; set; }
    public List<Character>? Character { get; set; }
}
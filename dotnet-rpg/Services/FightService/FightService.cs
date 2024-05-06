using AutoMapper;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.FightService;

public class FightService : IFightService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public FightService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
    {
        ServiceResponse<AttackResultDto> response = new();

        try
        {
            Character? attacker = await _context.Characters
                .Include(c => c.Weapon)
                .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

            Character? opponent = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == request.OpponentId);
            
            if (attacker == null || opponent == null || attacker.Weapon == null)
            {
                throw new Exception("Attacker, weapon or opponent not found.");
            }
            
            int damage = DoWeaponAttack(attacker, opponent, response);

            await _context.SaveChangesAsync();

            response.Data = new AttackResultDto
            {
                Attacker = attacker.Name ?? "Name was not found",
                Opponent = opponent.Name ?? "Name was not found",
                AttackerHp = attacker.HitPoints,
                OpponentHp = opponent.HitPoints,
                Damage = damage,
            };
            
            response.Message = $"{attacker.Name} attacked {opponent.Name} for {damage} damage.";
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }
        
        return response;
    }

    public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
    {
        ServiceResponse<AttackResultDto> response = new();

        try
        {
            Character? attacker = await _context.Characters
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

            Character? opponent = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

            if (attacker == null || opponent == null || attacker.Skills == null)
            {
                throw new Exception("Attacker, skills or opponent not found.");
            }

            Skill? characterSkill = attacker.Skills.FirstOrDefault(cs => cs.Id == request.SkillId);

            if (characterSkill == null)
            {
                response.Success = false;
                response.Message = $"{attacker.Name} does not know this skill.";
                return response;
            }

            int damage = DoSkillAttack(characterSkill, attacker, opponent, response);

            await _context.SaveChangesAsync();
            
            response.Message ??= $"{attacker.Name} attacked {opponent.Name} using {characterSkill.Name} for {damage} damage.";
            response.Data = new AttackResultDto
            {
                Attacker = attacker.Name ?? "Name was not found",
                Opponent = opponent.Name ?? "Name was not found",
                AttackerHp = attacker.HitPoints,
                OpponentHp = opponent.HitPoints,
                Damage = damage,
            };
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequest)
    {
        ServiceResponse<FightResultDto> response = new()
        {
            Data = new FightResultDto()
        };
        
        try
        {
            List<Character> characters = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .Where(c => fightRequest.CharacterIds.Contains(c.Id))
                .ToListAsync();

            bool defeated = false;
            while (!defeated)
            {
                foreach (Character attacker in characters)
                {
                    List<Character> opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                    Character opponent = opponents[new Random().Next(opponents.Count)];

                    int damage;
                    string attackUsed;
                    bool useWeapon = new Random().Next(2) == 0;

                    if (useWeapon && attacker.Weapon is not null)
                    {
                        attackUsed = attacker.Weapon.Name;
                        damage = DoWeaponAttack(attacker, opponent, response);
                    }
                    else if (attacker.Skills is not null && attacker.Skills.Any())
                    {
                        Skill characterSkill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                        attackUsed = characterSkill.Name;
                        damage = DoSkillAttack(characterSkill, attacker, opponent, response);
                    }
                    else
                    {
                        response.Data.Log.Add($"{attacker.Name} has no skills or weapon. {opponent.Name} wins this round.");
                        continue;
                    }
                    
                    response.Data.Log.Add($"{attacker.Name} attacked {opponent.Name} using {attackUsed} for {(damage >= 0 ? damage : 0)} damage.");

                    if (opponent.HitPoints <= 0)
                    {
                        defeated = true;
                        attacker.Victories++;
                        opponent.Defeats++;
                        response.Data.Log.Add($"{opponent.Name} has been defeated!\n");
                        response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left.");
                        break;
                    }
                }
            }

            characters.ForEach(c =>
            {
                c.Fights++;
                c.HitPoints = 100;
            });

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }
        
        return response;
    }
    
    public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore()
    {
        ServiceResponse<List<HighScoreDto>> response = new();
        
        try
        {
            List<Character> characters = await _context.Characters
                .Where(c => c.Fights > 0)
                .OrderByDescending(c => c.Victories)
                .ThenBy(c => c.Defeats)
                .ToListAsync();
            
            response.Message = "High score retrieved.";
            response.Data = characters.Select(c => _mapper.Map<HighScoreDto>(c)).ToList();
            // OR ->
            //     characters.Select(c => new HighScoreDto
            // {
            //     Id = c.Id,
            //     Name = c.Name!,
            //     Fights = c.Fights,
            //     Victories = c.Victories,
            //     Defeats = c.Defeats
            // }).ToList();
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }
    
    private static int DoWeaponAttack<T>(Character attacker, Character opponent, ServiceResponse<T> response)
    {
        if (attacker.Weapon == null)
        {
            response.Success = false;
            response.Message = $"{attacker.Name} has no weapon.";
            return 0;
        }
        
        int damage = attacker.Weapon.Damage + new Random().Next(attacker.Strength);
        damage -= new Random().Next(opponent.Defense);
            
        ManageDamageReceived(opponent, response, damage);

        return damage;
    }
    
    private static int DoSkillAttack<T>(Skill characterSkill, Character attacker, Character opponent, ServiceResponse<T> response)
    {
        int damage = characterSkill.Damage + new Random().Next(attacker.Intelligence);
        damage -= new Random().Next(opponent.Defense);
            
        ManageDamageReceived(opponent, response, damage);

        return damage;
    }

    private static void ManageDamageReceived<T>(Character opponent, ServiceResponse<T> response, int damage)
    {
        if (damage > 0)
        {
            opponent.HitPoints -= damage;
        }

        if (opponent.HitPoints <= 0)
        {
            response.Message = $"{opponent.Name} has been defeated!";
        }
    }
}
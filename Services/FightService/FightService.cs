﻿using DotNet_RPG.Data;
using DotNet_RPG.Dtos.Fight;
using DotNet_RPG.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet_RPG.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;

        public FightService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            ServiceResponse<FightResultDto> response = new ServiceResponse<FightResultDto> { Data = new FightResultDto()};

            try
            {

                List<Character> characters =
                    await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkills)
                    .ThenInclude(cs => cs.Skill)
                    .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();

                bool defeated = false;
                while (!defeated)
                {
                    foreach (Character attacker in characters)
                    {
                        List<Character> opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        Character opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        else
                        {
                            int randomSkill = new Random().Next(attacker.CharacterSkills.Count);
                            attackUsed = attacker.CharacterSkills[randomSkill].Skill.Name;
                            damage = DoSkillAttack(attacker, opponent, attacker.CharacterSkills[randomSkill]);
                        }


                        response.Data.Log.Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage..");

                        if (opponent.HitPoints <=0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated!!");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!!");
                            break;
                        }
                    }
                }


                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoints = 100;
                });

                _context.Characters.UpdateRange(characters);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }


            return  response;
        }







        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {

            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();

            try
            {
                Character attacker = await _context.Characters
                    .Include(b => b.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                Character opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);


                CharacterSkill characterSkill = attacker.CharacterSkills.FirstOrDefault(cs => cs.Skill.Id == request.SkillId);

                if (characterSkill == null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesnt know that skill";
                    return response;
                }

                int damage = DoSkillAttack(attacker, opponent, characterSkill);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated";

                _context.Characters.Update(opponent);
                await _context.SaveChangesAsync();


                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static int DoSkillAttack(Character attacker, Character opponent, CharacterSkill characterSkill)
        {
            int damage = characterSkill.Skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.HitPoints -= damage;
            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();

            try
            {
                Character attacker = await _context.Characters
                    .Include(b => b.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                Character opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                int damage = DoWeaponAttack(attacker, opponent);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated";

                _context.Characters.Update(opponent);
                await _context.SaveChangesAsync();


                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.HitPoints -= damage;
            return damage;
        }

    }
}

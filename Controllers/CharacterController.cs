using DotNet_RPG.Models;
using DotNet_RPG.Services.CharacterService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet_RPG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
      
        private readonly ICharacterService _characterService;

        public CharacterController (ICharacterService characterService)
        {
            _characterService = characterService;
        }


        [HttpGet("GetAll")]
        public IActionResult Get()
        {
            return Ok(_characterService.GetAllCharacter());
        }


        [HttpGet("{id}")]
        public IActionResult GetSingle(int id)
        {
            return Ok(_characterService.GetCharacterById(id));
        }


        public  IActionResult AddCharacter (Character newCharacter)
        {
            return Ok(_characterService.AddCharacter(newCharacter));
        }
    }
}

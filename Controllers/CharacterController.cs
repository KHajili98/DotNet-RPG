using DotNet_RPG.Models;
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
       // private static Character knigh = new Character();
        private static List<Character> characters = new List<Character> { 
        new Character(),
        new Character{Id=1,Name ="filan"}
        };


        [HttpGet("GetAll")]
        public IActionResult Get()
        {
            return Ok(characters);
        }


        [HttpGet("{id}")]
        public IActionResult GetSingle(int id)
        {
            return Ok(characters.FirstOrDefault(c=> c.Id==id));
        }


        public  IActionResult AddCharacter (Character newCharacter)
        {
            characters.Add(newCharacter);
            return Ok(characters);
        }
    }
}

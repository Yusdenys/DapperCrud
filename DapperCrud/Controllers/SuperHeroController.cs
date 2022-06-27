using Microsoft.AspNetCore.Mvc;
using DapperCrud.Models;
using System.Data.SqlClient;
using Dapper;

namespace DapperCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _config;
        public SuperHeroController(IConfiguration config)
        {
            this._config = config;
        }

        /**
         * GET Method
         */
        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHereos()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<SuperHero> heroes = await SelectAllHeroes(connection);
            return Ok(heroes);
        }

        [HttpGet("{heroId}")]
        public async Task<ActionResult<SuperHero>> GetHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var heroes = await connection.QueryAsync<SuperHero>("SELECT * FROM SuperHeroes WHERE Id = @Id"
                , new { Id = heroId });
            return Ok(heroes);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into superheroes (name,firstname,lastname,place) values (@Name,@FirstName,@LastName,@Place)", hero);
            return Ok(await SelectAllHeroes(connection));
        }

        [HttpPut]
        public async Task<ActionResult<SuperHero>> UpdateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update superheroes set name = @Name, firstname = @FirstName, lastname = @LastName, place = @Place where id = @Id", hero);
            return Ok(await SelectAllHeroes(connection));
        }

        [HttpDelete("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from superheroes where id = @Id", new { Id = heroId });
            return Ok(await SelectAllHeroes(connection));
        }

        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("SELECT * FROM SuperHeroes");
        }
    }
}

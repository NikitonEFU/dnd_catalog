using dnd_catalog_backend.Models;
using dnd_catalog_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // <-- ДОДАНО

namespace dnd_catalog_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RacesController : ControllerBase
    {
        private readonly IDndCatalogService _dndCatalogService;

        public RacesController(IDndCatalogService dndCatalogService)
        {
            _dndCatalogService = dndCatalogService;
        }

        // GET: api/Races (Доступно всім)
        [HttpGet]
        public async Task<ActionResult<List<Race>>> Get([FromQuery] bool includeAbilities = false) =>
            await _dndCatalogService.GetRacesAsync(includeAbilities);

        // GET: api/Races/{id} (Доступно всім)
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Race>> Get(string id, [FromQuery] bool includeAbilities = false)
        {
            var race = await _dndCatalogService.GetRaceAsync(id, includeAbilities);

            if (race is null)
            {
                return NotFound();
            }

            return race;
        }

        // POST: api/Races (Тільки для адмінів)
        [Authorize(Roles = "admin")] // <-- ДОДАНО ЗАХИСТ
        [HttpPost]
        public async Task<IActionResult> Post(Race newRace)
        {
            await _dndCatalogService.CreateRaceAsync(newRace);

            return CreatedAtAction(nameof(Get), new { id = newRace.Id }, newRace);
        }

        // PUT: api/Races/{id} (Тільки для адмінів)
        [Authorize(Roles = "admin")] // <-- ДОДАНО ЗАХИСТ
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Race updatedRace)
        {
            var race = await _dndCatalogService.GetRaceAsync(id);

            if (race is null)
            {
                return NotFound();
            }

            updatedRace.Id = race.Id;

            await _dndCatalogService.UpdateRaceAsync(id, updatedRace);

            return NoContent();
        }

        // DELETE: api/Races/{id} (Тільки для адмінів)
        [Authorize(Roles = "admin")] // <-- ДОДАНО ЗАХИСТ
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var race = await _dndCatalogService.GetRaceAsync(id);

            if (race is null)
            {
                return NotFound();
            }

            await _dndCatalogService.RemoveRaceAsync(id);

            return NoContent();
        }
    }
}
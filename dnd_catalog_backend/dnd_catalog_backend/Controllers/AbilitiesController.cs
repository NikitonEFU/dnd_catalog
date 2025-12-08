using dnd_catalog_backend.Models;
using dnd_catalog_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // <-- ДОДАНО

namespace dnd_catalog_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbilitiesController : ControllerBase
    {
        private readonly IDndCatalogService _dndCatalogService;

        public AbilitiesController(IDndCatalogService dndCatalogService)
        {
            _dndCatalogService = dndCatalogService;
        }

        // GET: api/Abilities (Доступно всім)
        [HttpGet]
        public async Task<ActionResult<List<Ability>>> Get() =>
            await _dndCatalogService.GetAbilitiesAsync();

        // GET: api/Abilities/{id} (Доступно всім)
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Ability>> Get(string id)
        {
            var ability = await _dndCatalogService.GetAbilityAsync(id);

            if (ability is null)
            {
                return NotFound();
            }

            return ability;
        }

        // POST: api/Abilities (Тільки для адмінів)
        [Authorize(Roles = "admin")] // <-- ДОДАНО ЗАХИСТ
        [HttpPost]
        public async Task<IActionResult> Post(Ability newAbility)
        {
            await _dndCatalogService.CreateAbilityAsync(newAbility);

            return CreatedAtAction(nameof(Get), new { id = newAbility.Id }, newAbility);
        }

        // PUT: api/Abilities/{id} (Тільки для адмінів)
        [Authorize(Roles = "admin")] // <-- ДОДАНО ЗАХИСТ
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Ability updatedAbility)
        {
            var ability = await _dndCatalogService.GetAbilityAsync(id);

            if (ability is null)
            {
                return NotFound();
            }

            updatedAbility.Id = ability.Id;

            await _dndCatalogService.UpdateAbilityAsync(id, updatedAbility);

            return NoContent();
        }

        // DELETE: api/Abilities/{id} (Тільки для адмінів)
        [Authorize(Roles = "admin")] // <-- ДОДАНО ЗАХИСТ
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ability = await _dndCatalogService.GetAbilityAsync(id);

            if (ability is null)
            {
                return NotFound();
            }

            await _dndCatalogService.RemoveAbilityAsync(id);

            return NoContent();
        }
    }
}
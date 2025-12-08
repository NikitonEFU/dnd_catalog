using dnd_catalog_backend.Models;
using dnd_catalog_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // <-- ДОДАНО

namespace dnd_catalog_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassesController : ControllerBase
    {
        private readonly IDndCatalogService _dndCatalogService;

        public ClassesController(IDndCatalogService dndCatalogService)
        {
            _dndCatalogService = dndCatalogService;
        }

        // GET: api/Classes (Доступно всім)
        [HttpGet]
        public async Task<ActionResult<List<Class>>> Get([FromQuery] bool includeAbilities = false) =>
            await _dndCatalogService.GetClassesAsync(includeAbilities);

        // GET: api/Classes/{id} (Доступно всім)
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Class>> Get(string id, [FromQuery] bool includeAbilities = false)
        {
            var @class = await _dndCatalogService.GetClassAsync(id, includeAbilities);

            if (@class is null)
            {
                return NotFound();
            }

            return @class;
        }

        // POST: api/Classes (Тільки для адмінів)
        [Authorize(Roles = "admin")] // <-- ДОДАНО ЗАХИСТ
        [HttpPost]
        public async Task<IActionResult> Post(Class newClass)
        {
            await _dndCatalogService.CreateClassAsync(newClass);

            return CreatedAtAction(nameof(Get), new { id = newClass.Id }, newClass);
        }

        // PUT: api/Classes/{id} (Тільки для адмінів)
        [Authorize(Roles = "admin")] // <-- ДОДАНО ЗАХИСТ
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Class updatedClass)
        {
            var @class = await _dndCatalogService.GetClassAsync(id);

            if (@class is null)
            {
                return NotFound();
            }

            updatedClass.Id = @class.Id;

            await _dndCatalogService.UpdateClassAsync(id, updatedClass);

            return NoContent();
        }

        // DELETE: api/Classes/{id} (Тільки для адмінів)
        [Authorize(Roles = "admin")] // <-- ДОДАНО ЗАХИСТ
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var @class = await _dndCatalogService.GetClassAsync(id);

            if (@class is null)
            {
                return NotFound();
            }

            await _dndCatalogService.RemoveClassAsync(id);

            return NoContent();
        }
    }
}
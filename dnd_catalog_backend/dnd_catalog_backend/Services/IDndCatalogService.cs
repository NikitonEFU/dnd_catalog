// Services/IDndCatalogService.cs (ПОВНИЙ ОНОВЛЕНИЙ КОД)
using dnd_catalog_backend.Models;

namespace dnd_catalog_backend.Services
{
    public interface IDndCatalogService
    {
        // CRUD для Abilities
        Task<List<Ability>> GetAbilitiesAsync();
        Task<Ability?> GetAbilityAsync(string id);
        Task CreateAbilityAsync(Ability newAbility);
        Task UpdateAbilityAsync(string id, Ability updatedAbility);
        Task RemoveAbilityAsync(string id);

        // CRUD для Races
        Task<List<Race>> GetRacesAsync(bool includeAbilities = false);
        Task<Race?> GetRaceAsync(string id, bool includeAbilities = false);
        Task CreateRaceAsync(Race newRace);
        Task UpdateRaceAsync(string id, Race updatedRace);
        Task RemoveRaceAsync(string id);

        // CRUD для Classes
        Task<List<Class>> GetClassesAsync(bool includeAbilities = false);
        Task<Class?> GetClassAsync(string id, bool includeAbilities = false);
        Task CreateClassAsync(Class newClass);
        Task UpdateClassAsync(string id, Class updatedClass);
        Task RemoveClassAsync(string id);

        // ------------------------------------
        // --- CRUD для Characters (НОВЕ) ---
        // ------------------------------------
        Task CreateCharacterAsync(Character newCharacter);
        Task<List<Character>> GetCharactersByUserIdAsync(string userId);
        Task<Character?> GetCharacterByIdAsync(string id);
        Task RemoveCharacterAsync(string id);
    }
}
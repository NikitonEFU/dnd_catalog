// Services/DndCatalogService.cs (ПОВНИЙ ОНОВЛЕНИЙ КОД)
using dnd_catalog_backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace dnd_catalog_backend.Services
{
    public class DndCatalogService : IDndCatalogService
    {
        private readonly IMongoCollection<Ability> _abilitiesCollection;
        private readonly IMongoCollection<Race> _racesCollection;
        private readonly IMongoCollection<Class> _classesCollection;

        // --- НОВА КОЛЕКЦІЯ ---
        private readonly IMongoCollection<Character> _charactersCollection;

        public DndCatalogService(IMongoDatabase database, IOptions<DndDatabaseSettings> settings)
        {
            var dndSettings = settings.Value;

            _abilitiesCollection = database.GetCollection<Ability>(dndSettings.AbilitiesCollectionName);
            _racesCollection = database.GetCollection<Race>(dndSettings.RacesCollectionName);
            _classesCollection = database.GetCollection<Class>(dndSettings.ClassesCollectionName);

            // --- ІНІЦІАЛІЗАЦІЯ НОВОЇ КОЛЕКЦІЇ ---
            // Примітка: Вам потрібно додати "CharactersCollectionName" до DndDatabaseSettings
            _charactersCollection = database.GetCollection<Character>(dndSettings.CharactersCollectionName);
        }

        // ------------------------------------
        // --- Допоміжні методи (JOIN/LOOKUP) ---
        // ------------------------------------

        // Функція для отримання пов'язаних Abilities за списком ID
        private async Task<List<Ability>> GetAbilitiesByIds(List<string> ids)
        {
            if (ids == null || !ids.Any())
                return new List<Ability>();

            var filter = Builders<Ability>.Filter.In(a => a.Id, ids);
            return await _abilitiesCollection.Find(filter).ToListAsync();
        }

        // Функція для додавання пов'язаних Ability до Race
        private async Task<Race> PopulateRaceAbilities(Race race)
        {
            race.Abilities = await GetAbilitiesByIds(race.AbilityIds);
            return race;
        }

        // Функція для додавання пов'язаних Ability до Class
        private async Task<Class> PopulateClassAbilities(Class @class)
        {
            @class.Abilities = await GetAbilitiesByIds(@class.AbilityIds);
            return @class;
        }

        // ------------------------------------
        // --- Реалізація: Characters CRUD (НОВЕ) ---
        // ------------------------------------

        public async Task CreateCharacterAsync(Character newCharacter) =>
            await _charactersCollection.InsertOneAsync(newCharacter);

        public async Task<List<Character>> GetCharactersByUserIdAsync(string userId) =>
            await _charactersCollection.Find(c => c.UserId == userId).ToListAsync();

        public async Task<Character?> GetCharacterByIdAsync(string id) =>
            await _charactersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task RemoveCharacterAsync(string id) =>
            await _charactersCollection.DeleteOneAsync(x => x.Id == id);

        // ------------------------------------
        // --- Реалізація: Abilities CRUD ---
        // ------------------------------------

        public async Task<List<Ability>> GetAbilitiesAsync() =>
            await _abilitiesCollection.Find(_ => true).ToListAsync();

        public async Task<Ability?> GetAbilityAsync(string id) =>
            await _abilitiesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAbilityAsync(Ability newAbility) =>
            await _abilitiesCollection.InsertOneAsync(newAbility);

        public async Task UpdateAbilityAsync(string id, Ability updatedAbility) =>
            await _abilitiesCollection.ReplaceOneAsync(x => x.Id == id, updatedAbility);

        public async Task RemoveAbilityAsync(string id) =>
            await _abilitiesCollection.DeleteOneAsync(x => x.Id == id);

        // ------------------------------------
        // --- Реалізація: Races CRUD (з JOIN) ---
        // ------------------------------------

        public async Task<List<Race>> GetRacesAsync(bool includeAbilities = false)
        {
            var races = await _racesCollection.Find(_ => true).ToListAsync();

            if (includeAbilities)
            {
                var tasks = races.Select(PopulateRaceAbilities).ToList();
                await Task.WhenAll(tasks);
            }
            return races;
        }

        public async Task<Race?> GetRaceAsync(string id, bool includeAbilities = false)
        {
            var race = await _racesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (race != null && includeAbilities)
            {
                await PopulateRaceAbilities(race);
            }
            return race;
        }

        public async Task CreateRaceAsync(Race newRace) =>
            await _racesCollection.InsertOneAsync(newRace);

        public async Task UpdateRaceAsync(string id, Race updatedRace) =>
            await _racesCollection.ReplaceOneAsync(x => x.Id == id, updatedRace);

        public async Task RemoveRaceAsync(string id) =>
            await _racesCollection.DeleteOneAsync(x => x.Id == id);

        // ------------------------------------
        // --- Реалізація: Classes CRUD (з JOIN) ---
        // ------------------------------------

        public async Task<List<Class>> GetClassesAsync(bool includeAbilities = false)
        {
            var classes = await _classesCollection.Find(_ => true).ToListAsync();

            if (includeAbilities)
            {
                var tasks = classes.Select(PopulateClassAbilities).ToList();
                await Task.WhenAll(tasks);
            }
            return classes;
        }

        public async Task<Class?> GetClassAsync(string id, bool includeAbilities = false)
        {
            var @class = await _classesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (@class != null && includeAbilities)
            {
                await PopulateClassAbilities(@class);
            }
            return @class;
        }

        public async Task CreateClassAsync(Class newClass) =>
            await _classesCollection.InsertOneAsync(newClass);

        public async Task UpdateClassAsync(string id, Class updatedClass) =>
            await _classesCollection.ReplaceOneAsync(x => x.Id == id, updatedClass);

        public async Task RemoveClassAsync(string id) =>
            await _classesCollection.DeleteOneAsync(x => x.Id == id);
    }
}
// Models/DndDatabaseSettings.cs
namespace dnd_catalog_backend.Models
{
    public class DndDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string RacesCollectionName { get; set; } = null!;
        public string ClassesCollectionName { get; set; } = null!;
        public string AbilitiesCollectionName { get; set; } = null!;
        public string CharactersCollectionName { get; set; } = null!;
    }
}
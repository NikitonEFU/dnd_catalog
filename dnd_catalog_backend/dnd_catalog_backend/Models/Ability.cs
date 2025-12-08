// Models/Ability.cs
using MongoDB.Bson.Serialization.Attributes;

namespace dnd_catalog_backend.Models
{
    // Здібності не мають зовнішніх посилань, успадковуються від MongoBase
    public class Ability : MongoBase
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
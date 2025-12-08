// Models/Class.cs
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace dnd_catalog_backend.Models
{
    public class Class : MongoBase
    {
        public string Name { get; set; } = null!;
        public string HitDie { get; set; } = null!; // Наприклад: d8, d10, d12

        // Зв'язок з Ability: Клас має список ID Здібностей
        public List<string> AbilityIds { get; set; } = new List<string>();

        // Властивість для ПОВНОГО відображення (JOIN)
        [BsonIgnore]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Ability>? Abilities { get; set; }
    }
}
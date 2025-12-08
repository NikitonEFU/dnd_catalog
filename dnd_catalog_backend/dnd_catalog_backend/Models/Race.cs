// Models/Race.cs
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace dnd_catalog_backend.Models
{
    public class Race : MongoBase
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        // Зв'язок з Ability: Раса має список ID Здібностей
        public List<string> AbilityIds { get; set; } = new List<string>();

        // Ця властивість буде використовуватися для ПОВНОГО відображення (JOIN) на фронтенді
        // [BsonIgnore] - означає, що це поле не буде зберігатися в базі даних MongoDB
        [BsonIgnore]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Ability>? Abilities { get; set; }
    }
}
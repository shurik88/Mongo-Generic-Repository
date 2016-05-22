using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Repository
{
    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class GuidEntity
    {
        [BsonId]
        public string Id { get; set; }

        public GuidEntity()
        {
            Id = Guid.NewGuid().ToString("N");
        }
    }
}

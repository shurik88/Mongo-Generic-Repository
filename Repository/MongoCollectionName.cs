using System;

namespace Repository
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class MongoCollectionName : Attribute
    {
        public MongoCollectionName(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}

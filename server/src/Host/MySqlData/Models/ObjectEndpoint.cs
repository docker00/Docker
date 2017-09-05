using System;

namespace MySql.AspNet.Identity
{
    public class ObjectEndpoint
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string ObjectId { get; set; }
        public string Description { get; set; }

        public ObjectEndpoint()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ObjectEndpoint(string value)
            : this()
        {
            Value = value;
        }
        
        public ObjectEndpoint(string value, string objectId)
            : this()
        {
            Value = value;
            ObjectId = objectId;
        }
    }
}

using System;

namespace MySql.AspNet.Identity
{
    public class ObjectEndpointPermition
    {
        public string Id { get; set; }
        public string ObjectEndpointId { get; set; }
        public string PermissionId { get; set; }

        public ObjectEndpointPermition()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}

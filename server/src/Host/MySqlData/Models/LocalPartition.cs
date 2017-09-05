using System;

namespace MySql.AspNet.Identity
{
    public class LocalPartition
    {
        public string Id { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }

        public LocalPartition()
        {
            Id = Guid.NewGuid().ToString();
        }

        public LocalPartition(string controllerName, string actionName)
            : this()
        {
            ControllerName = controllerName;
            ActionName = actionName;
        }
    }
}

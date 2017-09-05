using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.Extensions.GenerateMenu.Models
{
    public class MethodModel
    {
        public string Controller { get; set; }
        public string Method { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public bool ShowInMenu { get; set; }
        public int Weight { get; set; }
        public string Relation { get; set; }

        public MethodModel()
        {
            Controller = "";
            Method = "";
            Name = "";
            Description = "";
            GroupName = "";
            ShowInMenu = false;
            Weight = 0;
            Relation = "";
        }
    }
}

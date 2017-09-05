using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.Extensions.GenerateMenu.Models
{
    public class ControllerModel
    {
        public string Controller { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public bool ShowInMenu { get; set; }
        public int Weight { get; set; }
        public string Relation { get; set; }

        public List<MethodModel> Methods { get; set; }

        public ControllerModel()
        {
            Controller = "";
            Name = "";
            Description = "";
            GroupName = "";
            ShowInMenu = false;
            Weight = 0;
            Relation = "";
            Methods = new List<MethodModel>();
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class LocalPartitionViewModel : LocalPartitionInputModel
    {
        public LocalPartitionViewModel()
        {
        }

        public LocalPartitionViewModel(string id, string controllerName, string actionName)
        {
            Id = id;
            ControllerName = controllerName;
            ActionName = actionName;
        }
    }
}

using System.Collections.Generic;

namespace IdentityServer4.Quickstart.UI
{
    public class GroupViewModel : GroupInputModel
    {
        public List<GroupRoleViewModel> Roles { get; set; }
        public List<GroupViewModel> ChildrenGroups { get; set; }
        public List<UserViewModel> Users { get; set; }

        public GroupViewModel()
        {
            Roles = new List<GroupRoleViewModel>();
            ChildrenGroups = new List<GroupViewModel>();
            Users = new List<UserViewModel>();
        }

        public GroupViewModel(string id, string name)
        {
            Id = id;
            Name = name;
            Roles = new List<GroupRoleViewModel>();
            ChildrenGroups = new List<GroupViewModel>();
            Users = new List<UserViewModel>();
        }
    }
}

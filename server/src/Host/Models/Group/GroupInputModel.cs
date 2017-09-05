using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class GroupInputModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "{0} должно быть заполнено")]
        [Display(Name = "Название группы")]
        public string Name { get; set; }
        [Display(Name = "Группа-родитель")]
        public string ParentId { get; set; }

        public GroupInputModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        public GroupInputModel(string parentId)
        {
            ParentId = parentId;
        }

        public GroupInputModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}

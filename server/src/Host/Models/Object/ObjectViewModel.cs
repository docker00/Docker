using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ObjectViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Название")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Ссылка")]
        [DataType(DataType.Url)]
        [Required]
        public string Url { get; set; }

        public string ClientId { get; set; }
        public string RoleId { get; set; }

        public bool Enabled { get; set; }
    }
}

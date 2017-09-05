using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class AllowedCorsOriginViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        public AllowedCorsOriginViewModel()
        {
        }

        public AllowedCorsOriginViewModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}

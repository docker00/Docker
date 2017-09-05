using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class ProfileAttributesInputModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Обязательно при регистрации")]
        public bool RequiredRegister { get; set; }
        [Display(Name = "Обязательно при добавлении")]
        public bool RequiredAdditional { get; set; }
        [Display(Name = "Заблокировано")]
        public bool Disabled { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Только цифры")]
        [Display(Name = "Размер")]
        public int Weight { get; set; }
        [Display(Name = "Удалено")]
        public bool Deleted { get; set; }

        public string ClaimId { get; set; }
        public string ClaimType { get; set; }

        public ProfileAttributesInputModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ProfileAttributesInputModel(string name)
        {
            Name = name;
        }
    }
}

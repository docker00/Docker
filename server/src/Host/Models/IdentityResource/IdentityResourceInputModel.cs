using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class IdentityResourceInputModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "{0} должно быть заполнено")]
        [Display(Name = "Название ресурса")]
        public string Name { get; set; }
        [Display(Name = "Отображаемое название")]
        public string DisplayName { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Включен?")]
        public bool Enabled { get; set; } = true;
        [Display(Name = "Обязательный?")]
        public bool Required { get; set; }
        [Display(Name = "Подчеркивать?")]
        public bool Emphasize { get; set; }
        [Display(Name = "Отображать?")]
        public bool ShowInDiscoveryDocument { get; set; } = true;

        public IdentityResourceInputModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}

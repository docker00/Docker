using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ClientSecretInputModel
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string SecretId { get; set; }

        [Required(ErrorMessage = "{0} должно быть введено")]
        [Display(Name = "Значение ключа")]
        public string Value { get; set; }

        [Required(ErrorMessage = "{0} должен быть выбран")]
        [Display(Name ="Тип ключа")]
        public string Type { get; set; }

        public ClientSecretInputModel()
        {
            SecretId = Guid.NewGuid().ToString();
        }

        public ClientSecretInputModel(string clientId)
        {
            SecretId = Guid.NewGuid().ToString();
            ClientId = clientId;
        }

        public ClientSecretInputModel(string clientId, string secretId)
        {
            ClientId = clientId;
            SecretId = secretId;
        }
    }
}

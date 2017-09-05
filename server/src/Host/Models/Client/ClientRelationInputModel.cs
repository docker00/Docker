using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ClientRelationInputModel
    {
        [Required]
        public string FromClientId { get; set; }
        [Required]
        public string ToClientId { get; set; }

        public string ClientName { get; set; }

        public ClientRelationInputModel()
        {

        }

        public ClientRelationInputModel(string fromClientId)
        {
            FromClientId = fromClientId;
        }

        public ClientRelationInputModel(string fromClientId, string toClientId)
        {
            FromClientId = fromClientId;
            ToClientId = toClientId;
        }
    }
}

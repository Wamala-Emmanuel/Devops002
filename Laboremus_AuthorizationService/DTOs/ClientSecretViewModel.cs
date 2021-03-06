using IdentityServer4.EntityFramework.Entities;
using Newtonsoft.Json;

namespace Laboremus_AuthorizationService.DTOs
{
    public class ClientSecretViewModel : ClientSecret
    {
        [JsonIgnore] public Client Client { get; set; }
    }
}
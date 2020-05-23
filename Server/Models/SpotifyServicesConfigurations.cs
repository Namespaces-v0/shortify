namespace shortify.Server.Models
{
public class SpotifyServicesConfigurations
{
    public string ClientId { get; set; }

    public string ClientSecret { get; set; }
    public string RedirectUri { get; set; }
     public string Scopes { get; set; }
     public string AuthorizeAPIBaseURI { get; set;}
}
}
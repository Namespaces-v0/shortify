namespace shortify.Server.Models
{
public static class SpotifyServicesAPITokenCache
{
    public static string access_token { get { return "_access_token"; } }
    public static string token_type { get { return "_token_type"; } }
    public static string scope { get { return "_scope"; } }
     public static string expires_in { get { return "_expires_in"; } }
     public static string refresh_token { get { return "_refresh_token"; } }
}
}
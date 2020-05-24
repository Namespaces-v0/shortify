using System.Collections.Generic;
namespace shortify.Server.Models
{
public class SpotifyServicesAlbum
{
    public  string href { get; set; }
    public  IList<string> items { get; set; }
    public  string limit { get; set; }
     public  string next { get; set; }
     public  string offset { get; set;}
      public  string previous { get; set;}
       public  string total { get; set;}
}
}
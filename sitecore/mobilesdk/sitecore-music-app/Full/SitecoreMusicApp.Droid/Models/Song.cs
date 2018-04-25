namespace SitecoreMusicApp.Droid.Models
{
    public class Song
    {
        public  string Number  { get; set; }
        public  string SongName{ get; set; }
        public  string Duration{ get; set; }
        public  string SongFile { get; set; }
        public string SitecoreID { get; internal set; }
    }
}
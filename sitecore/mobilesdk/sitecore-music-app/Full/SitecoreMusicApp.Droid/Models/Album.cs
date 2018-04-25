using System.Collections.Generic;

namespace SitecoreMusicApp.Droid.Models
{
    public class Album
    {
        public Album()
        {
            Songs = new List<Song>();
        }

        public string SitecoreID { get; internal set; }
        public string Name { get; internal set; }
        public string Thumbnail { get; internal set; }
        public string Author { get; internal set; }
        public string Year { get; internal set; }
        public List<Song> Songs { get; set; }
        public int NumberOfSongs { get { return Songs.Count; } }

    }
}
using Android.Util;
using SitecoreMusicApp.Droid.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SitecoreMusicApp.Droid.Extensions;

namespace SitecoreMusicApp.Droid.Managers
{
    public class MusicStoreManager
    {
        private SitecoreManager _sitecoreManager;
        public MusicStoreManager()
        {
            // TODO: Implement IOC here
            _sitecoreManager = new SitecoreManager();
        }

        public async Task<List<Album>> GetAlbums()
        {

            List<Album> albums = new List<Album>();



            try
            {
                var sitecoreItems = await _sitecoreManager.GetItemByPath(
                                Constants.Sitecore.MusicStore.RepositoryPath,
                                Sitecore.MobileSDK.API.Request.Parameters.PayloadType.Full,
                                new List<Sitecore.MobileSDK.API.Request.Parameters.ScopeType> { Sitecore.MobileSDK.API.Request.Parameters.ScopeType.Children });


                if (sitecoreItems != null)
                    foreach (var item in sitecoreItems)
                    {

                        var album = new Album
                        {
                            SitecoreID = item.Id,
                            Name = item.GetValueFromField(Constants.Sitecore.Templates.Album.Fields.AlbumName),
                            Thumbnail = item.GetImageUrlFromMediaField(Constants.Sitecore.Templates.Album.Fields.Thumbnail, Constants.Sitecore.RestBaseUrl),
                            Author = item.GetValueFromField(Constants.Sitecore.Templates.Album.Fields.Author),
                            Year = item.GetValueFromField(Constants.Sitecore.Templates.Album.Fields.Year)
                        };

                        album.Songs = await GetSongs(album.SitecoreID);

                        albums.Add(album);

                    }


            }
            catch (Exception ex)
            {
                Log.Error("Error in GetAlbums(),  Error: {0}", ex.Message);
                throw ex;
            }

            return albums;
        }

        public async Task<List<Song>> GetSongs(string albumSitecoreID)
        {

            List<Song> songs = new List<Song>();

            try
            {
                var sitecoreItems = await _sitecoreManager.GetItemById(albumSitecoreID,
                                Sitecore.MobileSDK.API.Request.Parameters.PayloadType.Full,
                                new List<Sitecore.MobileSDK.API.Request.Parameters.ScopeType> { Sitecore.MobileSDK.API.Request.Parameters.ScopeType.Children });

                if (sitecoreItems != null)
                    foreach (var item in sitecoreItems)
                    {
                        var song = new Song
                        {
                            SitecoreID = item.Id,
                            SongName = item.GetValueFromField(Constants.Sitecore.Templates.Song.Fields.SongName),
                            Duration = item.GetValueFromField(Constants.Sitecore.Templates.Song.Fields.Duration),
                            Number = item.GetValueFromField(Constants.Sitecore.Templates.Song.Fields.Number),
                            SongFile = item.GetValueFromField(Constants.Sitecore.Templates.Song.Fields.SongFile)
                        };
                        songs.Add(song);
                    }

            }
            catch (Exception ex)
            {
                Log.Error("Error in GetSongs(),  Error: {0}", ex.Message);
                throw ex;
            }

            return songs;
        }

    }
}
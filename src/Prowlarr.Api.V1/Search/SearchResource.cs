using System;
using System.Collections.Generic;
using System.Linq;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Parser.Model;
using Prowlarr.Http.REST;

namespace Prowlarr.Api.V1.Search
{
    public class SearchResource : RestResource
    {
        public string Guid { get; set; }
        public int Age { get; set; }
        public double AgeHours { get; set; }
        public double AgeMinutes { get; set; }
        public long Size { get; set; }
        public int? Files { get; set; }
        public int? Grabs { get; set; }
        public int IndexerId { get; set; }
        public string Indexer { get; set; }
        public string SubGroup { get; set; }
        public string ReleaseHash { get; set; }
        public string Title { get; set; }
        public bool Approved { get; set; }
        public int ImdbId { get; set; }
        public DateTime PublishDate { get; set; }
        public string CommentUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string InfoUrl { get; set; }
        public IEnumerable<string> IndexerFlags { get; set; }
        public ICollection<IndexerCategory> Categories { get; set; }

        public string MagnetUrl { get; set; }
        public string InfoHash { get; set; }
        public int? Seeders { get; set; }
        public int? Leechers { get; set; }
        public DownloadProtocol Protocol { get; set; }
    }

    public static class ReleaseResourceMapper
    {
        public static SearchResource ToResource(this ReleaseInfo model)
        {
            var releaseInfo = model;
            var torrentInfo = (model as TorrentInfo) ?? new TorrentInfo();
            var indexerFlags = torrentInfo.IndexerFlags.ToString().Split(new string[] { ", " }, StringSplitOptions.None).Where(x => x != "0");

            // TODO: Clean this mess up. don't mix data from multiple classes, use sub-resources instead? (Got a huge Deja Vu, didn't we talk about this already once?)
            return new SearchResource
            {
                Guid = releaseInfo.Guid,

                //QualityWeight
                Age = releaseInfo.Age,
                AgeHours = releaseInfo.AgeHours,
                AgeMinutes = releaseInfo.AgeMinutes,
                Size = releaseInfo.Size ?? 0,
                Files = releaseInfo.Files,
                Grabs = releaseInfo.Grabs,
                IndexerId = releaseInfo.IndexerId,
                Indexer = releaseInfo.Indexer,
                Title = releaseInfo.Title,
                ImdbId = releaseInfo.ImdbId,
                PublishDate = releaseInfo.PublishDate,
                CommentUrl = releaseInfo.CommentUrl,
                DownloadUrl = releaseInfo.DownloadUrl,
                InfoUrl = releaseInfo.InfoUrl,
                Categories = releaseInfo.Category,

                //ReleaseWeight
                MagnetUrl = torrentInfo.MagnetUrl,
                InfoHash = torrentInfo.InfoHash,
                Seeders = torrentInfo.Seeders,
                Leechers = (torrentInfo.Peers.HasValue && torrentInfo.Seeders.HasValue) ? (torrentInfo.Peers.Value - torrentInfo.Seeders.Value) : (int?)null,
                Protocol = releaseInfo.DownloadProtocol,
                IndexerFlags = indexerFlags
            };
        }

        public static ReleaseInfo ToModel(this SearchResource resource)
        {
            ReleaseInfo model;

            if (resource.Protocol == DownloadProtocol.Torrent)
            {
                model = new TorrentInfo
                {
                    MagnetUrl = resource.MagnetUrl,
                    InfoHash = resource.InfoHash,
                    Seeders = resource.Seeders,
                    Peers = (resource.Seeders.HasValue && resource.Leechers.HasValue) ? (resource.Seeders + resource.Leechers) : null
                };
            }
            else
            {
                model = new ReleaseInfo();
            }

            model.Guid = resource.Guid;
            model.Title = resource.Title;
            model.Size = resource.Size;
            model.DownloadUrl = resource.DownloadUrl;
            model.InfoUrl = resource.InfoUrl;
            model.CommentUrl = resource.CommentUrl;
            model.IndexerId = resource.IndexerId;
            model.Indexer = resource.Indexer;
            model.DownloadProtocol = resource.Protocol;
            model.ImdbId = resource.ImdbId;
            model.PublishDate = resource.PublishDate.ToUniversalTime();

            return model;
        }
    }
}

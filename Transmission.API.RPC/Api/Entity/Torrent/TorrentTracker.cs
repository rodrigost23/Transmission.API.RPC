using Newtonsoft.Json;
using Transmission.API.RPC.Core.Entity;

namespace Transmission.API.RPC.Api.Entity.Torrent;

public class TorrentTracker : ITransmissionTorrentTracker
{
    [JsonConstructor]
    internal TorrentTracker(string announce, int id, string scrape, int tier)
    {
        Announce = announce;
        Id = id;
        Scrape = scrape;
        Tier = tier;
    }

    public string Announce { get; set; }
    public int Id { get; set; }
    public string Scrape { get; set; }
    public int Tier { get; set; }
}

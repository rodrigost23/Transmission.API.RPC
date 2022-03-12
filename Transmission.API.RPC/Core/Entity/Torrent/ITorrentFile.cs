using Newtonsoft.Json;

namespace Transmission.API.RPC.Core.Entity.Torrent;

public interface ITorrentFile
{
    /// <summary>
    /// The current size of the file, i.e. how much we've downloaded
    /// </summary>
    [JsonProperty("bytesCompleted")]
    long? BytesCompleted { get; set; }

    /// <summary>
    /// The total size of the file
    /// </summary>
    [JsonProperty("length")]
    long? Length { get; set; }

    /// <summary>
    /// This file's name. Includes the full subpath in the torrent.
    /// </summary>
    [JsonProperty("name")]
    string? Name { get; set; }
}

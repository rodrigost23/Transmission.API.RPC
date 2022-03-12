using Transmission.API.RPC.Core.Entity.Torrent;

namespace Transmission.API.RPC.Api.Entity.Torrent;

public class TorrentFile : ITorrentFile
{
    public long? BytesCompleted { get; set; }
    public long? Length { get; set; }
    public string? Name { get; set; }
}

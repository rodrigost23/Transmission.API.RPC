using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Transmission.API.RPC.Api.Entity;
using Transmission.API.RPC.Core.Entity.Torrent;
using Transmission.API.RPC.Core.Enums;
using static Transmission.API.RPC.Api.TorrentFields;

namespace Transmission.API.RPC.Core.Entity;

public interface ITorrentView : ITorrentData
{
    /// <summary>
    /// The torrent's unique Id.
    /// </summary>
    [JsonProperty(ID)]
    int Id { get; set; }

    /// <summary>
    /// The last time we uploaded or downloaded piece data on this torrent.
    /// </summary>
    [JsonProperty(ACTIVITY_DATE), JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime ActivityDate { get; set; }

    /// <summary>
    /// When the torrent was first added.
    /// </summary>
    [JsonProperty(ADDED_DATE), JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime AddedDate { get; set; }

    [JsonProperty(COMMENT)]
    public string Comment { get; set; }

    /// <summary>
    /// Byte count of all the corrupt data you've ever downloaded for
    /// this torrent.If you're on a poisoned torrent, this number can
    /// grow very large.
    /// </summary>
    [JsonProperty(CORRUPT_EVER)]
    public int CorruptEver { get; set; }

    [JsonProperty(CREATOR)]
    public string Creator { get; set; }

    [JsonProperty(DATE_CREATED), JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Byte count of all the piece data we want and don't have yet,
    /// but that a connected peer does have. [0...<see cref="LeftUntilDone"/>]
    /// </summary>
    [JsonProperty(DESIRED_AVAILABLE)]
    public long DesiredAvailable { get; set; }

    /// <summary>
    /// When the torrent finished downloading.
    /// </summary>
    [JsonProperty(DONE_DATE), JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime DoneDate { get; set; }

    [JsonProperty(DOWNLOAD_DIR)]
    public string DownloadDir { get; set; }

    /// <summary>
    /// Byte count of all the non-corrupt data you've ever downloaded
    /// for this torrent. If you delete the files and download them a
    /// second time, this will be 2 * <see cref="TotalSize"/>..
    /// </summary>
    [JsonProperty(DOWNLOADED_EVER)]
    public long DownloadedEver { get; set; }

    /// <summary>
    /// The last time during this session that a rarely-changing field
    /// changed -- e.g.any tr_torrent_metainfo field(trackers, filenames, name)
    /// or download directory.RPC clients can monitor this to know when
    /// to reload fields that rarely change.
    /// </summary>
    [JsonProperty(EDIT_DATE), JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime EditDate { get; set; }

    /// <summary>
    /// Defines what kind of text is in errorString.
    /// 
    /// See <seealso cref="ErrorString"/>
    /// </summary>
    [JsonProperty(ERROR)]
    public TorrentError Error { get; set; }

    /// <summary>
    /// A warning or error message regarding the torrent.
    /// 
    /// <seealso cref="Error"/>
    /// </summary>
    [JsonProperty(ERROR_STRING)]
    public string ErrorString { get; set; }

    /// <summary>
    /// If downloading, estimated number of seconds left until the torrent is done.
    /// If seeding, estimated number of seconds left until seed ratio is reached.
    /// </summary>
    [JsonProperty(ETA)]
    public int Eta { get; set; }

    /// <summary>
    /// If seeding, number of seconds left until the idle time limit is reached.
    /// </summary>
    [JsonProperty(ETA_IDLE)]
    public int EtaIdle { get; set; }

    [JsonProperty(FILE_COUNT)]
    public int FileCount { get; set; }

    [JsonProperty(FILES)]
    public ITorrentFile[] Files { get; set; }

    [JsonProperty(FILE_STATS)]
    public ITorrentFileStats[] FileStats { get; set; }

    [JsonProperty(HASH_STRING)]
    public string HashString { get; set; }

    /// <summary>
    /// Byte count of all the partial piece data we have for this torrent.
    /// 
    /// As pieces become complete, this value may decrease as portions of it
    /// are moved to <see cref="CorruptEver"/> or <see cref="HaveValid"/>.
    /// </summary>
    [JsonProperty(HAVE_UNCHECKED)]
    public int HaveUnchecked { get; set; }

    /// <summary>
    /// Byte count of all the checksum-verified data we have for this torrent.
    /// </summary>
    [JsonProperty(HAVE_VALID)]
    public long HaveValid { get; set; }

    /// <summary>
    /// A torrent is considered finished if it has met its seed ratio.
    /// As a result, only paused torrents can be finished.
    /// </summary>
    [JsonProperty(IS_FINISHED)]
    public bool IsFinished { get; set; }

    [JsonProperty(IS_PRIVATE)]
    public bool IsPrivate { get; set; }

    /// <summary>
    /// <see langword="true"/> if the torrent is running, but has been idle for long enough
    /// to be considered stalled.
    /// 
    /// See <seealso cref="SessionInfo.QueueStalledMinutes"/>
    /// </summary>
    [JsonProperty(IS_STALLED)]
    public bool IsStalled { get; set; }

    /// <summary>
    /// Byte count of how much data is left to be downloaded until we've got
    /// all the pieces that we want. [0..<see cref="SizeWhenDone"/>]
    /// </summary>
    [JsonProperty(LEFT_UNTIL_DONE)]
    public long LeftUntilDone { get; set; }

    [JsonProperty(MAGNET_LINK)]
    public string MagnetLink { get; set; }

    /// <summary>
    /// Time when one or more of the torrent's trackers will
    /// allow you to manually ask for more peers, or 0 if you
    /// can't
    /// </summary>
    [JsonProperty(MANUAL_ANNOUNCE_TIME)]
    public int ManualAnnounceTime { get; set; }

    [JsonProperty(MAX_CONNECTED_PEERS)]
    public int MaxConnectedPeers { get; set; }

    /// <summary>
    /// How much of the metadata the torrent has.
    /// For torrents added from a.torrent this will always be 1.
    /// For magnet links, this number will from from 0 to 1 as the metadata is downloaded.
    /// Range is [0..1] 
    /// </summary>
    [JsonProperty(METADATA_PERCENT_COMPLETE)]
    public double MetadataPercentComplete { get; set; }

    [JsonProperty(NAME)]
    public string Name { get; set; }

    [JsonProperty(PEERS)]
    public ITorrentPeers[] Peers { get; set; }

    /// <summary>
    /// Number of peers that we're connected to
    /// </summary>
    [JsonProperty(PEERS_CONNECTED)]
    public int PeersConnected { get; set; }

    /// <summary>
    /// How many peers we found out about from the tracker, or from pex,
    /// or from incoming connections, or from our resume file.
    /// </summary>
    [JsonProperty(PEERS_FROM)]
    public ITorrentPeersFrom PeersFrom { get; set; }

    /// <summary>
    /// Number of peers that we're sending data to.
    /// </summary>
    [JsonProperty(PEERS_GETTING_FROM_US)]
    public int PeersGettingFromUs { get; set; }

    /// <summary>
    /// Number of peers that are sending data to us.
    /// </summary>
    [JsonProperty(PEERS_SENDING_TO_US)]
    public int PeersSendingToUs { get; set; }

    /// <summary>
    /// How much has been downloaded of the entire torrent.
    /// Range is [0..1]
    /// </summary>
    [JsonProperty(PERCENT_COMPLETE)]
    public double PercentComplete { get; set; }

    /// <summary>
    /// How much has been downloaded of the files the user wants. This differs
    /// from percentComplete if the user wants only some of the torrent's files.
    /// Range is [0..1]
    /// 
    /// See <seealso cref="LeftUntilDone"/>
    /// </summary>
    [JsonProperty(PERCENT_DONE)]
    public double PercentDone { get; set; }

    [JsonProperty(PIECES)]
    public string Pieces { get; set; }

    [JsonProperty(PIECE_COUNT)]
    public int PieceCount { get; set; }

    [JsonProperty(PIECE_SIZE)]
    public long PieceSize { get; set; }

    /// <summary>
    /// Array of <see langword="Priority"/>, with each one corresponding to a file.
    /// </summary>
    [JsonProperty(PRIORITIES)]
    public Priority[] Priorities { get; set; }

    /// <summary>
    /// Return the mime-type (e.g. "audio/x-flac") that matches more of the
    /// torrent's content than any other mime-type. 
    /// </summary>
    [JsonProperty(PRIMARY_MIME_TYPE)]
    public string PrimaryMimeType { get; set; }

    /// <summary>
    /// Download speed (B/s)
    /// </summary>
    [JsonProperty(RATE_DOWNLOAD)]
    public int RateDownload { get; set; }

    /// <summary>
    /// Upload speed (B/s)
    /// </summary>
    [JsonProperty(RATE_UPLOAD)]
    public int RateUpload { get; set; }

    /// <summary>
    /// When <see cref="Status"/> is <see cref="TorrentStatus.Verifying"/> or <see cref="TorrentStatus.VerifyQueue"/>,
    /// this is the percentage of how much of the files has been
    /// verified. When it gets to 1, the verify process is done.
    /// Range is [0..1]
    /// 
    /// See <seealso cref="Status"/>
    /// </summary>
    [JsonProperty(RECHECK_PROGRESS)]
    public double RecheckProgress { get; set; }

    /// <summary>
    /// Cumulative seconds the torrent's ever spent downloading 
    /// </summary>
    [JsonProperty(SECONDS_DOWNLOADING)]
    public int SecondsDownloading { get; set; }

    /// <summary>
    /// Cumulative seconds the torrent's ever spent seeding 
    /// </summary>
    [JsonProperty(SECONDS_SEEDING)]
    public int SecondsSeeding { get; set; }

    /// <summary>
    /// Byte count of all the piece data we'll have downloaded when we're done,
    /// whether or not we have it yet. This may be less than <see cref="TotalSize"/>
    /// if only some of the torrent's files are wanted.
    /// [0...<see cref="TotalSize"/>]
    /// </summary>
    [JsonProperty(SIZE_WHEN_DONE)]
    public long SizeWhenDone { get; set; }

    /// <summary>
    /// When the torrent was last started.
    /// </summary>
    [JsonProperty(START_DATE), JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// What is this torrent doing right now?
    /// </summary>
    [JsonProperty(STATUS)]
    public TorrentStatus Status { get; set; }

    [JsonProperty(TRACKERS)]
    public ITransmissionTorrentTracker[] Trackers { get; set; }

    [JsonProperty(TRACKER_STATS)]
    public ITorrentTrackerStats[] TrackerStats { get; set; }

    /// <summary>
    /// Total size of the torrent, including unwanted files.
    /// </summary>
    [JsonProperty(TOTAL_SIZE)]
    public long TotalSize { get; set; }

    /// <summary>
    /// Path to the torrent file in the server
    /// </summary>
    [JsonProperty(TORRENT_FILE)]
    public string TorrentFile { get; set; }

    /// <summary>
    /// Byte count of all data you've ever uploaded for this torrent.
    /// </summary>
    [JsonProperty(UPLOADED_EVER)]
    public long UploadedEver { get; set; }

    /// <summary>
    /// Total uploaded bytes / total torrent size.
    /// </summary>
    [JsonProperty(UPLOAD_RATIO)]
    public double UploadRatio { get; set; }

    /// <summary>
    /// An array of booleans, with each item corresponding to a file
    /// </summary>
    [JsonProperty(WANTED)]
    public bool[] Wanted { get; set; }

    [JsonProperty(WEBSEEDS)]
    public string[] Webseeds { get; set; }

    /// <summary>
    /// Number of webseeds that are sending data to us.
    /// </summary>
    [JsonProperty(WEBSEEDS_SENDING_TO_US)]
    public int WebseedsSendingToUs { get; set; }
}

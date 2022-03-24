using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using Transmission.Net.Api;
using Transmission.Net.Api.Entity;
using Transmission.Net.Api.Entity.Session;
using Transmission.Net.Arguments;
using Transmission.Net.Core;
using Transmission.Net.Exception;

namespace Transmission.Net;

/// <summary>
/// Transmission client
/// </summary>
public class TransmissionClient : ITransmissionClient
{
    private readonly string? _authorization;
    private readonly bool _needAuthorization;

    /// <summary>
    /// Url to service
    /// </summary>
    public string Url
    {
        get;
        private set;
    }

    /// <summary>
    /// Session ID
    /// </summary>
    public string? SessionID { get; private set; }

    /// <summary>
    /// Current Tag
    /// </summary>
    public int CurrentTag
    { get; private set; }

    /// <summary>
    /// Initialize client
    /// <example>For example
    /// <code>
    /// new Transmission.Net.Client("https://website.com:9091/transmission/rpc")
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="url">URL to Transmission RPC API. Often it looks like schema://host:port/transmission/rpc </param>
    /// <param name="sessionID">Session ID</param>
    /// <param name="login">Login</param>
    /// <param name="password">Password</param>
    public TransmissionClient(string url, string? sessionID = null, string? login = null, string? password = null)
    {
        Url = url;
        SessionID = sessionID;

        if (!string.IsNullOrWhiteSpace(login))
        {
            var authBytes = Encoding.UTF8.GetBytes(login + ":" + password);
            var encoded = Convert.ToBase64String(authBytes);

            _authorization = "Basic " + encoded;
            _needAuthorization = true;
        }
    }

    #region Session methods

    /// <summary>
    /// Close current session (API: session-close)
    /// </summary>
    public async Task CloseSessionAsync()
    {
        var request = new TransmissionRequest("session-close");
        _ = await SendRequestAsync(request);
    }

    /// <summary>
    /// Set information to current session (API: session-set)
    /// </summary>
    /// <param name="settings">New session settings</param>
    public async Task SetSessionSettingsAsync(SessionSettings settings)
    {
        var request = new TransmissionRequest("session-set", settings);
        _ = await SendRequestAsync(request);
    }

    /// <summary>
    /// Get session stat
    /// </summary>
    /// <returns>Session stat</returns>
    public async Task<Stats?> GetSessionStatisticAsync()
    {
        var request = new TransmissionRequest("session-stats");
        var response = await SendRequestAsync(request);
        var result = response.Deserialize<Stats>();
        return result;
    }

    /// <summary>
    /// Get information of current session (API: session-get)
    /// </summary>
    /// <returns>Session information</returns>
    //TODO: support optional "fields" argument
    public async Task<SessionInfo?> GetSessionInformationAsync()
    {
        var request = new TransmissionRequest("session-get");
        var response = await SendRequestAsync(request);
        var result = response.Deserialize<SessionInfo>();
        return result;
    }

    #endregion

    #region Torrents methods

    /// <summary>
    /// Add torrent (API: torrent-add)
    /// </summary>
    /// <returns>Torrent info (ID, Name and HashString)</returns>
    public async Task<NewTorrentInfo?> TorrentAddAsync(NewTorrent torrent)
    {
        if (string.IsNullOrWhiteSpace(torrent.Metainfo) && string.IsNullOrWhiteSpace(torrent.Filename))
        {
            throw new ArgumentException("Either \"filename\" or \"metainfo\" must be included.");
        }

        var request = new TransmissionRequest("torrent-add", torrent);
        var response = await SendRequestAsync(request);
        var jObject = response.Deserialize<JObject>();

        if (jObject == null || jObject.First == null)
        {
            return null;
        }

        NewTorrentInfo? result = null;

        if (jObject.TryGetValue("torrent-duplicate", out var value) || jObject.TryGetValue("torrent-added", out value))
        {
            result = JsonConvert.DeserializeObject<NewTorrentInfo>(value.ToString());
        }

        return result;
    }

    /// <summary>
    /// Set torrent params (API: torrent-set)
    /// </summary>
    /// <param name="settings">Torrent settings</param>
    public async Task TorrentSetAsync(TorrentSettings settings)
    {
        var request = new TransmissionRequest("torrent-set", settings);
        _ = await SendRequestAsync(request);
    }

    /// <inheritdoc/>
    public async Task<TorrentsResult?> TorrentGetAsync(int[]? ids = null, params string[] fields)
    {
        var arguments = new Dictionary<string, object>
        {
            { "fields", fields.Any() ? fields : TorrentFields.ALL_FIELDS }
        };

        if (ids != null && ids.Any())
        {
            arguments.Add("ids", ids);
        }

        var request = new TransmissionRequest("torrent-get", arguments);

        var response = await SendRequestAsync(request);
        var result = response.Deserialize<TorrentsResult>();

        return result;
    }

    /// <summary>
    /// Gets <paramref name="fields"/> from all torrents
    /// </summary>
    /// <param name="fields">Fields of torrent (empty for <see cref="TorrentFields.ALL_FIELDS"/>)</param>
    /// <returns></returns>
    public async Task<TorrentsResult?> TorrentGetAsync(params string[] fields)
    {
        return await TorrentGetAsync(null, fields);
    }


    /// <summary>
    /// Gets <paramref name="fields"/> of one specific torrent by its <paramref name="id"/>.
    /// </summary>
    /// <param name="id">ID of torrent</param>
    /// <param name="fields">Fields of torrent (empty for <see cref="TorrentFields.ALL_FIELDS"/>)</param>
    /// <returns>Torrents info</returns>
    public async Task<TorrentsResult?> TorrentGetAsync(int id, params string[] fields)
    {
        return await TorrentGetAsync(new int[] { id }, fields);
    }

    /// <summary>
    /// Remove torrents
    /// </summary>
    /// <param name="ids">Torrents id</param>
    /// <param name="deleteData">Remove data</param>
    public async Task TorrentRemoveAsync(int[] ids, bool deleteData = false)
    {
        var arguments = new Dictionary<string, object>
        {
            { "ids", ids },
            { "delete-local-data", deleteData }
        };

        var request = new TransmissionRequest("torrent-remove", arguments);
        _ = await SendRequestAsync(request);
    }

    #region Torrent Start

    /// <summary>
    /// Start torrents (API: torrent-start)
    /// </summary>
    /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
    public async Task TorrentStartAsync(params object[] ids)
    {
        var request = new TransmissionRequest("torrent-start", new Dictionary<string, object> { { "ids", ids } });
        _ = await SendRequestAsync(request);
    }

    /// <summary>
    /// Start torrents (API: torrent-start)
    /// </summary>
    /// <param name="hashes">A list of torrent id numbers, sha1 hash strings, or both</param>
    public async Task TorrentStartAsync(params string[] hashes)
    {
        var request = new TransmissionRequest("torrent-start", new Dictionary<string, object> { { "ids", hashes } });
        _ = await SendRequestAsync(request);
    }

    /// <summary>
    /// Start recently active torrents (API: torrent-start)
    /// </summary>
    public async Task TorrentStartAsync()
    {
        var request = new TransmissionRequest("torrent-start", new Dictionary<string, object> { { "ids", "recently-active" } });
        _ = await SendRequestAsync(request);
    }

    #endregion

    #region Torrent Start Now

    /// <summary>
    /// Start now torrents (API: torrent-start-now)
    /// </summary>
    /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
    public async Task TorrentStartNowAsync(object[] ids)
    {
        var request = new TransmissionRequest("torrent-start-now", new Dictionary<string, object> { { "ids", ids } });
        _ = await SendRequestAsync(request);
    }

    /// <summary>
    /// Start now recently active torrents (API: torrent-start-now)
    /// </summary>
    public async Task TorrentStartNowAsync()
    {
        var request = new TransmissionRequest("torrent-start-now", new Dictionary<string, object> { { "ids", "recently-active" } });
        _ = await SendRequestAsync(request);
    }

    #endregion

    #region Torrent Stop

    /// <summary>
    /// Stop torrents (API: torrent-stop)
    /// </summary>
    /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
    public async Task TorrentStopAsync(object[] ids)
    {
        var request = new TransmissionRequest("torrent-stop", new Dictionary<string, object> { { "ids", ids } });
        _ = await SendRequestAsync(request);
    }

    /// <summary>
    /// Stop recently active torrents (API: torrent-stop)
    /// </summary>
    public async Task TorrentStopAsync()
    {
        var request = new TransmissionRequest("torrent-stop", new Dictionary<string, object> { { "ids", "recently-active" } });
        _ = await SendRequestAsync(request);
    }

    #endregion

    #region Torrent Verify

    /// <summary>
    /// Verify torrents (API: torrent-verify)
    /// </summary>
    /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
    public async Task TorrentVerifyAsync(object[] ids)
    {
        var request = new TransmissionRequest("torrent-verify", new Dictionary<string, object> { { "ids", ids } });
        _ = await SendRequestAsync(request);
    }

    /// <summary>
    /// Verify recently active torrents (API: torrent-verify)
    /// </summary>
    public async Task TorrentVerifyAsync()
    {
        var request = new TransmissionRequest("torrent-verify", new Dictionary<string, object> { { "ids", "recently-active" } });
        _ = await SendRequestAsync(request);
    }
    #endregion

    /// <summary>
    /// Move torrents in queue on top (API: queue-move-top)
    /// </summary>
    /// <param name="ids">Torrents id</param>
    public async Task TorrentQueueMoveTopAsync(int[] ids)
    {
        var request = new TransmissionRequest("queue-move-top", new Dictionary<string, object> { { "ids", ids } });
        _ = await SendRequestAsync(request);
    }

    /// <summary>
    /// Move up torrents in queue (API: queue-move-up)
    /// </summary>
    /// <param name="ids"></param>
    public async Task TorrentQueueMoveUpAsync(int[] ids)
    {
        _ = await SendRequestAsync(new("queue-move-up", new Dictionary<string, object> { { "ids", ids } }));
    }

    /// <summary>
    /// Move down torrents in queue (API: queue-move-down)
    /// </summary>
    /// <param name="ids"></param>
    public async Task TorrentQueueMoveDownAsync(int[] ids)
    {
        var request = new TransmissionRequest("queue-move-down", new Dictionary<string, object> { { "ids", ids } });
        _ = await SendRequestAsync(request);
    }

    /// <summary>
    /// Move torrents to bottom in queue  (API: queue-move-bottom)
    /// </summary>
    /// <param name="ids"></param>
    public async Task TorrentQueueMoveBottomAsync(int[] ids)
    {
        var request = new TransmissionRequest("queue-move-bottom", new Dictionary<string, object> { { "ids", ids } });
        _ = await SendRequestAsync(request);
    }

    /// <summary>
    /// Set new location for torrents files (API: torrent-set-location)
    /// </summary>
    /// <param name="ids">Torrent ids</param>
    /// <param name="location">The new torrent location</param>
    /// <param name="move">Move from previous location</param>
    public async Task TorrentSetLocationAsync(int[] ids, string location, bool move)
    {
        var arguments = new Dictionary<string, object>
        {
            { "ids", ids },
            { "location", location },
            { "move", move }
        };

        var request = new TransmissionRequest("torrent-set-location", arguments);
        _ = await SendRequestAsync(request);
    }

    /// <summary>
    /// Rename a file or directory in a torrent (API: torrent-rename-path)
    /// </summary>
    /// <param name="id">The torrent whose path will be renamed</param>
    /// <param name="path">The path to the file or folder that will be renamed</param>
    /// <param name="name">The file or folder's new name</param>
    public async Task<RenameTorrentInfo?> TorrentRenamePathAsync(int id, string path, string name)
    {
        var arguments = new Dictionary<string, object>
        {
            { "ids", new int[] { id } },
            { "path", path },
            { "name", name }
        };

        var request = new TransmissionRequest("torrent-rename-path", arguments);
        var response = await SendRequestAsync(request);

        var result = response.Deserialize<RenameTorrentInfo>();

        return result;
    }

    //method name not recognized
    ///// <summary>
    ///// Reannounce torrent (API: torrent-reannounce)
    ///// </summary>
    ///// <param name="ids"></param>
    //public void ReannounceTorrents(object[] ids)
    //{
    //    var arguments = new Dictionary<string, object>();
    //    arguments.Add("ids", ids);

    //    var request = new TransmissionRequest("torrent-reannounce", arguments);
    //    var response = SendRequest(request);
    //}

    #endregion

    #region System

    /// <summary>
    /// See if your incoming peer port is accessible from the outside world (API: port-test)
    /// </summary>
    /// <returns>Accessible state</returns>
    public async Task<bool?> PortTestAsync()
    {
        var request = new TransmissionRequest("port-test");
        var response = await SendRequestAsync(request);

        var data = response.Deserialize<JObject>();
        var result = (bool?)data?.GetValue("port-is-open");
        return result;
    }

    /// <summary>
    /// Update blocklist (API: blocklist-update)
    /// </summary>
    /// <returns>Blocklist size</returns>
    public async Task<int?> BlocklistUpdateAsync()
    {
        var request = new TransmissionRequest("blocklist-update");
        var response = await SendRequestAsync(request);

        var data = response.Deserialize<JObject>();
        var result = (int?)data?.GetValue("blocklist-size");
        return result;
    }

    /// <summary>
    /// Get free space is available in a client-specified folder.
    /// </summary>
    /// <param name="path">The directory to query</param>
    public async Task<long?> FreeSpaceAsync(string path)
    {
        var arguments = new Dictionary<string, object>
        {
            { "path", path }
        };

        var request = new TransmissionRequest("free-space", arguments);
        var response = await SendRequestAsync(request);

        var data = response.Deserialize<JObject>();
        var result = (long?)data?.GetValue("size-bytes");
        return result;
    }

    #endregion

    private async Task<TransmissionResponse> SendRequestAsync(TransmissionRequest request)
    {
        TransmissionResponse? result;

        request.Tag = ++CurrentTag;

        //Prepare http web request
        HttpClient httpClient = new();

        HttpRequestMessage httpRequest = new(HttpMethod.Post, Url);
        httpRequest.Headers.Add("X-Transmission-Session-Id", SessionID);

        if (_needAuthorization)
        {
            httpRequest.Headers.Add("Authorization", _authorization);
        }

        httpRequest.Content = new StringContent(request.ToJson() ?? "", Encoding.UTF8, "application/json-rpc");

        //Send request and prepare response
        using (var httpResponse = await httpClient.SendAsync(httpRequest))
        {
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseString = await httpResponse.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<TransmissionResponse>(responseString);

                if (result?.Result != "success")
                {
                    throw new TransmissionException(result?.Result ?? "Invalid response");
                }
            }
            else if (httpResponse.StatusCode == HttpStatusCode.Conflict)
            {
                if (httpResponse.Headers.Any())
                {
                    //If session id expired, try get session id and send request
                    if (httpResponse.Headers.TryGetValues("X-Transmission-Session-Id", out var values))
                    {
                        SessionID = values.First();
                    }
                    else
                    {
                        throw new TransmissionException("Session ID Error");
                    }

                    result = await SendRequestAsync(request);
                }
                else
                {
                    throw new TransmissionException("Session ID Error");
                }
            }
            else
            {
                throw new TransmissionException(
                    $"HTTP Error: {httpResponse.ReasonPhrase}",
                    new HttpRequestException(httpResponse.ReasonPhrase, null, httpResponse.StatusCode)
                );
            }
        }

        return result;
    }
}

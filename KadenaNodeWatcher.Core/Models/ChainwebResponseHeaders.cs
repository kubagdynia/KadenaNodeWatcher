namespace KadenaNodeWatcher.Core.Models;

internal class ChainwebResponseHeaders
{
    /// <summary>
    /// Host and port of the client as observed by the remote node, e.g. "10.36.1.3:42988"
    /// </summary>
    public string PeerAddr { get; set; }

    /// <summary>
    /// The time of the clock of the remote node (seconds since POSIX epoch), e.g. 1618597601
    /// </summary>
    public int ServerTimestamp { get; set; }

    /// <summary>
    /// The version of the remote chainweb node, e.g. "2.6"
    /// </summary>
    public string ChainwebNodeVersion { get; set; }
}
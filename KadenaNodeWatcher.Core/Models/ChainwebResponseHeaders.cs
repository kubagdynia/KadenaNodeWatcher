namespace KadenaNodeWatcher.Core.Models;

public class ChainwebResponseHeaders
{
    /// <summary>
    /// Host and port of the client as observed by the remote node
    /// </summary>
    public string PeerAddr { get; set; }
        
    /// <summary>
    /// The time of the clock of the remote node (seconds since POSIX epoch)
    /// </summary>
    public int ServerTimestamp { get; set; }
        
    /// <summary>
    /// The version of the remote chainweb node
    /// </summary>
    public string ChainwebNodeVersion { get; set; }
}
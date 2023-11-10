namespace KadenaNodeWatcher.Core.Models;

public class GetCutResponse
{
    /// <summary>
    /// Cut datastruction of the chainweb API.
    /// </summary>
    public Cut Cut { get; set; }

    public ChainwebResponseHeaders ResponseHeaders { get; set; }
}
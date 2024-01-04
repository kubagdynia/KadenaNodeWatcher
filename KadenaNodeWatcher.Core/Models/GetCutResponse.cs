namespace KadenaNodeWatcher.Core.Models;

internal class GetCutResponse
{
    /// <summary>
    /// Cut datastruction of the chainweb API.
    /// </summary>
    public Cut Cut { get; set; }

    public ChainwebResponseHeaders ResponseHeaders { get; set; }
}
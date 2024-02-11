using CommandLine;

namespace KadenaNodeWatcher.Collector;

public class RunningOptions
{
    [Option(shortName: 'h', longName: "hostname", Required = false, HelpText = "Host name")]
    public string? HostName { get; set; }
    
    [Option(shortName: 'c', longName: "CheckChildNodes", Default  = false, Required = false, HelpText = "Check child nodes")]
    public bool CheckChildNodes { get; set; }
    
    [Option(shortName: 'g', longName: "CheckIpGeolocation", Default  = false, Required = false, HelpText = "Check IP Geolocation")]
    public bool CheckIpGeolocation { get; set; }
    
    [Option(shortName: 'a', longName: "CollectNodeDataAutomatically", Default  = false, Required = false, HelpText = "Collect node data automatically")]
    public bool CollectNodeDataAutomatically { get; set; }
    
    [Option(shortName: 'b', longName: "CollectNodeIpGeolocations", Default  = false, Required = false, HelpText = "Collect node ip geolocations")]
    public bool CollectNodeIpGeolocations { get; set; }
    
    [Option(shortName: 'n', longName: "NumberOfItems", Default = 10, Required = false, HelpText = "Number of items that will be returned")]
    public int NumberOfItems { get; set; }
    
    [Option(shortName: 'q', longName: "QueryDatabase", Default = false, Required = false, HelpText = "Return data from the database")]
    public bool QueryDatabase { get; set; }
    
    [Option(shortName: 'd', longName: "Date", Required = false, HelpText = "Date")]
    public string? Date { get; set; }
}
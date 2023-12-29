using CommandLine;

namespace KadenaNodeWatcher.Collector;

public class RunningOptions
{
    [Option(shortName: 'h', longName: "hostname", Required = false, HelpText = "Host name")]
    public string HostName { get; set; }
    
    [Option(shortName: 'c', longName: "CheckChildNodes", Default  = false, Required = false, HelpText = "Check child nodes")]
    public bool CheckChildNodes { get; set; }
    
    [Option(shortName: 'g', longName: "CheckIpGeolocation", Default  = false, Required = false, HelpText = "Check IP Geolocation")]
    public bool CheckIpGeolocation { get; set; }
}
using CommandLine;

namespace KadenaNodeWatcher.ConsoleApp;

public class RunningOptions
{
    [Option(shortName: 'h', longName: "hostname", Required = false, HelpText = "Host name")]
    public string HostName { get; set; }
    
    [Option(shortName: 'c', longName: "CheckChildNodes", Default  = false, Required = false, HelpText = "Check child nodes")]
    public bool CheckChildNodes { get; set; }
}
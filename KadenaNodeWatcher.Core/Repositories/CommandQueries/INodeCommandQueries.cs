﻿namespace KadenaNodeWatcher.Core.Repositories.CommandQueries;

internal interface INodeCommandQueries
{
    string AddNode { get; }

    string CountNodes(bool? isOnline = null);
    
    string IpGeolocationExists { get; }
    
    string GetIpGeolocation { get; }
    
    string AddIpGeolocation { get; }
}
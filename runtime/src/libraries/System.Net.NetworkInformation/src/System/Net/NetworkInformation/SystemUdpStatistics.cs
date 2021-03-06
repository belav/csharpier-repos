// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Sockets;

namespace System.Net.NetworkInformation
{
    // UDP statistics.
    internal sealed class SystemUdpStatistics : UdpStatistics
    {
        private readonly Interop.IpHlpApi.MibUdpStats _stats;

        private SystemUdpStatistics() { }

        internal unsafe SystemUdpStatistics(AddressFamily family)
        {
            fixed (Interop.IpHlpApi.MibUdpStats* pStats = &_stats)
            {
                uint result = Interop.IpHlpApi.GetUdpStatisticsEx(pStats, family);
                if (result != Interop.IpHlpApi.ERROR_SUCCESS)
                {
                    throw new NetworkInformationException((int)result);
                }
            }
        }

        public override long DatagramsReceived { get { return _stats.datagramsReceived; } }

        public override long IncomingDatagramsDiscarded { get { return _stats.incomingDatagramsDiscarded; } }

        public override long IncomingDatagramsWithErrors { get { return _stats.incomingDatagramsWithErrors; } }

        public override long DatagramsSent { get { return _stats.datagramsSent; } }

        public override int UdpListeners { get { return (int)_stats.udpListeners; } }
    }
}

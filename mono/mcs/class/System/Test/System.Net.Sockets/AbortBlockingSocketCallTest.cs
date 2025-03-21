using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MonoTests.Helpers;
using NUnit.Framework;

namespace MonoTests.System.Net.Sockets
{
    [TestFixture]
    public class AbortBlockingSocketCallTest
    {
        void StartBlockingAcceptCall()
        {
            TcpListener listener = null;
            try
            {
                listener = NetworkHelpers.CreateAndStartTcpListener(out int port);
                Socket socket = listener.AcceptSocket();
                socket.Close();
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }

        [Test]
        public void AbortBlockingAcceptCall()
        {
            Thread listenerThread = new Thread(StartBlockingAcceptCall);
            listenerThread.Start();
            Thread.Sleep(2000);

            listenerThread.Abort();
            listenerThread.Join();
        }
    }
}

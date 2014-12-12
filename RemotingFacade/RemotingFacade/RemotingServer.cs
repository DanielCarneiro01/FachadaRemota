using System;
using System.Collections;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Ipc;

namespace RemotingFacade
{
    public class RemotingServer<ObjectType, FactoryType>: IDisposable
    {
        IClientChannelSinkProvider clientProvider;
        IServerChannelSinkProvider serverProvider;
        IChannel channel;

        // Summary:
        //     Initializes a new instance of the Remoting.RemotingServer class
        //
        // Parameters:
        //   formatter:
        //     Indicates which formatter to use.
        //
        //   protocol, port, portName
        //     Identify the channel
        //
        //   serviceName, activation
        //     Indicate which activation mode will be used for the type and a name for the binding
        //

        public RemotingServer(
            Formatter formatter, 
            Protocol protocol, int port, string portName,
            string serviceName, ServerActivation activation
        )
        {
            CreateFormatters(formatter);
            RegisterChannel(protocol, port, portName);
            RegisterType(serviceName, activation);
            disposed = false;
        }

        private void CreateFormatters(Formatter formatter)
        {
            switch (formatter)
            {
                case Formatter.Binary:
                    serverProvider = new BinaryServerFormatterSinkProvider();
                    clientProvider = new BinaryClientFormatterSinkProvider(); ;
                    break;
                case Formatter.Soap:
                    serverProvider = new SoapServerFormatterSinkProvider();
                    clientProvider = new SoapClientFormatterSinkProvider(); ;
                    break;
            }
        }

        private void RegisterChannel(Protocol protocol, int port, string portName)
        {
            IDictionary props = new Hashtable();
            channel = null;

            switch (protocol)
            {
                case Protocol.Tcp:
                    props["port"] = port;
                    channel = new TcpChannel(props, clientProvider, serverProvider);
                    break;
                case Protocol.Http:
                    props["port"] = port;
                    channel = new HttpChannel(props, clientProvider, serverProvider);
                    break;
                case Protocol.Ipc:
                    props["portName"] = portName;
                    channel = new IpcChannel(props, clientProvider, serverProvider);
                    break;
            }
            ChannelServices.RegisterChannel(channel, false);
        }

        private static void RegisterType(string serviceName, ServerActivation activation)
        {
            if (activation == ServerActivation.Client)
            {
                // Register factory method as a SOA SingleCall

                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(FactoryType),
                    serviceName,
                    WellKnownObjectMode.SingleCall
                );
            }
            else
            {
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(ObjectType),
                    serviceName,
                    (WellKnownObjectMode)activation
                );
            }
        }

        #region IDisposable Members

        bool disposed = true;
        void IDisposable.Dispose()
        {
            if (! disposed)
                ChannelServices.UnregisterChannel(channel);
            disposed = true;
        }

        #endregion
    }
}

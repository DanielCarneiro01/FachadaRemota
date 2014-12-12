using System;
using System.Collections;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Ipc;

namespace RemotingFacade
{
    public class RemotingClient<ObjectType, FactoryType> : IDisposable
        where FactoryType : IRemotedFactory<ObjectType>
    {
        private string serviceName;
        private ClientActivation activation;
        private string url;
        private FactoryType factory;
        private IChannel channel;
        
        IClientChannelSinkProvider clientProvider;
        IServerChannelSinkProvider serverProvider;

        // Summary:
        //     Initializes a new instance of the Remoting.RemotingClient class
        //
        // Parameters:
        //   formatter:
        //     Indicates which formatter to use.
        //
        //   serverName, protocol, port, portName
        //     Identify the channel
        //
        //   serviceName, activation
        //     Indicate which activation mode will be used for the type and a name for the binding
        //

        public RemotingClient(
            Formatter formatter, 
            Protocol protocol, string serverName, int port, string portName,
            string serviceName, ClientActivation activation)
        {
            this.serviceName = serviceName;
            this.activation = activation;

            CreateFormatters(formatter);
            RegisterChannel(protocol, serverName, port, portName);
            RegisterType(activation);
            disposed = false;
        }

        public ObjectType GetObject()
        {
            if (activation == ClientActivation.Client)
            {
                // This explicit call makes the retrieved Object a CAO
                return (ObjectType)factory.Create();
            }
            else
            {
                return (ObjectType)
                    Activator.GetObject(typeof(ObjectType), url + serviceName);
            }
        }

        private void CreateFormatters(Formatter formatter)
        {
            switch (formatter)
            {
                case Formatter.Binary:
                    serverProvider = new BinaryServerFormatterSinkProvider();
                    clientProvider = new BinaryClientFormatterSinkProvider();
                    break;
                case Formatter.Soap:
                    serverProvider = new SoapServerFormatterSinkProvider();
                    clientProvider = new SoapClientFormatterSinkProvider();
                    break;
            }
        }

        private void RegisterChannel(Protocol protocol, string serverName, int port, string portName)
        {
            IDictionary props = new Hashtable();
            props["port"] = 0;

            channel = null;

            switch (protocol)
            {
                case Protocol.Tcp:
                    url = @"tcp://" + serverName + ":" + port.ToString() + "/";
                    channel = new TcpChannel(props, clientProvider, serverProvider);
                    break;

                case Protocol.Http:
                    url = @"http://" + serverName + ":" + port.ToString() + "/";
                    channel = new HttpChannel(props, clientProvider, serverProvider);
                    break;

                case Protocol.Ipc:
                    url = @"ipc://" + portName + "/";
                    channel = new IpcChannel(props, clientProvider, serverProvider);
                    break;
            }
            ChannelServices.RegisterChannel(channel, false);
        }

        void RegisterType(ClientActivation activation)
        {
            if (activation == ClientActivation.Client)
            {
                // Factory is SAO but it will be used to create an Object as CAO

                // We do not need to call RegisterWellKnownClientType because we are not calling new()
                // RemotingConfiguration.RegisterWellKnownClientType(typeof(FactoryType), url);

                factory = (FactoryType)
                    Activator.GetObject(typeof(FactoryType), url + serviceName);
            }
            else
            {
                // We do not need to call RegisterWellKnownClientType because this.GetObject() does not call new()
                // RemotingConfiguration.RegisterWellKnownClientType(typeof(ObjectType), url);
            }
        }

        #region IDisposable Members

        bool disposed = true;
        void IDisposable.Dispose()
        {
            if (!disposed)
                ChannelServices.UnregisterChannel(channel);
            disposed = true;
        }

        #endregion
    }
}

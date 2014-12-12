//
// The Remoter namespace defines the RemotingClient and the RemotingServer helper classes.
// These classes wrap the .Net Remoting APIs to give simple access to all combinations of .Net
// Remoting:
//      Formatter:  Binary or SOAP 
//      Channel:    TCP or HTTP or IPC
//      Activation: Server Activation Singleton Server Activation Single Call or Client Activation
// There are 2 x 3 x 3 possible combinations. 
// The same combination must be used to configure both RemotingClient and RemotingServer
//

namespace RemotingFacade
{
    public enum Formatter
    {
        Binary,
        Soap
    }

    public enum Protocol
    {
        Tcp,
        Http,
        Ipc
    }
    
    public enum ServerActivation
    {
        Client = 0,
        ServerSingleton = 1,
        ServerSingleCall = 2
    }

    public enum ClientActivation    // The RemotingClient cannot configure Singleton vs SingleCall
    {
        Client = ServerActivation.Client,
        Server                      // Use this for RemotingClient when RemotingServer uses ServerSingleton or ServerSingleCall
    }

    // Implement this interface for Client Activation. 
    // See http://msdn.microsoft.com/en-us/library/ms998520.aspx
    // and http://www.codeproject.com/KB/IP/caofactoryguide.aspx

    public interface IRemotedFactory<ObjectType>
    {
        ObjectType Create();
    }
}

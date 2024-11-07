namespace ArtNet;

// ReSharper disable FunctionNeverReturns
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Enums;
using Packets;
using Packets.Builder;
using Spectre.Console;
using Structs;

public class Server
{
    public static ConcurrentQueue<string> Logs = new();
    internal ServerSettings Settings { get; set; }

    /// <summary>
    /// Provides actions to control the ArtNet Server.
    /// </summary>
    private ServerActions _actions = ServerActions.Idle;
    
    /// <summary>
    /// Provides information about the current state of the ArtNet Server. This should only be updated in the server thread.
    /// </summary>
    private ServerState  _state = ServerState.Stopped;
    private static Thread? ListenThread;
    private const ushort Port = 6454;
    public ConcurrentQueue<DMXUniverseData> Data { get; } = new();
    internal UdpClient? ReceiverClient { get; private set; }
    internal UdpClient? SenderClient { get; private set; }
    internal Socket? ReceiverSocket { get; private set; }
    internal Socket? SenderSocket { get; private set; }
    
    internal IPAddress? LocalAddress { get; private set; }
    internal IReadOnlyList<ArtNetUniverse> NeededUniverses =>  this._cachedUniverses;
    private ConcurrentQueue<OutgoingPacket> PacketsToSend { get; } = new();
    private List<ArtNetUniverse> _cachedUniverses { get; set; } = new ();

    private uint _failedConnects = 0;
    private byte _sequence = 0;
    
    public Server(ServerSettings settings)
    {
        Settings = settings;
        if(settings.DebugMode)
            Log.Object(settings);
        // Only one instance of the server should be running at a time.
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    }

    /// <summary>
    /// Starts the ArtNet server.
    /// </summary>
    public void Run() => this._startServer();

    /// <summary>
    /// Internally starts the server.
    /// </summary>
    private void _startServer()
    {
        this._actions = ServerActions.Run;
        // Check if the thread is still running.
        if (ListenThread is not null && ListenThread.IsAlive )
            return;
        
        // Init the thread if it is not yet running.
        ListenThread ??= new Thread(() => { this.Listen(); });
        
        // Start the thread.
        ListenThread.Start();
    }
    // todo add a public binding that is thread safe.
    private void _restartServer()
    {
        this._actions = ServerActions.Restart;
    }
    // todo add a public binding that is thread safe.
    public void UpdateSettings()
    {
        
    }
    // todo add a public binding that is thread safe.
    // todo Implement logic to make this update the proper settings that were changed.
    private void _updateSettings(ServerSettings newSettings)
    {
        if (Settings.Port != newSettings.Port)
        {
            Log.Debug($"Settings updated, port: {Settings.Port}->{newSettings.Port}", Settings.DebugMode);
        }
        
        if (Settings.DebugMode != newSettings.DebugMode)
        {
            Log.Debug($"Settings updated, Debug Mode: {Settings.DebugMode}->{newSettings.DebugMode}", Settings.DebugMode);
            
        }
        if (Settings.IgnoreSequenceHeader != newSettings.DebugMode)
        {
            Log.Debug($"Settings updated, Ignore Sequence Header: {Settings.IgnoreSequenceHeader}->{newSettings.IgnoreSequenceHeader}", Settings.DebugMode);
            
        }
        
        // Check for any changed IP Addresses.
        var newIps = newSettings.ValidAddresses.Except(Settings.ValidAddresses).ToList();
        var removedIps = Settings.ValidAddresses.Except(newSettings.ValidAddresses).ToList();
        if (newIps.Any() || removedIps.Any())
        {
            Log.Debug($"Settings updated, IP Addresses Changed.", Settings.DebugMode);
            Log.Object(newIps, "New Ip Addresses");
            Log.Object(removedIps, "Removed Ip Addresses");
        }

        // Check for any changed DMX Addresses.
        var newAddresses = newSettings.UniversesToListenFor.Except(Settings.UniversesToListenFor).ToList();
        var removedAddresses = Settings.UniversesToListenFor.Except(newSettings.UniversesToListenFor).ToList();
        if (newAddresses.Any() || removedAddresses.Any())
        {
            Log.Debug($"Settings updated, Addresses to Listen For Changed.", Settings.DebugMode);
            Log.Object(newAddresses, "New Addresses");
            Log.Object(removedAddresses, "Removed Addresses");
        }
    }
    
    // todo rework the address system into allowing multiple universes.
    private void updateAddresses(List<ArtNetUniverse> universes)
    {
        _cachedUniverses = universes;
        Log.Object(this._cachedUniverses, "Cached Universe Addresses.");
    }
    
    /// <summary>
    /// The main Listening Loop. This is what the thread loop essentially is.
    /// </summary>
    // todo - add multicast support.
    private void Listen()
    {
        // Socket should always follow this order.
        /*
         * Get Suitable Ip,
         * Bind to Socket Address,
         * | - Check Run State
         * | - Listen for Packets
         * | - Process Packets
         */
        while (true)
        {
            idle:
            if (this._actions == ServerActions.Restart)
                this._actions = ServerActions.Run;

            Log.Debug($"Server going into idle mode.", Settings.DebugMode && this._actions == ServerActions.Idle);
            while(this._actions == ServerActions.Idle)
            {
                this._state = ServerState.Idling;
                Thread.Sleep(500);
            }
            
            // Stop the server.
            if (this._actions == ServerActions.Stop)
            {
                Log.Debug($"Server is stopping.", Settings.DebugMode);
                this._state = ServerState.Stopped;
                return;
            }
            
            this._state = ServerState.WaitingForIp;
            Log.Debug($"Server Getting Addresses.", Settings.DebugMode);
            // Wait for IP
            while (this.LocalAddress is null)
            {
                LocalAddress = Extensions.GetLocalIPAddress();
                Thread.Sleep(500);
                if (this._actions is not ServerActions.Run)
                    goto idle;
            }
            this._state = ServerState.Starting;
            Log.Info($"Starting ArtNet Server.");

            try
            {
                Log.Debug($"Binding to Address {LocalAddress}:{Port} (Datagram.UDP)", Settings.DebugMode);

                // Open Socket
                Log.Debug($"Binding to socket.", Settings.DebugMode);
                this.SenderClient = new UdpClient();
                this.ReceiverClient = new UdpClient();
                this.SenderClient.Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this.ReceiverClient.Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this.SenderClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                this.ReceiverClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                // ushort localPort = Extensions.GetNextUDPPort(50000);
                //this.SenderClient.Client.Bind(new IPEndPoint(LocalAddress, 0));
                this.SenderClient.Client.Bind(new IPEndPoint(IPAddress.Any, 0));
                // this.ReceiverClient.Client.Bind(new IPEndPoint(LocalAddress, Port));
                this.ReceiverClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));
                this.ReceiverClient.Client.ReceiveTimeout = (int)Settings.DataLossSeconds * 1000;
                this.SenderClient.Client.SendTimeout = 5000;
                // this.SenderClient.Client.SendBufferSize = 512;

                this._state = ServerState.Running;
                Log.Debug($"Listening to socket.", Settings.DebugMode);
                Log.Info($"ArtNet Server is Running.");
                Stopwatch lastDMXFrame = new();
                lastDMXFrame.Start();
                while (this.ReceiverClient.Client.IsBound && this.SenderClient.Client.IsBound)
                {
                    if (this._actions is not ServerActions.Run)
                        break;


                    byte[] buffer = new byte[1024];
                    if (this.PacketsToSend.TryDequeue(out OutgoingPacket packet) && packet.Endpoint is IPEndPoint ipEndPoint)
                    {
                        byte[] bytes = packet.Packet.GetBytes();
                        Log.Debug($"ArtNet Poll Reply Sent ({SenderClient.Client.LocalEndPoint} -> {ipEndPoint.Address.MapToIPv4()}:{ipEndPoint.Port}) [[{bytes.Length}]].", Settings.DebugMode);
                        this.SenderClient.Send(bytes, ipEndPoint);
                        // Thread.Sleep(25);
                    }
                    IPEndPoint ep = new(IPAddress.Any, 0);
                    try
                    {
                        buffer = ReceiverClient.Receive(ref ep);
                    }
                    catch (SocketException e)
                    {
                        if (e.Message.Contains("did not properly respond after a period of time") || e.Message.Contains("Connection timed out"))
                        {
                            timedOut();
                            lastDMXFrame.Restart();
                            continue;
                        }
                        Log.Exception(e);
                    }
                    if (processData(buffer.Length, buffer, ep) is OutputDMXPacket)
                    {
                        this._failedConnects = 0;
                        lastDMXFrame.Restart();
                    }

                    else if (lastDMXFrame.ElapsedMilliseconds > (int)(Settings.DataLossSeconds * 1000))
                    {
                        this.timedOut();
                        lastDMXFrame.Restart();
                    }
                }
                Log.Debug($"Gracefully restarting server.", Settings.DebugMode);
                this._state = this._actions == ServerActions.Restart ? ServerState.Restarting : ServerState.Stopping;
                this.ReceiverClient.Close();
                this.SenderClient.Close();
            }
            catch (NetworkInformationException)
            {
                Log.Debug($"Timed out too many times, Rebinding IP Address.");   
            }
            catch(Exception e)
            {
                this._state = ServerState.Restarting;
                Log.Exception(e, $"Network Loop caught an exception. Restarting Network Loop.");
                // Catch any errors and restart the process.
            }
            this.SenderClient!.Dispose();
            this.ReceiverClient!.Dispose();
            this.SenderClient = null;
            this.ReceiverClient = null;
            LocalAddress = null;
            // this.SenderClient.Dispose();
            // this.ReceiverClient.Dispose();
        }
    }

    /// <summary>
    /// Used when the server doesnt receive data for a long enough time.
    /// </summary>
    private void timedOut()
    {
        this._failedConnects++;
        if (this.Settings.ResetNetworkAdapterAfterXDataFailures != 0 && this.Settings.ResetNetworkAdapterAfterXDataFailures >= this._failedConnects)
        {
            throw new NetworkInformationException(10000);
        }
        // if(Settings.DisableDataLoss())
        Log.Debug($"Data Loss.");
        byte[] emptyData =
        [
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        ];
        foreach (var universe in _cachedUniverses)
        {
            this.Data.Enqueue(new DMXUniverseData(universe, emptyData));
        }
        
    }
    
    /// <summary>
    /// Used to process dmx frames.
    /// </summary>
    private ArtNetPacket? processData(int bytes, byte[] buffer, EndPoint ep)
    {
        var packet = ArtNetPacket.DecodePacket(buffer);
        switch (packet)
        {
            case OutputDMXPacket dmx:
                processDMXPacket(dmx);
                return dmx;
            case ArtPollPacket poll:
                Log.Debug($"Artnet Poll Found", Settings.DebugMode);
                respondWithPollReply(poll, ep);
                return poll;
        }
        return null;
    }

    private void respondWithPollReply(ArtPollPacket packet, EndPoint ep)
    {
        // todo add os tie-in for dhcp / static ip.
        var builder = new ArtNetPollReplyBuilder();
        builder.Init(this.LocalAddress!, Settings.MacAddress, false, "Data", "Data", "Good");
        PacketsToSend.Enqueue(new OutgoingPacket(builder.Build(), new IPEndPoint(((IPEndPoint)ep).Address, ((IPEndPoint)ep).Port)));
        
    }
    private bool isListeningToUniverse(ArtNetUniverse universe)
    {
        // todo fix cache here for multi-universe operations.
        // universe.
        // if(_cachedUniverses.ContainsKey())
        return true;
    }
    private void processDMXPacket(OutputDMXPacket packet)
    {
        try
        {

            if (!isListeningToUniverse(packet.Universe))
            {
                Log.Debug($"Ignoring packet due to unsubscribed universe. Universe [[{packet.Universe.FullUniverse}->{packet.Universe.Subnet}/{packet.Universe.Universe}]]", Settings.DebugMode);
                return;
            }

            Log.Debug($"Artnet Frame Found. Universe [[{packet.Universe.FullUniverse}->{packet.Universe.Subnet}/{packet.Universe.Universe}]]", Settings.DebugMode);

            Log.Debug($"Artnet Sequence. Old [[{packet.Sequence}]], new [[{this._sequence}]]", Settings.DebugMode);
            // Sequence of packets - shouldn't be old.
            if (!Settings.IgnoreSequenceHeader)
            {
                byte newSequence = packet.Sequence;
                byte oldSequence = this._sequence;
                if (!(newSequence == 0b00000000 || newSequence > oldSequence || oldSequence - newSequence > 0b10000000))
                {
                    return;
                }
                this._sequence = newSequence;
            }
            Log.Debug("Artnet Sequence Correct.", Settings.DebugMode);
            var data = new DMXUniverseData(packet.Universe, packet.Data);
            this.Data.Enqueue(data);
            Log.Debug("Enqueued DMX Frame.", Settings.DebugMode);
        }
        catch (Exception e)
        {
            Log.Exception(e, "Artnet processing failed for data.");
        }
    }
    
}

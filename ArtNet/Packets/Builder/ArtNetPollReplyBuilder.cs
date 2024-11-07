// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         ArtNetPollBuilder.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 14:28
//    Created Date:     10/14/2024 14:28
// -----------------------------------------

namespace ArtNet.Packets.Builder;

using System.Net;
using Enums;

// ("OemClayPakySharpy","0x0a89","Clay Paky S.p.A.","Sharpy","1","0","n","y",#"http://www.claypaky.it/"#),
// i am sharpy
public sealed class ArtNetPollReplyBuilder
{
    public ArtNetPollReplyBuilder()
    {
        this._portName = "";
        this._longName = "";
        this._nodeReport = "";
    }

    public ArtPollReplyPacket Build()
    {
        /*
        IPAddress address, ushort port, ushort artnetVersion, byte netSwitch, byte subSwitch,
        ushort oemId, byte UbeaVersion, Status status1, ushort estaId, string portName, string longName, string nodeReport,
        byte numberOfPorts, ArtNetPort[] portTypes, ArtNetInputPortStatus[] goodInputs, ArtNetOutputPortStatus[] goodOutputs,
        byte[] swIn, byte[] swOut, byte acnPriority, ArtNetMacro swMacro, ArtNetRemote swRemote, StyleCode style, byte[] mac,
        byte[]? bindIpAddress = null, byte? bindIndex = null, Status2? status2 = null, ArtNetOutputPortStatus2[]? goodOutputB = null, 
        byte[]? defaultResponseUid = null, ushort? user = null, ushort? refreshRate = null, byte[]? filler = null) : base()
         */
        return new ArtPollReplyPacket(
            address: this._ipAddress, port: this._port, artnetVersion: this._version,
            netSwitch: this._netSwitch, subSwitch: this._subSwitch, oemId: this._oemId,
            ubeaVersion: this._ubeaVersion, status1: this._status1, estaId: this._estaId,
            portName: this._portName, longName: this._longName, nodeReport: this._nodeReport,
            numberOfPorts: this.NumberOfPorts, portTypes: this._portTypes.ToArray(), 
            goodInputs: this._goodInput.ToArray(), goodOutputs: this._goodOutput.ToArray(), swIn: this._swIn, 
            swOut: this._swOut, acnPriority: this._acnPriority, swMacro: this._swMacro, 
            swRemote: this._swRemote, 
            style: this._style, 
            mac: this._mac, 
            bindIpAddress: this._bindIPAddress, bindIndex: this._bindIndex, status2: this._status2, 
            goodOutputB: this._goodOutputB.ToArray(), status3: this._status3, 
            defaultResponseUid: this._defaultResponseUID, user: this._user, refreshRate: this._refreshRate,
            backgroundQueuePolicy: this._backgroundQueuePolicy, filler: this._filler
        );
    }

    public ArtNetPollReplyBuilder Init(IPAddress address, byte[] mac, bool dhcp, string portName, string longName, string nodeReport)
    {
        this.IpAddress = address;
        this.Mac = mac;
        this.PortName = portName;
        this.LongName = longName;
        this.NodeReport = nodeReport;
        this.Status1 = Status.IndicatorsNormalMode | Status.PortProgrammingAuthorityWebConfig;
        this.Status2 = Status2.WebConfigSupport | Status2.DHCPCapable | (dhcp ? Status2.DHCPConfigured : 0);
        this.Status3 = Status3.FailsafeStateOutputTo0;
        return this;
    }

    private bool _rdmEnabled = false;

    public bool RDMEnabled
    {
        get => this._rdmEnabled;
        set
        {
            this._rdmEnabled = value;
            for (int i = 0; i < this.GoodOutputB!.Count; i++)
            {
                ArtNetOutputPortStatus2 status = this.GoodOutputB![i];
                this.GoodOutputB[i].SetFlag(ArtNetOutputPortStatus2.RDMDisabled, value);
            }
            this.Status2.SetFlag(Status2.RDMSupport, value);
            this.Status3.SetFlag(Status3.RDMNetSupported, value);
        }
    }

    public ArtNetPollReplyBuilder AddOutputPort(ArtNetPort portDataType, ArtNetOutputPortStatus outputPortStatus, ArtNetOutputPortStatus2 outputPortStatus2)
    {
        if (this.PortTypes.Count >= 4)
        {
            Log.Warn("Cannot add more than 4 output ports.");
            return this;
        }
        int portNum = this.GoodOutputB.Count;
        this.PortTypes[this.PortTypes.Count] = portDataType;
        this.GoodOutput[portNum] = outputPortStatus;
        this.GoodOutputB[portNum] = outputPortStatus2;
        return this;
    }
    
    public ArtNetPollReplyBuilder AddInputPort(ArtNetPort portDataType, ArtNetInputPortStatus inputPortStatus)
    {
        if (this.PortTypes.Count >= 4)
        {
            Log.Warn("Cannot add more than 4 output ports.");
            return this;
        }
        int portNum = this.GoodInput.Count;
        this.PortTypes[this.PortTypes.Count] = portDataType;
        this.GoodInput[portNum] = inputPortStatus;
        return this;
    }

    public ArtNetPollReplyBuilder RDM(bool value = true)
    {
        this.RDMEnabled = value;
        return this;
    }
    
    private IPAddress _ipAddress = IPAddress.Any;
    public IPAddress IpAddress
    {
        get => this._ipAddress;
        set => this._ipAddress = value;
    }


    private ushort _port = 6454;
    public ushort Port
    {
        get => this._port;
        set => this._port = value;
    }

    private ushort _version = 14;
    public ushort Version
    {
        get => this._version;
    }


    private byte _netSwitch = 0;
    public byte NetSwitch
    {
        get => this._netSwitch;
        set => this._netSwitch = value;
    }

    private byte _subSwitch = 0;
    public byte SubSwitch
    {
        get => this._subSwitch;
        set => this._subSwitch = value;
    }

    private ushort _oemId = 0x0a89;
    public ushort OemId
    {
        get => this._oemId;
        set => this._oemId = value;
    }
    
    
    private byte _ubeaVersion = 0;
    public byte UbeaVersion
    {
        get => this._ubeaVersion;
        set => this._ubeaVersion = value;
    }

    private Status _status1 = 0;
    public Status Status1
    {
        get => this._status1;
        set => this._status1 = value;
    }

    private ushort _estaId = 0x4350;
    public ushort EstaId
    {
        get => this._estaId;
        set => this._estaId = value;
    }
    
    // 18
    private string _portName;
    public string PortName 
    {
        get => this._portName;
        set => this._portName = value;
    }

    // 64
    private string _longName;
    public string LongName
    {
        get => this._longName;
        set => this._longName = value;
    }

    // 64
    private string _nodeReport;
    public string NodeReport
    {
        get => this._nodeReport;
        set => this._nodeReport = value;
    }

    // Currently the second byte only should be used. Max of 4.
    public byte NumberOfPorts
    {
        get => (byte)this._portTypes.Count;
    }

    // 4
    private List<ArtNetPort> _portTypes = new();
    public List<ArtNetPort> PortTypes 
    {
        get => this._portTypes;
    }

    // 4
    private List<ArtNetInputPortStatus> _goodInput = new();
    public List<ArtNetInputPortStatus> GoodInput 
    {
        get => this._goodInput;
        set => this._goodInput = value;
    }
    
    // 4
    private List<ArtNetOutputPortStatus> _goodOutput = new();
    public List<ArtNetOutputPortStatus> GoodOutput 
    {
        get => this._goodOutput;
        set => this._goodOutput = value;
    }

    // 4
    private byte[] _swIn = new byte[4];
    public byte[] SwIn 
    {
        get => this._swIn;
        set => this._swOut = value;
    }

    // 4
    private byte[] _swOut = new byte[4];
    public byte[] SwOut 
    {
        get => this._swOut;
        set => this._swOut = value;
    }

    private byte _acnPriority = 0;
    public byte ACNPriority 
    {
        get => this._acnPriority;
        set => this._acnPriority = value;
    }

    private ArtNetMacro _swMacro = 0;
    public ArtNetMacro SwMacro 
    {
        get => this._swMacro;
        set => this._swMacro = value;
    }

    private ArtNetRemote _swRemote = 0;
    public ArtNetRemote SwRemote 
    {
        get => this._swRemote;
        set => this._swRemote = value;
    }
    
    // 3 - spare bytes
    private byte[] _spare = new byte [3];
    public byte[] Spare 
    {
        get => this._spare;
    }
    
    private StyleCode _style = StyleCode.StNode;
    public StyleCode Style 
    {
        get => this._style;
        set => this._style = value;
    }

    // 6 - Hi, 1, 2, 3, 4, Lo
    
    private byte[] _mac = new byte[6];
    public byte[] Mac 
    {
        get => this._mac;
        set => this._mac = value;
    }
    
    // 4
    private byte[] _bindIPAddress = new byte[4];
    public byte[] BindIPAddress 
    {
        get => this._bindIPAddress;
        set => this._bindIPAddress = value;
    }

    
    private byte _bindIndex = 0;
    public byte BindIndex 
    {
        get => this._bindIndex;
        set => this._bindIndex = value;
    }

    private Status2 _status2 = 0;
    public Status2 Status2 
    {
        get => this._status2;
        set => this._status2 = value;
    }

    // 4
    private List<ArtNetOutputPortStatus2> _goodOutputB = new();
    public List<ArtNetOutputPortStatus2> GoodOutputB 
    {
        get => this._goodOutputB;
        set => this._goodOutputB = value;
    }
    
    private Status3 _status3 = 0;
    public Status3 Status3 
    {
        get => this._status3;
        set => this._status3 = value;
    }

    // 6
    private byte[] _defaultResponseUID = new byte[6];
    public byte[] DefaultResponseUID 
    {
        get => this._defaultResponseUID;
        set => this._defaultResponseUID = value;
    }

    private ushort _user = 0;
    public ushort User 
    {
        get => this._user;
        set => this._user = value;
    }

    // DMX Max rate is 44hz, but it is possible to transmit faster rates.
    private ushort _refreshRate = 44;
    public ushort RefreshRate 
    {
        get => this._refreshRate;
        set => this._refreshRate = value;
    }
    
    private BackgroundQueuePolicy _backgroundQueuePolicy = Enums.BackgroundQueuePolicy.CollectionDisabled;
    public BackgroundQueuePolicy BackgroundQueuePolicy 
    {
        get => this._backgroundQueuePolicy;
        set => this._backgroundQueuePolicy = value;
    }

    // 10
    private byte[] _filler = new byte[10];
    public byte[] Filler 
    {
        get => this._filler;
    }
}
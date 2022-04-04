using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public class Root
{
    public Store[] Stores;

    public Machine[] Machines;
}

public class TransferPoint
{
    [XmlAttribute]
    public string Id;
    public string Description;
    public string DisplayName;
    public string ShortDisplayName;

    public bool HasPositiveDirection;
    public bool MayEnter;
    public bool MayLeave;
    public int PullCount;
    public bool MayPush;
    public bool ShiftGapsOnPull;
    public bool JoinBenches;
    public string StoreId;
    public int StoreIndex;
}

[XmlInclude(typeof(StaticTransportMachine))]
[XmlInclude(typeof(BenchShuttleTransporter))]
[XmlInclude(typeof(MobileTransportMachine))]
[XmlInclude(typeof(ChargerMachine))]
[XmlInclude(typeof(StorelessStaticTransportMachine))]
public class Machine
{
    public string Id;
    public string Description;
    public string MachineNumber;
    public string MachineNumberVersion;
    public string HardwareCode;
    public string StoreId;

    public PhysicalBounds PhysicalBounds;
}

public class StaticTransportMachine : Machine{}
public class BenchShuttleTransporter : Machine{}
public class MobileTransportMachine : Machine{}
public class ChargerMachine : Machine{}
public class StorelessStaticTransportMachine : Machine{}

public class Store
{
    [XmlAttribute]
    public string Id;
    public string Description;
    public string DisplayName;
    public string ShortDisplayName;
    public Vec3 Dimension;
    public Vec3 Orientation;
    public Pnt3 Position;

    public Vec3 BenchSize;
    public int Capacity;
    public int ClaimCapacity;
    public PhysicalBounds PhysicalBounds;
    public TransferPoint[] TransferPoints;
    public string Type;
}
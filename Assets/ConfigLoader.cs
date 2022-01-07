using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class ConfigLoader : MonoBehaviour
{

    public class Vector3
    {
        public double X, Y, Z;
    }

    public class Point3
    {
        public double X, Y, Z;
    }

    public class PhysicalBounds
    {
        [XmlAttribute]
        public double X;

        [XmlAttribute]
        public double Y;

        [XmlAttribute]
        public double Z;

        [XmlAttribute]
        public double Length;

        [XmlAttribute]
        public double Width;

        [XmlAttribute]
        public double Height;
    }

    public class Root
    {
        public Store[] Stores;
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

    public class Store
    {
        [XmlAttribute]
        public string Id;
        public string Description;
        public string DisplayName;
        public string ShortDisplayName;
        public Vector3 Dimension;
        public Vector3 Orientation;
        public Point3 Position;
        public int Capacity;
        public int ClaimCapacity;
        public PhysicalBounds PhysicalBounds;
        public TransferPoint[] TransferPoints;
        public string Type;
    }

    /* <Store Id="In2">
  <Description>Store 'In2'. A store of type PipeRun with a capacity of 1.</Description>
  <DisplayName>In2</DisplayName>
  <ShortDisplayName>In2</ShortDisplayName>
  <Dimension>
    <X>1</X>
    <Y>1</Y>
    <Z>1</Z>
  </Dimension>
  <Orientation>
    <X>0</X>
    <Y>-1</Y>
    <Z>0</Z>
  </Orientation>
  <Position>
    <X>7</X>
    <Y>1</Y>
    <Z>0</Z>
  </Position>
  <ParentElementId>In2</ParentElementId>
  <ParentElementType>StaticTransportMachine</ParentElementType>
  <PhysicalBounds X="5" Y="1" Z="0" Length="1" Width="1" Height="1" />
  <BenchSize>
    <X>1</X>
    <Y>1</Y>
    <Z>1</Z>
  </BenchSize>
  <Capacity>1</Capacity>
  <ClaimCapacity>1</ClaimCapacity>
  <TransferPoints>
    <TransferPoint Id="In2">
      <Description>Transfer Point 'In2'.</Description>
      <DisplayName>In2</DisplayName>
      <ShortDisplayName>In2</ShortDisplayName>
      <HasPositiveDirection>false</HasPositiveDirection>
      <MayEnter>true</MayEnter>
      <MayLeave>true</MayLeave>
      <PullCount>1</PullCount>
      <MayPush>false</MayPush>
      <ShiftGapsOnPull>true</ShiftGapsOnPull>
      <JoinBenches>false</JoinBenches>
      <StoreId>In2</StoreId>
      <StoreIndex>0</StoreIndex>
    </TransferPoint>
  </TransferPoints>
  <Type>PipeRun</Type>
</Store>*/

    public GameObject PhysicalBoundsPrefab;
    public GameObject TransferPointPrefab;

    public Material StoreMaterial;
    public Material TrackMaterial;

    public GameObject ParentObject;
    // Start is called before the first frame update
    void Start()
    {
        var serializer = new XmlSerializer(typeof(Root));
        using var stream = new FileStream("BenchSystem.config", FileMode.Open);
        // Call the Deserialize method and cast to the object type.
        var root = (Root) serializer.Deserialize(stream);

        foreach (var store in root.Stores)
        {
            if (store.PhysicalBounds is null)
            {
                Debug.LogWarningFormat("Store {0} does not have physical bounds!", store.Id);
                continue;
            }

            if (store.Type == "Stack")
            {
                Debug.LogWarningFormat("Ignoring stack {0}!", store.Id);
                continue;
            }

            var material = store.Type switch
            {
                "Track" => TrackMaterial,
                "PipeRun" => StoreMaterial,
                _ => throw new System.NotImplementedException(),
            };

            var name = store.Type switch
            {
                "Track" => "Track",
                "PipeRun" => "Store",
                _ => throw new System.NotImplementedException(),
            };

            var corner = new UnityEngine.Vector3(
                (float)(store.PhysicalBounds.X / 1000.0),
                (float)(store.PhysicalBounds.Z / 1000.0),
                (float)(store.PhysicalBounds.Y / 1000.0)
            );

            var dimensions = new UnityEngine.Vector3(
                (float)(store.PhysicalBounds.Length / 1000.0),
                (float)(store.PhysicalBounds.Height / 1000.0),
                (float)(store.PhysicalBounds.Width / 1000.0)
            );

            var center = corner + (dimensions * 0.5f);

            var storeObject = new GameObject(name + " " + store.DisplayName);
            storeObject.transform.SetParent(ParentObject.transform);
            storeObject.transform.localPosition = center;

            var meshObject = Instantiate(PhysicalBoundsPrefab, storeObject.transform);
            meshObject.name = storeObject.name; 
            meshObject.transform.localScale = dimensions;
            meshObject.GetComponent<MeshRenderer>().material = material;
            
            var xOri = store.Orientation.X != 0;

            foreach (var tp in store.TransferPoints)
            {
                var h = (xOri ? dimensions.x : dimensions.z) * 0.5f;
                var d = Lerp(tp.StoreIndex + 0.5f, 0, store.Capacity, h, -h);
                var tpObject = Instantiate(TransferPointPrefab, storeObject.transform);
                tpObject.transform.localPosition = new UnityEngine.Vector3(xOri ? d : 0, 0, xOri ? 0 : d); 
                tpObject.name = "TransferPoint " + tp.DisplayName;
            }
        }
    }

    private static float Lerp(float x, float x0, float x1, float y0, float y1)
    {
        return (y0 * (x1 - x) + y1 * (x - x0)) / (x1 - x0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

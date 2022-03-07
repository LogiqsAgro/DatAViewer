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

    public GameObject PhysicalBoundsPrefab;
    public GameObject TransferPointPrefab;

    public Material StoreMaterial;
    public Material TrackMaterial;

    public GameObject ParentObject;
    // Start is called before the first frame update

    private FileSystemWatcher watcher;

    private bool shouldReload = true;

    void Start()
    {
        watcher = new FileSystemWatcher(".", "BenchSystem.config");
        watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
        watcher.Changed += (sender, @event) => shouldReload = true;
        watcher.Created += (sender, @event) => shouldReload = true;
        watcher.Deleted += (sender, @event) => shouldReload = true;
        watcher.Renamed += (sender, @event) => shouldReload = true;
        watcher.EnableRaisingEvents = true;
    }

    private void LoadConfiguration(string path)
    {
        // Open stream first so that a failure to open will be caused before we delete anything.
        using var stream = new FileStream(path, FileMode.Open);

        for (var n = ParentObject.transform.childCount; n > 0;)
        {
            Destroy(ParentObject.transform.GetChild(--n).gameObject);
        }

        Debug.LogFormat("Loading configuration from {0}...", path); 
        var serializer = new XmlSerializer(typeof(Root));
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

                var tpObjectTop = Instantiate(TransferPointPrefab, storeObject.transform);
                tpObjectTop.transform.localPosition = new UnityEngine.Vector3(xOri ? d : 0, dimensions.y / 2 - 0.125f, xOri ? 0 : d);
                tpObjectTop.transform.localScale = new UnityEngine.Vector3(0.5f, 0.5f, 0.5f);
                tpObjectTop.name = "TransferPoint " + tp.DisplayName;

                var tpObjectBottom = Instantiate(TransferPointPrefab, storeObject.transform);
                tpObjectBottom.transform.localPosition = new UnityEngine.Vector3(xOri ? d : 0, -dimensions.y / 2 + 0.125f, xOri ? 0 : d);
                tpObjectBottom.transform.localScale = new UnityEngine.Vector3(0.5f, 0.5f, 0.5f);
                tpObjectBottom.name = "TransferPoint " + tp.DisplayName;
            }
        }

        Debug.LogFormat("Loaded configuration from {0}.", path);
    }

    private static float Lerp(float x, float x0, float x1, float y0, float y1)
    {
        return (y0 * (x1 - x) + y1 * (x - x0)) / (x1 - x0);
    }

    void Update()
    {
        if (shouldReload)
        {
            try
            {
                LoadConfiguration("BenchSystem.config");
                shouldReload = false;
            } catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32) {
                // Sharing violation, see https://docs.microsoft.com/en-us/dotnet/standard/io/handling-io-errors
            } catch (FileNotFoundException)
            {
                // Stop trying, the file watcher will trigger again when something changes.
                shouldReload = false;
            }
        }
    }
}

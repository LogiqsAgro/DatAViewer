using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;


public class ConfigLoader : MonoBehaviour
{  

    const string BenchSystemConfig = "BenchSystem.config";

    public GameObject PhysicalBoundsPrefab;
    public GameObject TransferPointPrefab;

    public Material StoreMaterial;
    public Material TrackMaterial;

    public GameObject ParentObject;

    private FileSystemWatcher watcher;

    private bool shouldReload = true;

    private SettingsScript settingsScript;


    // Start is called before the first frame update
    void Start()
    {

        settingsScript = gameObject.GetComponent<SettingsScript>();
        watcher = new FileSystemWatcher(settingsScript.ResolvedConfigurationLocation, BenchSystemConfig);
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
        var root = (Root)serializer.Deserialize(stream);

        var hasCarrierMachineNumber = new HasMachineNumber { MachineNumbers = { "40703" } };
        var carriers = new Collector<Machine> { Condition = hasCarrierMachineNumber };
        carriers.ProcessAll(root.Machines);

        var carrierStores = new HashSet<string>(carriers.Items.Select(x => x.StoreId).Where(id => !string.IsNullOrWhiteSpace(id)));
        var stores = root.Stores.Where(s => !carrierStores.Contains(s.Id)).ToList();

        var benchSize = new Vec3 { X = 1250, Y = 2600, Z = 100 };
        new BenchSizeUpdater
        {
            Condition = new StoreBenchSizeInvalid(),
            BenchSize = benchSize,
        }.ProcessAll(stores);

        var storeBoundsInvalid = new StoreBoundsInvalid();
        new StorePhysicalBoundUpdater
        {
            Condition = storeBoundsInvalid,
            BenchSize = benchSize,
        }.ProcessAll(stores);


        Debug.LogFormat("Loading {0} stores...", root.Stores.Length);

        var isStack = new HasStoreType { Types = { "Stack" } };
        var ignoredStores = isStack.Or(storeBoundsInvalid);
        foreach (var store in stores)
        {
            if (ignoredStores.IsSatisfiedBy(store))
            {
                Debug.LogWarningFormat("Store {0} ignored: matched condition {1}", store.Id, ignoredStores);
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

            var corner = Coordinates.ToUnity(store.PhysicalBounds.Location);
            var dimensions = Coordinates.ToUnity(store.PhysicalBounds.Size);
            var orientation = Coordinates.ToUnity(store.Orientation);
            var center = corner + (dimensions * 0.5f);

            var storeObject = new GameObject(name + " " + store.DisplayName);

            storeObject.transform.SetParent(ParentObject.transform);
            storeObject.transform.position = corner;
            var storeBehavior = storeObject.AddComponent<StoreBehavior>();
            storeBehavior.material = material;
            storeBehavior.size = dimensions;

            // var meshObject = Instantiate(PhysicalBoundsPrefab, storeObject.transform);
            // meshObject.name = storeObject.name + " Mesh";
            // meshObject.transform.localScale = dimensions;
            // meshObject.GetComponent<MeshRenderer>().material = material;

            // var axis = Coordinates.GetAxisAlignment(orientation);

            // var tpLocalScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
            // foreach (var tp in store.TransferPoints)
            // {
            //     var h = Coordinates.GetComponent(dimensions, axis) / 0.5f;
            //     tpLocalScale = Coordinates.SetComponent( new UnityEngine.Vector3(0.1f, 0.1f, 0.1f),axis,1);
            //     var d = Coordinates.Lerp(tp.StoreIndex + 0.5f, 0, store.Capacity, h, -h);

            //     var localPos = Coordinates.SetComponent(dimensions * 0.5f, axis, d);


            //     var tpObject = Instantiate(TransferPointPrefab, storeObject.transform);
            //     tpObject.transform.localPosition = new UnityEngine.Vector3();
            //     tpObject.transform.localScale = tpLocalScale;
            //     tpObject.name = "TransferPoint " + tp.DisplayName;
            // }
        }

        Debug.LogFormat("Loaded configuration from {0}.", path);
    }



    void Update()
    {
        if (shouldReload)
        {
            try
            {
                LoadConfiguration(Path.Combine(settingsScript.ResolvedConfigurationLocation, BenchSystemConfig));
                shouldReload = false;
            }
            catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32)
            {
                // Sharing violation, see https://docs.microsoft.com/en-us/dotnet/standard/io/handling-io-errors
            }
            catch (FileNotFoundException)
            {
                // Stop trying, the file watcher will trigger again when something changes.
                shouldReload = false;
            }
        }
    }
}

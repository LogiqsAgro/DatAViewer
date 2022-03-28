using UnityEngine;

public class SettingsScript : MonoBehaviour
{
    private const string DefaultServerName = "localhost";
    private const bool DefaultAutoConnect = false;

    private const string DefaultConfigurationLocation = "%LOGIQSDIR%";
    private const bool DefaultAutoReloadConfiguration = false;

    public string ServerName { get; set; } = DefaultServerName;
    public bool AutoConnect { get; set; } = DefaultAutoConnect;
    public string ConfigurationLocation { get; set; } = DefaultConfigurationLocation;
    public bool AutoReloadConfiguration { get; set; } = DefaultAutoReloadConfiguration;

    public string ResolvedConfigurationLocation { get; set; }


    void Awake()
    {
        LoadPrefs();
    }

    void OnDestroy()
    {
        SavePrefs();
    }

    ///<ummary> called in editor mode only </summary>
    void Reset()
    {
        ServerName = DefaultServerName;
        AutoConnect = DefaultAutoConnect;
        ConfigurationLocation = DefaultConfigurationLocation;
        AutoReloadConfiguration = DefaultAutoReloadConfiguration;
        ResolvedConfigurationLocation = "";
        Debug.Log("Reset SettingsScript: " + ToJson());
    }

    void LoadPrefs()
    {
        ServerName = PlayerPrefs.GetString(nameof(ServerName), DefaultServerName);
        ConfigurationLocation = PlayerPrefs.GetString(nameof(ConfigurationLocation), DefaultConfigurationLocation);
        AutoConnect = PlayerPrefs.GetInt(nameof(AutoConnect), DefaultAutoConnect ? 1 : 0) > 0;
        AutoReloadConfiguration = PlayerPrefs.GetInt(nameof(AutoReloadConfiguration), DefaultAutoReloadConfiguration ? 1 : 0) > 0;

        UpdateConfigurationLocation(ConfigurationLocation);

        Debug.Log($"{nameof(LoadPrefs)} in {gameObject.name}: {ToJson()}");
    }

    void SavePrefs()
    {
        Debug.Log($"{nameof(SavePrefs)} in {gameObject.name}: {ToJson()}");


        PlayerPrefs.SetString(nameof(ServerName), ServerName);
        PlayerPrefs.SetString(nameof(ConfigurationLocation), ConfigurationLocation);
        PlayerPrefs.SetInt(nameof(AutoConnect), AutoConnect ? 1 : 0);
        PlayerPrefs.SetInt(nameof(AutoReloadConfiguration), AutoReloadConfiguration ? 1 : 0);
    }

    public string UpdateConfigurationLocation(string location)
    {
        try
        {
            Debug.Log($"Resolving configuration location: {location}");
            var expandedLocation = System.Environment.ExpandEnvironmentVariables(location);

            Debug.Log($"Expanded environment variables in location '{location}': '{expandedLocation}'");
            var directory = new System.IO.DirectoryInfo(expandedLocation);
            if (directory.Exists)
            {
                if (location != ConfigurationLocation)
                {
                    ConfigurationLocation = location;
                    PlayerPrefs.SetString(nameof(ConfigurationLocation), location);
                }

                ResolvedConfigurationLocation = directory.FullName;
                Debug.Log($"Resolved configuration location {ResolvedConfigurationLocation}");
                return ResolvedConfigurationLocation;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error resolving configuration location " + ex);
        }

        ResolvedConfigurationLocation = "";
        return "";
    }

    public string ToJson()
    {
        return $"{{\"ServerName\": \"{ServerName}\", \"ConfigurationLocation\": \"{ConfigurationLocation}\", \"AutoConnect\": \"{AutoConnect}\", " +
        $"\"AutoReloadConfiguration\": \"{AutoReloadConfiguration}\", \"ResolvedConfigurationLocation\": \"{ResolvedConfigurationLocation}\"}}";
    }
}
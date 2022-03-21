using UnityEngine;

public class SettingsScript : MonoBehaviour
{
    private const string DefaultServerName = "localhost";
    private const bool DefaultAutoConnect = false;

    private const string DefaultConfigurationLocation = "%LOGIQSDIR%";
    private const bool DefaultAutoReloadConfiguration = false;

    public string ServerName = DefaultServerName;
    public bool AutoConnect = DefaultAutoConnect;
    public string ConfigurationLocation = DefaultConfigurationLocation;
    public bool AutoReloadConfiguration = DefaultAutoReloadConfiguration;


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
    }

    void LoadPrefs()
    {
        Debug.Log(nameof(LoadPrefs) + " in " + gameObject.name);
        ServerName = PlayerPrefs.GetString(nameof(ServerName), DefaultServerName);
        ConfigurationLocation = PlayerPrefs.GetString(nameof(ConfigurationLocation), DefaultConfigurationLocation);
        AutoConnect = PlayerPrefs.GetInt(nameof(AutoConnect), DefaultAutoConnect ? 1 : 0) > 0;
        AutoReloadConfiguration = PlayerPrefs.GetInt(nameof(AutoConnect), DefaultAutoReloadConfiguration ? 1 : 0) > 0;
    }

    void SavePrefs()
    {
        Debug.Log(nameof(SavePrefs) + " in " + gameObject.name);
        PlayerPrefs.SetString(nameof(ServerName), ServerName);
        PlayerPrefs.SetString(nameof(ConfigurationLocation), ConfigurationLocation);
        PlayerPrefs.SetInt(nameof(AutoConnect), AutoConnect ? 1 : 0);
        PlayerPrefs.SetInt(nameof(AutoReloadConfiguration), AutoReloadConfiguration ? 1 : 0);
    }
}
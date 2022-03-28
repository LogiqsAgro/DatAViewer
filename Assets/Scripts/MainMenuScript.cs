using System;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class MainMenuScript : MonoBehaviour
{
    private SettingsScript Settings;

    // Start is called before the first frame update
    void Start()
    {
        Settings = gameObject.GetComponent<SettingsScript>();
        var ui = GetVisualElementRoot();
        var btn = ui.Q<Button>(name: "OKButton");
        btn.clicked += OnOKButtonClicked;

        ui.Q<Toggle>("AutoReloadCheckBox").value = Settings.AutoReloadConfiguration;
        ui.Q<Toggle>("AutoReloadCheckBox").RegisterValueChangedCallback(e => Settings.AutoReloadConfiguration = e.newValue);

        var locationInput = ui.Q<TextField>("ConfigurationLocationTextInput");
        locationInput.value = Settings.ConfigurationLocation;
        locationInput.RegisterValueChangedCallback(e =>
        {
            Debug.Log("New configuration location value: " + e.newValue);
            var actualLocation = Settings.UpdateConfigurationLocation(e.newValue);
            if (actualLocation == "")
            {
                var msg = "Could not resolve configuration location";
                Debug.Log(msg);
                locationInput.tooltip = msg;
            }
            else
            {
                var msg = actualLocation;                
                Debug.Log("Config location changed to: "+ msg);
                locationInput.tooltip = actualLocation;
            }
        });


        ui.Q<Toggle>("AutoConnectCheckBox").value = Settings.AutoConnect;
        ui.Q<Toggle>("AutoConnectCheckBox").RegisterValueChangedCallback(e => Settings.AutoConnect = e.newValue);

        ui.Q<TextField>("ServerNameTextField").value = Settings.ServerName;
        ui.Q<TextField>("ServerNameTextField").RegisterValueChangedCallback(e => Settings.ServerName = e.newValue);
    }

    VisualElement GetVisualElementRoot()
    {
        var doc = gameObject.GetComponent<UIDocument>();
        if (doc == null) return null;
        return doc.rootVisualElement;
    }

    void OnOKButtonClicked()
    {
        var btn = GetVisualElementRoot().Q<Button>(name: "OKButton");
        SceneManager.LoadScene("WorldScene");
    }
}

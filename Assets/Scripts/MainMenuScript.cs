using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private bool _AutoStartControlPort;
    [SerializeField] private bool _AutoReloadOnChange;
    [SerializeField] private int _ControlPort;
    [SerializeField] private string _ConfigurationLocation;

    public bool AutoStartControlPort
    {
        get => _AutoStartControlPort;
        set => _AutoStartControlPort = value;
    }

    public bool AutoReloadOnChange
    {
        get => _AutoReloadOnChange;
        set => _AutoReloadOnChange = value;
    }

    public int ControlPort
    {
        get => _ControlPort;
        set => _ControlPort = value;
    }

    public string ConfigurationLocation
    {
        get => _ConfigurationLocation;
        set => _ConfigurationLocation = value;
    }


    // Start is called before the first frame update
    void Start()
    {
        var btn = GetVisualElementRoot().Q<Button>(name: "OKButton");
        btn.clicked += OnOKButtonClicked;
    }

    // Update is called once per frame
    void Update()
    {

    }

    VisualElement GetVisualElementRoot()
    {
        var doc = this.GetComponentInParent<UIDocument>();
        if (doc == null) return null;
        return doc.rootVisualElement;
    }

    void OnOKButtonClicked()
    {
        var btn = GetVisualElementRoot().Q<Button>(name: "OKButton");
        if (btn.text == "OK")
        {
            btn.text = "OK (TODO Change scene to world view)";
        }
        else
        {
            btn.text = "OK";
        }
    }
}

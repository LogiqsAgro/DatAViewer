#nullable enable

using UnityEngine;

public class Picker : MonoBehaviour
{
    public Camera Camera;
    public UnityEngine.UI.Text PickText;
    private GameObject? lastHitObject;
    private Color? hitColor;

    void Update()
    {
        var mousePosition = Cursor.lockState == CursorLockMode.Locked
            ? new Vector3(Camera.pixelWidth, Camera.pixelHeight, 0) * 0.5f
            : Input.mousePosition;

        Ray ray = Camera.ScreenPointToRay(mousePosition);

        GameObject? hitObject = Physics.Raycast(ray, out var hit) ? hit.transform.gameObject : null;

        if (hitObject != lastHitObject)
        {
            OnHitEnd();
            lastHitObject = hitObject;
            if (lastHitObject != null)
            {
                OnHitStart();
            }
            PickText.text = lastHitObject?.name ?? "";
        }
    }

    private void OnHitStart()
    {
        var meshRenderer = lastHitObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            hitColor = meshRenderer.material.color;
            meshRenderer.material.color += Color.Lerp(hitColor.Value, Color.white, 0.2f);
        }
    }

    private void OnHitEnd()
    {
        if (hitColor is Color color)
        {
            try
            {
                if (lastHitObject != null)
                {
                    var meshRenderer = lastHitObject.GetComponent<MeshRenderer>();
                    if (meshRenderer != null) {
                        meshRenderer.material.color = color;
                    }
                }
            }
            catch (MissingReferenceException) { }
            hitColor = null;
        }
        lastHitObject = null;
    }

}

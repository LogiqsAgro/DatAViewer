#nullable enable

using UnityEngine;

public class Picker : MonoBehaviour
{
    public Camera? Camera;
    public UnityEngine.UI.Text? PickText;
    private GameObject? lastHitObject;
    private Color? hitColor;

    void Update()
    {
        if (Camera == null)
            return;

        Ray ray = MouseUtils.GetMouseRay(Camera);
        GameObject? hitObject = Physics.Raycast(ray, out var hit) ? hit.transform.gameObject : null;

        if (hitObject != lastHitObject)
        {
            OnHitEnd();
            lastHitObject = hitObject;
            if (lastHitObject != null)
            {
                OnHitStart();
            }
            if (PickText != null)
                PickText.text = lastHitObject?.name ?? "";
        }
    }

    private void OnHitStart()
    {
        if (lastHitObject == null) return;

        var meshRenderer = lastHitObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null) return;

        hitColor = meshRenderer.material.color;
        meshRenderer.material.color += Color.Lerp(hitColor.Value, Color.white, 0.2f);
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
                    if (meshRenderer != null)
                    {
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

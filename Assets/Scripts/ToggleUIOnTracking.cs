using UnityEngine;
using Vuforia;

public class ToggleUIOnTracking : MonoBehaviour
{
    public ObserverBehaviour imageTarget;   // drag your ImageTarget here
    public Canvas canvasToToggle;           // drag your Canvas_UI here

    void Awake()
    {
        if (canvasToToggle) 
            canvasToToggle.enabled = false;   // 🔴 Hide at start

        if (imageTarget != null)
            imageTarget.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    private void OnDestroy()
    {
        if (imageTarget != null)
            imageTarget.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool visible = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;
        if (canvasToToggle) canvasToToggle.enabled = visible;
    }
}

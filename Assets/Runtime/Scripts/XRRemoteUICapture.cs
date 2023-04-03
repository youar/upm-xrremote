using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRRemoteUICapture : MonoBehaviour
{
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private RenderTexture targetTexture;

    private void Awake()
    {
        //Initialize targetTexture
        if (targetTexture == null) {
            //Default texture size
            int width = 1080 / 2;
            int height = 1920 / 2;
            int colorDepth = 8;

            //If canvas is found, use its dimensions instead
            if (uiCanvas != null) {
                RectTransform uiCanvasRectTransform = GetComponent<RectTransform>();
                width = Mathf.RoundToInt(uiCanvasRectTransform.rect.width);
                height = Mathf.RoundToInt(uiCanvasRectTransform.rect.height);
            }

            //Create a new RenderTexture
            targetTexture = new RenderTexture(
                width,
                height,
                colorDepth
            );
        }
    }

    public void CaptureUiToRenderTexture()
    {
        //Save canvas's previous state
        Camera prevCam = uiCanvas.worldCamera;
        RenderMode prevRenderMode = uiCanvas.renderMode;

        //Enable UI Camera and ready canvas for capture
        uiCamera.gameObject.SetActive(true);
        uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        uiCanvas.worldCamera = uiCamera;
        uiCamera.targetTexture = targetTexture;

        //Capture the UI layer
        uiCamera.Render();

        //Restore canvas's previous state and disable UI Camera
        uiCamera.targetTexture = null;
        uiCanvas.worldCamera = prevCam;
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        uiCamera.gameObject.SetActive(false);
    }
}

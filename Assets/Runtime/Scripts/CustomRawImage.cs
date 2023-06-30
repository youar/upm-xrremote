using UnityEngine.UI;
using UnityEngine;
using System;

namespace XRRemote
{
    public class CustomRawImage : RawImage
    {
        // public AspectRatioFitter aspectFitter = null;

        protected override void Start()
        {
            AspectRatioFitter aspectFitter = GetComponent<AspectRatioFitter>();
           
            if (aspectFitter != null)
            {
                // Debug.Log("Device Aspect Ratio Variable = " + deviceAspectRatio);
                // Debug.Log("aspect fitter variable = " + aspectFitter.aspectRatio);
                
                if (CustomNdiReceiver.Instance.aspectRatio != 0f)
                {
                    aspectFitter.aspectRatio = CustomNdiReceiver.Instance.aspectRatio;
                }
                // aspectFitter.aspectRatio = (Mathf.Approximately(CustomNdiReceiver.Instance.aspectRatio, 0) ? deviceAspectRatio : CustomNdiReceiver.Instance.aspectRatio);            
                aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            }
        }

        // protected override void OnEnable()
        // {
        //     base.OnEnable();
        //     if (CustomNdiReceiver.Instance == null) return;
        //     CustomNdiReceiver.Instance.OnAspectRatioChanged += CustomNdiReceiver_OnAspectRatioChanged;
        // }

        // protected override void OnDisable()
        // {
        //     base.OnDisable();
        //     if (CustomNdiReceiver.Instance == null) return;
        //     CustomNdiReceiver.Instance.OnAspectRatioChanged -= CustomNdiReceiver_OnAspectRatioChanged;
        // }


        // protected void CustomNdiReceiver_OnAspectRatioChanged(object sender, EventArgs e)
        // {
        //     Debug.Log(Mathf.Approximately(CustomNdiReceiver.Instance.aspectRatio, 0));
        //     aspectFitter.aspectRatio = (Mathf.Approximately(CustomNdiReceiver.Instance.aspectRatio, 0) ? deviceAspectRatio : CustomNdiReceiver.Instance.aspectRatio);            
        // }
    }
}

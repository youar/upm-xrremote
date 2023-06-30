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
                
                if (CustomNdiReceiver.Instance.aspectRatio != 0f)
                {
                    aspectFitter.aspectRatio = CustomNdiReceiver.Instance.aspectRatio;
                }
                // aspectFitter.aspectRatio = (Mathf.Approximately(CustomNdiReceiver.Instance.aspectRatio, 0) ? deviceAspectRatio : CustomNdiReceiver.Instance.aspectRatio);            
                aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            }
        }
    }
}

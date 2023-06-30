using UnityEngine.UI;
using UnityEngine;
using System;


namespace XRRemote
{
    public class CustomRawImage : RawImage
    {
        // public float? height= null;
        // public float? width = null;
        public AspectRatioFitter aspectFitter = null;
        public float deviceAspectRatio = .565f; // Property in the custom object class

        protected override void Start()
        {
            aspectFitter = GetComponent<AspectRatioFitter>();
           
            if (aspectFitter != null)
            {
                aspectFitter.aspectRatio = deviceAspectRatio;
                aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (CustomNdiReceiver.Instance == null) return;
            CustomNdiReceiver.Instance.OnAspectRatioChanged += CustomNdiReceiver_OnAspectRatioChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (CustomNdiReceiver.Instance == null) return;
            CustomNdiReceiver.Instance.OnAspectRatioChanged -= CustomNdiReceiver_OnAspectRatioChanged;
        }


        protected void CustomNdiReceiver_OnAspectRatioChanged(object sender, EventArgs e)
        {
            Debug.Log("OnAspectRatioChanged Ran");
            if (this.texture != null)
            {
                deviceAspectRatio = (float)this.texture.height / (float)this.texture.width;
                aspectFitter.aspectRatio = deviceAspectRatio;
            }
            
        }
    }
}
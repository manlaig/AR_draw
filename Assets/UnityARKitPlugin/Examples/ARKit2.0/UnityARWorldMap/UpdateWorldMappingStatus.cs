using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class UpdateWorldMappingStatus : MonoBehaviour 
{
	public Text text;
	public Text tracking;
    public Text textAnchor;

   	void Start () 
	{
		UnityARSessionNativeInterface.ARFrameUpdatedEvent += CheckWorldMapStatus;
	}

	void CheckWorldMapStatus(UnityARCamera cam)
	{
        if (cam.worldMappingStatus == ARWorldMappingStatus.ARWorldMappingStatusMapped)
            text.text = "Ready to Save";
        else
            text.text = "Exploring Surroundings";

        if (cam.trackingState == ARTrackingState.ARTrackingStateLimited)
        {
            if (cam.trackingReason == ARTrackingStateReason.ARTrackingStateReasonExcessiveMotion)
                tracking.text = "Too much motion";
            else if (cam.trackingReason == ARTrackingStateReason.ARTrackingStateReasonInsufficientFeatures)
                tracking.text = "Not enough features";
            else if (cam.trackingReason == ARTrackingStateReason.ARTrackingStateReasonRelocalizing)
                tracking.text = "Relocalizing";
            else
                tracking.text = "";
        }
        else
            tracking.text = "";
	}

	void OnDestroy()
	{
		UnityARSessionNativeInterface.ARFrameUpdatedEvent -= CheckWorldMapStatus;
	}

    public void ChangeTextTo(string txt)
    {
        textAnchor.text = txt;
    }
}

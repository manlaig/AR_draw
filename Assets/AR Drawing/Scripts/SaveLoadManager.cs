using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] GameObject exploreEnv;

    UnityARCameraManager arManager;

    public SaveLoadManager(UnityARCameraManager unityARCameraManager)
    {
        arManager = unityARCameraManager;
    }

    public bool CanSave (ARTrackingStateReason reason, ARWorldMappingStatus status)
    {
        if (reason != ARTrackingStateReason.ARTrackingStateReasonNone || status != ARWorldMappingStatus.ARWorldMappingStatusMapped && exploreEnv != null)
        {
            GameObject instance = Instantiate(exploreEnv, GameObject.Find("Canvas").transform);
            Destroy(instance, 3f);
            return false;
        }
        return true;
	}
}

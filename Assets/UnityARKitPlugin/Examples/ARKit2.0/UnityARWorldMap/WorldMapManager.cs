using System.IO;
using System;
using UnityEngine;
using UnityEngine.XR.iOS;

public class WorldMapManager : MonoBehaviour
{
    [SerializeField] UnityARCameraManager m_ARCameraManager = null;
    [SerializeField] GameObject loadingScreen = null;

    SaveLoadManager saveLoadManager; // my custom component (not part of plugin)
    bool relocalizing; // my custom defined variable

	serializableARWorldMap serializedWorldMap;
	ARWorldMap m_LoadedMap;
    ARTrackingStateReason m_LastReason;
    ARWorldMappingStatus lastStatus;

    static UnityARSessionNativeInterface session
    {
        get { return UnityARSessionNativeInterface.GetARSessionNativeInterface(); }
    }

    static string path
    {
        get { return GetPathToSave(); }
    }


    void Start ()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += OnFrameUpdate;
        saveLoadManager = GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>();
        relocalizing = false;
    }


    void OnFrameUpdate(UnityARCamera arCamera)
    {
        m_LastReason = arCamera.trackingReason;
        lastStatus = arCamera.worldMappingStatus;

        if (relocalizing && finishedRelocalizing())
        {
            relocalizing = false;
            saveLoadManager.RelocalizeSuccessful();
        }
    }


    bool finishedRelocalizing()
    {
        bool res = (m_LastReason == ARTrackingStateReason.ARTrackingStateReasonNone &&
                    (lastStatus == ARWorldMappingStatus.ARWorldMappingStatusMapped
                     || lastStatus == ARWorldMappingStatus.ARWorldMappingStatusLimited));
        return res;
    }


    static string GetPathToSave()
    {
        DateTime dt = DateTime.Now;
        string pathToSave = dt.Month.ToString() + "-" + dt.Day.ToString() + "-" + dt.Year.ToString() + " ";
        pathToSave += dt.Hour.ToString() + ":" + dt.Minute.ToString() + ":" + dt.Second.ToString() + ".worldmap";
        return pathToSave;
    }


    void OnWorldMap(ARWorldMap worldMap)
    {
        if (worldMap != null)
        {
            loadingScreen.SetActive(true);

            string pathCurrent = path;
            worldMap.Save(Path.Combine(Application.persistentDataPath, pathCurrent));

            string allMaps = PlayerPrefs.GetString("AllWorldMaps", "");
            allMaps += pathCurrent + '?';
            PlayerPrefs.SetString("AllWorldMaps", allMaps);

			saveLoadManager.SaveSuccessful();
            loadingScreen.SetActive(false);
        }
    }


    public void Save()
    {
        if(saveLoadManager != null)
            if(saveLoadManager.CanSave(m_LastReason, lastStatus))
                session.GetCurrentWorldMapAsync(OnWorldMap);
    }


    public void Load(string pathText)
    {
        var worldMap = ARWorldMap.Load(Path.Combine(Application.persistentDataPath, pathText));
        if (worldMap != null && m_LoadedMap != worldMap)
        {
            m_LoadedMap = worldMap;
            Debug.LogFormat("Map loaded. Center: {0} Extent: {1}", worldMap.center, worldMap.extent);

            UnityARSessionNativeInterface.ARSessionShouldAttemptRelocalization = true;
            relocalizing = true;

            var config = m_ARCameraManager.sessionConfiguration;
            config.worldMap = worldMap;
			UnityARSessionRunOption runOption = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;

			Debug.Log("Restarting session with worldMap");
			session.RunWithConfigAndOptions(config, runOption);

        }
    }


	void OnWorldMapSerialized(ARWorldMap worldMap)
	{
		if (worldMap != null)
		{
			//we have an operator that converts a ARWorldMap to a serializableARWorldMap
			serializedWorldMap = worldMap;
			Debug.Log ("ARWorldMap serialized to serializableARWorldMap");
		}
	}


	public void SaveSerialized()
	{
		session.GetCurrentWorldMapAsync(OnWorldMapSerialized);
	}


	public void LoadSerialized()
	{
		Debug.Log("Loading ARWorldMap from serialized data");
		//we have an operator that converts a serializableARWorldMap to a ARWorldMap
		ARWorldMap worldMap = serializedWorldMap;
		if (worldMap != null)
		{
			m_LoadedMap = worldMap;
			Debug.LogFormat("Map loaded. Center: {0} Extent: {1}", worldMap.center, worldMap.extent);

			UnityARSessionNativeInterface.ARSessionShouldAttemptRelocalization = true;

			var config = m_ARCameraManager.sessionConfiguration;
			config.worldMap = worldMap;
			UnityARSessionRunOption runOption = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;

			Debug.Log("Restarting session with worldMap");
			session.RunWithConfigAndOptions(config, runOption);
		}
	}

}

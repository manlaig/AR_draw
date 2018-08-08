using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.XR.iOS;

public class WorldMapManager : MonoBehaviour
{
    [SerializeField] UnityARCameraManager m_ARCameraManager = null;
    [SerializeField] GameObject loadingScreen = null;
    [SerializeField] GameObject allMaterials = null;

    SaveLoadManager saveLoadManager; // my custom component (not part of plugin)
    bool relocalizing; // my custom defined variable
    float delayToLoad; // my custom defined variable
    string loadedWorldMapName; // my custom defined variable

	serializableARWorldMap serializedWorldMap;
	ARWorldMap m_LoadedMap;
    ARTrackingStateReason m_LastReason;
    ARWorldMappingStatus lastStatus;

    static UnityARSessionNativeInterface session
    {
        get { return UnityARSessionNativeInterface.GetARSessionNativeInterface(); }
    }


    void Start()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += OnFrameUpdate;
        saveLoadManager = GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>();
        relocalizing = false;
        loadedWorldMapName = "";
        delayToLoad = 0f;
    }


    void Update()
    {
        if(relocalizing)
            delayToLoad += Time.deltaTime;
    }


    void OnFrameUpdate(UnityARCamera arCamera)
    {
        m_LastReason = arCamera.trackingReason;
        lastStatus = arCamera.worldMappingStatus;

        if (relocalizing && finishedRelocalizing() && delayToLoad > 0.5f)
        {
            relocalizing = false;
            delayToLoad = 0f;
            saveLoadManager.RelocalizeSuccessful();

            // load the lines from path here because the user has to be relocalized in order to draw the lines
            if(loadedWorldMapName != "")
            {
                string linesFileName = loadedWorldMapName.Split('.')[0] + ".dat";
                LoadLinesFromFile(linesFileName);
            }
        }
    }


    bool finishedRelocalizing()
    {
        bool res = (m_LastReason == ARTrackingStateReason.ARTrackingStateReasonNone &&
                    (lastStatus == ARWorldMappingStatus.ARWorldMappingStatusMapped
                     || lastStatus == ARWorldMappingStatus.ARWorldMappingStatusLimited));
        return res;
    }


    static string GetFileName()
    {
        DateTime dt = DateTime.Now;
        string pathToSave = dt.ToString("MMM dd") + " " + " ";
        pathToSave += dt.Hour.ToString() + ":" + dt.Minute.ToString() + ":" + dt.Second.ToString() + ".worldmap";
        return pathToSave;
    }


    public void Save()
    {
        if(saveLoadManager != null)
        {
            if(saveLoadManager.CanSave(m_LastReason, lastStatus))
            {
                if(PlayerPrefs.GetString("IsFirstTime", "true") == "true")
                {
                    PlayerPrefs.SetString("IsFirstTime", "false");
                    saveLoadManager.SpawnWarningToExplore();
                }
                else
                    session.GetCurrentWorldMapAsync(OnWorldMap);
            }
        }
    }


    void OnWorldMap(ARWorldMap worldMap)
    {
        if (worldMap != null)
        {
            loadingScreen.SetActive(true);

            string fileName = GetFileName();
            string pathToSave = Path.Combine(Application.persistentDataPath, fileName);

            // saving the lineRenderers as a biteArray
            SaveLinesInScene(fileName);
            worldMap.Save(pathToSave);

            string allMaps = PlayerPrefs.GetString("AllWorldMaps", "");
            allMaps += fileName + '?';
            PlayerPrefs.SetString("AllWorldMaps", allMaps);

            saveLoadManager.SaveSuccessful();
            loadingScreen.SetActive(false);
        }
    }


    void SaveLinesInScene(string fileName)
    {
        string pathToSave = Path.Combine(Application.persistentDataPath, fileName.Split('.')[0] + ".dat");

        BinaryFormatter formatter = new BinaryFormatter();

        using(FileStream file = File.Open(pathToSave, FileMode.Create))
        {
            List<GameObject> linesActive = GameObject.Find("LineDrawer").GetComponent<DrawLine>().linesInScene;
            if(linesActive.Count > 0)
            {
                LineRendererData data = new LineRendererData();
                data.lines = new List<SingleLineRendererData>();
                
                for (int i = 0; i < linesActive.Count; i++)
                {
                    SingleLineRendererData lineData = LineRendererToData(linesActive[i].GetComponent<LineRenderer>());
                    if(lineData != null)
                        data.lines.Add(lineData);
                }
                formatter.Serialize(file, data);
            }
        }
    }


    SingleLineRendererData LineRendererToData(LineRenderer line)
    {
        if(line == null)
            return null;

        SingleLineRendererData data = new SingleLineRendererData();

        data.points = new List<Vector3Ser>();

        for (int i = 0; i < line.positionCount; i++)
            data.points.Add(line.GetPosition(i));

        data.startColor = line.startColor;
        data.startWidth = line.startWidth;
        data.endWidth = line.endWidth;
        data.material = (MaterialsEnum) allMaterials.GetComponent<AllMaterials>().GetCorrespondingIndex(line.sharedMaterial);
        data.cornerVertices = line.numCornerVertices;
        
        data.position = line.gameObject.transform.position;
        data.rotation = line.gameObject.transform.rotation;
        data.scale = line.gameObject.transform.localScale;
        
        return data;
    }


    void LoadLinesFromFile(string pathText)
    {
        string pathToLoad = Path.Combine(Application.persistentDataPath, pathText);

        if(File.Exists(pathToLoad))
        {
            using(FileStream file = File.Open(pathToLoad, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                LineRendererData data = (LineRendererData) formatter.Deserialize(file);

                foreach(SingleLineRendererData line in data.lines)
                {
                    GameObject go = new GameObject();
                    
                    go.transform.position = line.position;
                    go.transform.rotation = line.rotation;
                    go.transform.localScale = line.scale;
                    
                    go.AddComponent<LineRenderer>();
                    
                    go.GetComponent<LineRenderer>().startColor = line.startColor;
                    go.GetComponent<LineRenderer>().endColor = line.startColor;
                    go.GetComponent<LineRenderer>().startWidth = line.startWidth;
                    go.GetComponent<LineRenderer>().endWidth = line.endWidth;
                    go.GetComponent<LineRenderer>().numCornerVertices = line.cornerVertices;
                    go.GetComponent<LineRenderer>().numCapVertices = line.cornerVertices;
                    go.GetComponent<LineRenderer>().sharedMaterial = allMaterials.GetComponent<AllMaterials>().materials[(int)line.material];
                    go.GetComponent<LineRenderer>().positionCount = line.points.Count;
                    go.GetComponent<LineRenderer>().textureMode = LineTextureMode.Tile;
                    go.GetComponent<LineRenderer>().receiveShadows = false;
                    go.GetComponent<LineRenderer>().useWorldSpace = false;

                    for (int i = 0; i < line.points.Count; i++)
                        go.GetComponent<LineRenderer>().SetPosition(i, line.points[i]);

                    go.AddComponent<UnityARUserAnchorComponent>();

                    Instantiate(go, go.transform.position, go.transform.rotation);
                }
            }
        }
    }


    public void Load(string fileName)
    {
        var worldMap = ARWorldMap.Load(Path.Combine(Application.persistentDataPath, fileName));

        if (worldMap != null && m_LoadedMap != worldMap)
        {
            m_LoadedMap = worldMap;
            loadedWorldMapName = fileName;

            Debug.LogFormat("Map loaded. Center: {0} Extent: {1}", worldMap.center, worldMap.extent);

            UnityARSessionNativeInterface.ARSessionShouldAttemptRelocalization = true;
            relocalizing = true;

            var config = m_ARCameraManager.sessionConfiguration;
            config.worldMap = worldMap;
			UnityARSessionRunOption runOption = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;

			session.RunWithConfigAndOptions(config, runOption);

        }
    }

    /*
	public void SaveSerialized()
	{
		session.GetCurrentWorldMapAsync(OnWorldMapSerialized);
	}


    void OnWorldMapSerialized(ARWorldMap worldMap)
    {
        if (worldMap != null)
        {
            //we have an operator that converts a ARWorldMap to a serializableARWorldMap
            serializedWorldMap = worldMap;
            Debug.Log("ARWorldMap serialized to serializableARWorldMap");
        }
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
    */
}

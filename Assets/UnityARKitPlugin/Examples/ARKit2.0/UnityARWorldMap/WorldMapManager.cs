using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.XR.iOS;

public class WorldMapManager : MonoBehaviour
{
    [SerializeField] UnityARCameraManager m_ARCameraManager;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] MaterialSO allMaterials;

    public static string loadedWorldMapName; // without extension

    WindowManager windowManager;
    bool relocalizing;
    float startTime;
    ARTrackingStateReason m_LastReason;
    ARWorldMappingStatus lastStatus;

    static UnityARSessionNativeInterface session
    {
        get { return UnityARSessionNativeInterface.GetARSessionNativeInterface(); }
    }


    static string GetFileName()
    {
        DateTime dt = DateTime.Now;
        string pathToSave = string.Concat(dt.ToString("MMM dd"), "  ", dt.Hour.ToString(), ":", dt.Minute.ToString(), ":", dt.Second.ToString());
        return pathToSave;
    }


    void Start()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += OnFrameUpdate;
        windowManager = GameObject.Find("WindowManager").GetComponent<WindowManager>();
        relocalizing = false;
        startTime = 0f;
        loadedWorldMapName = "";
    }


    void Update()
    {
        if (relocalizing && finishedRelocalizing())
        {
            if(Time.time - startTime > 2f)
            {
                relocalizing = false;
                windowManager.RelocalizeSuccessful();

                /*load the lines from path here because the user has to be relocalized in order to draw the lines*/
                if(loadedWorldMapName != "")
                {
                    Debug.Log("Loading lines");
                    string linesFileName = string.Concat(loadedWorldMapName, ".dat");
                    LoadLinesFromFile(linesFileName);
                }
            }
        }
    }


    void OnFrameUpdate(UnityARCamera arCamera)
    {
        m_LastReason = arCamera.trackingReason;
        lastStatus = arCamera.worldMappingStatus;
    }


    bool finishedRelocalizing()
    {
        return m_LastReason != ARTrackingStateReason.ARTrackingStateReasonRelocalizing;
    }


    public void Save()
    {
        SaveLoadManager saveLoadManager = GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>();

        if(saveLoadManager != null)
        {
            if(saveLoadManager.CanSave(lastStatus))
                windowManager.SpawnWarningToExplore();
            else
                windowManager.SpawnExploreEnvironment();
        }
    }


    public void SaveAnyway()
    {
        session.GetCurrentWorldMapAsync(OnWorldMap);
    }


    void OnWorldMap(ARWorldMap worldMap)
    {
        if (worldMap != null)
        {
            loadingScreen.SetActive(true);

            string fileName = "";
            if(loadedWorldMapName == "")
                fileName = GetFileName();
            else
                fileName = loadedWorldMapName;

            string pathToSave = Path.Combine(Application.persistentDataPath, string.Concat(fileName, ".worldmap"));

            // saving the lineRenderers in scene
            SaveLinesInScene(fileName);
            worldMap.Save(pathToSave);


            if(fileName != loadedWorldMapName)
            {
                string allMaps = PlayerPrefs.GetString("AllWorldMaps", "");
                PlayerPrefs.SetString("AllWorldMaps", string.Concat(allMaps, fileName, '?'));
                windowManager.SaveSuccessful();
            }
            else
            {
                windowManager.UpdateSuccessful();
            } 
            loadingScreen.SetActive(false);
        }
    }


    void SaveLinesInScene(string fileName)
    {
        string pathToSave = Path.Combine(Application.persistentDataPath, string.Concat(fileName, ".dat"));
        BinaryFormatter formatter = new BinaryFormatter();

        try
        {
            FileStream file = File.Open(pathToSave, FileMode.Create);
            GameObject[] linesInScene = GameObject.FindGameObjectsWithTag("Line");

            if(linesInScene.Length > 0)
            {
                LineRendererData data = new LineRendererData();
                data.lines = new List<SingleLineRendererData>();
                
                for (int i = 0; i < linesInScene.Length; i++)
                {
                    SingleLineRendererData lineData = null;
                    
                    if(linesInScene[i] != null)
                        lineData = LineRendererToData(linesInScene[i].GetComponent<LineRenderer>());

                    if(lineData != null)
                        data.lines.Add(lineData);
                }
                if(data.lines.Count > 0 && file != null)
                    formatter.Serialize(file, data); 
            }
            file.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            GameObject.Find("Text").GetComponent<UpdateWorldMappingStatus>().ChangeTextTo(e.ToString());
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
        data.material = (MaterialsEnum) allMaterials.GetCorrespondingIndex(line.sharedMaterial);
        data.cornerVertices = line.numCornerVertices;
        
        data.position = line.gameObject.transform.position;
        data.rotation = line.gameObject.transform.rotation;
        data.scale = line.gameObject.transform.localScale;
        
        return data;
    }


    void LoadLinesFromFile(string pathText)
    {
        string pathToLoad = Path.Combine(Application.persistentDataPath, pathText);
        LineRendererData data = null;

        if(File.Exists(pathToLoad))
        {
            try
            {
                using(FileStream file = File.Open(pathToLoad, FileMode.Open))
                {
                    if(file != null)
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        data = (LineRendererData) formatter.Deserialize(file);
                    }
                }

                if(data == null)
                    return;

                foreach(SingleLineRendererData line in data.lines)
                {
                    if(line != null)
                    {
                        GameObject go = new GameObject();
                        
                        go.transform.position = line.position;
                        go.transform.rotation = line.rotation;
                        go.transform.localScale = line.scale;
                        
                        go.tag = "Line";
                        
                        go.AddComponent<LineRenderer>();

                        go.GetComponent<LineRenderer>().startColor = line.startColor;
                        go.GetComponent<LineRenderer>().endColor = line.startColor;
                        go.GetComponent<LineRenderer>().startWidth = line.startWidth;
                        go.GetComponent<LineRenderer>().endWidth = line.endWidth;
                        go.GetComponent<LineRenderer>().numCornerVertices = line.cornerVertices;
                        go.GetComponent<LineRenderer>().numCapVertices = line.cornerVertices;
                        go.GetComponent<LineRenderer>().sharedMaterial = allMaterials.materials[(int)line.material];
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
            catch(Exception e)
            {
                Debug.Log(e.ToString());
                GameObject.Find("Text").GetComponent<UpdateWorldMappingStatus>().ChangeTextTo(e.ToString());
            }
        }
    }
 

    public void Load(string fileName)
    {
        var worldMap = ARWorldMap.Load(Path.Combine(Application.persistentDataPath, string.Concat(fileName, ".worldmap")));

        if (worldMap != null)
        {
            windowManager.SpawnGuideToLoad();

            loadedWorldMapName = fileName;

            UnityARSessionNativeInterface.ARSessionShouldAttemptRelocalization = true;
            relocalizing = true;
            startTime = Time.time;

            ARKitWorldTrackingSessionConfiguration config = m_ARCameraManager.sessionConfiguration;
            config.worldMap = worldMap;
            UnityARSessionRunOption runOption = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;

            foreach(GameObject go in GameObject.FindGameObjectsWithTag("Line"))
                Destroy(go);                        

            session.RunWithConfigAndOptions(config, runOption);
        }
    }
}

using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] GameObject exploreEnv, saveDone, loadSession, contentItem, loadDone, warningToExplore;

    static GameObject loadInstance = null;

    Transform canvas;


    void Start()
    {
        canvas = GameObject.Find("Canvas").transform;
    }


    public bool CanSave (ARTrackingStateReason reason, ARWorldMappingStatus status)
    {
        if (reason != ARTrackingStateReason.ARTrackingStateReasonNone || status != ARWorldMappingStatus.ARWorldMappingStatusMapped && exploreEnv != null)
        {
            SpawnInCanvasAndDestroy(exploreEnv);
            return false;
        }
        return true;
	}


    public void SaveSuccessful()
    {
        SpawnInCanvasAndDestroy(saveDone);
    }


    public void RelocalizeSuccessful()
    {
        SpawnInCanvasAndDestroy(loadDone);
    }


    public void SpawnWarningToExplore()
    {
        Instantiate(warningToExplore, canvas);
    }


	void SpawnInCanvasAndDestroy(GameObject go)
	{
		GameObject instance = Instantiate(go, canvas);
		Destroy(instance, 4f);
	}


    // spawn the window that shows saved AR sessions
    public void LoadToggle()
    {
        if (loadInstance == null)
            LoadSessionWindow();
        else
            DestroyWindow(loadInstance);
    }


    public void LoadSession(Text worldMapText)
    {
        WorldMapManager manager = GameObject.Find("WorldMapManager").GetComponent<WorldMapManager>();
        manager.Load(worldMapText.text + ".worldmap");
    }


    void LoadSessionWindow()
    {
		string[] maps = FetchWorldMaps();
        loadInstance = Instantiate(loadSession, canvas);

        if (maps != null)
        {
            foreach (string s in maps)
            {
                if (s != "" && s != " ")
                {
					Debug.Log(s);
                    GameObject listItem = Instantiate(contentItem, GameObject.Find("Content").transform);
                    foreach (Text t in listItem.GetComponentsInChildren<Text>())
                        if (t != null)
                            t.text = s;
                }
            }
        }
    }


    public void DestroyWindow(GameObject go)
    {
        Destroy(go);
    }


    string[] FetchWorldMaps()
    {
        string allMaps = PlayerPrefs.GetString("AllWorldMaps", "");

        if (allMaps != "")
        {
            string[] words = allMaps.Split('?');

            // splitting fileNames from extensions
            for (int i = 0; i < words.Length; i++)
                words[i] = words[i].Split('.')[0];
            return words;
        }

        return null;
    }
}

using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] GameObject exploreEnv, saveDone, loadSession, contentItem, loadDone;

    static GameObject loadInstance = null;


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


	void SpawnInCanvasAndDestroy(GameObject go)
	{
		GameObject instance = Instantiate(go, GameObject.Find("Canvas").transform);
		Destroy(instance, 3f);
	}


    // spawn the window that shows saved AR sessions
    public void LoadToggle()
    {
        if (loadInstance == null)
            LoadSessionWindow();
        else
            DestroyLoadSessionWindow();
    }


    public void LoadSession(Text worldMapText)
    {
        WorldMapManager manager = GameObject.Find("WorldMapManager").GetComponent<WorldMapManager>();
        manager.Load(worldMapText.text + ".worldmap");
    }


    void LoadSessionWindow()
    {
		string[] maps = FetchWorldMaps();
        loadInstance = Instantiate(loadSession, GameObject.Find("Canvas").transform);

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


    void DestroyLoadSessionWindow()
    {
        Destroy(loadInstance);
    }


    string[] FetchWorldMaps()
    {
        string allMaps = PlayerPrefs.GetString("AllWorldMaps", "");
        Debug.Log("AllMaps: " + allMaps);

        if (allMaps != "")
        {
            string[] words = allMaps.Split('?');

            for (int i = 0; i < words.Length; i++)
                words[i] = words[i].Split('.')[0];
            return words;
        }

        return null;
    }
}

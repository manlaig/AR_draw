using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;
using System.Collections.Generic;

/*
 TRY NOT TO MESS WITH THIS FILE
 I SPENT A WHOLE DAY DEBUGGING IT
 */
public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] GameObject loadSession, contentItem;

    static GameObject loadInstance = null;


    // checking if the camera is currently in mapped state
    public bool CanSave (ARWorldMappingStatus status)
    {
        if (status == ARWorldMappingStatus.ARWorldMappingStatusMapped || status == ARWorldMappingStatus.ARWorldMappingStatusExtending)
            return true;
        return false;
	}


    // called when a list item clicked
    public void LoadSession(Text worldMapText)
    {
        WorldMapManager manager = GameObject.Find("WorldMapManager").GetComponent<WorldMapManager>();
        manager.Load(worldMapText.text);
    }


    // spawn the window that shows previously saved AR sessions, called on button click
    public void LoadToggle()
    {
        if (loadInstance == null)
            LoadSessionWindow();
        else
            Destroy(loadInstance);
    }


    void LoadSessionWindow()
    {
		HashSet<string> maps = FetchWorldMaps();
        loadInstance = Instantiate(loadSession, GameObject.FindGameObjectWithTag("Canvas").transform);

        if (maps != null)
        {
            Transform contentTransform = GameObject.Find("Content").transform;
            foreach (string s in maps)
            {
                if (s != "" && s != " ")
                {
                    GameObject listItem = Instantiate(contentItem, contentTransform);
                    Text t = listItem.GetComponentInChildren<Text>();
                    if (t != null)
                        t.text = s;
                }
            }
        }
    }


    HashSet<string> FetchWorldMaps()
    {
        string allMaps = PlayerPrefs.GetString("AllWorldMaps", "");
        string previous = "";

        if (allMaps != "")
        {
            string[] words = allMaps.Split('?');
            HashSet<string> maps = new HashSet<string>();

            // splitting fileNames from extensions
            for (int i = 0; i < words.Length; i++)
            {
                if(previous == "")
                {
                    previous = words[i];
                    maps.Add(previous);
                }
                else if(words[i] != previous)
                {
                    maps.Add(words[i]);
                    previous = words[i];
                }
            }
            return maps;
        }
        return null;
    }
}

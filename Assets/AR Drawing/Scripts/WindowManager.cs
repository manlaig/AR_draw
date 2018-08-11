﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
	[SerializeField] GameObject askNewDocument, warningToExplore, guideToLoad;
    [SerializeField] GameObject exploreEnv, saveDone, loadDone, updateDone;

	Transform canvas;

	void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas").transform;
    }
	

	void CheckCanvasTransform()
    {
        if(canvas == null)
            canvas = GameObject.FindGameObjectWithTag("Canvas").transform;
    }


	GameObject SpawnInCanvas(GameObject go)
	{
		CheckCanvasTransform();
        return Instantiate(go, canvas);
	}


    public void SaveSuccessful()
    {
		Destroy(SpawnInCanvas(saveDone), 3f);
    }


    public void UpdateSuccessful()
    {
		Destroy(SpawnInCanvas(updateDone), 3f);
    }


    public void RelocalizeSuccessful()
    {
        Destroy(SpawnInCanvas(loadDone), 3f);
    }


    public void SpawnExploreEnvironment()
    {
        Destroy(SpawnInCanvas(exploreEnv), 4f);
    }
    

    public void SpawnWarningToExplore()
    {
		SpawnInCanvas(warningToExplore);
    }


    public void SpawnGuideToLoad()
    {
		SpawnInCanvas(guideToLoad);
    }


    public void SpawnAskToRestartDocument()
    {
		SpawnInCanvas(askNewDocument);
    }


	public void SaveFromWindow()
    {
        WorldMapManager manager = GameObject.Find("WorldMapManager").GetComponent<WorldMapManager>();
        if(manager != null)
            manager.SaveAnyway();
    }


	public void DestroyWindow(GameObject go)
    {
        Destroy(go);
    }
}

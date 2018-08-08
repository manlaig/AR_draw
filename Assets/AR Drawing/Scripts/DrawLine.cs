﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;
using System.Collections.Generic;

public class DrawLine : MonoBehaviour
{
    [SerializeField] float distanceFromCamera = 1f;
    [SerializeField] GameObject line = null;

    // linesInScene is used when saving the anchors 
    public List<GameObject> linesInScene;

    // reference to current lineRenderer to add more points
    LineRenderer lineRenderer;

	// to show a point when user just draws a point, so index = 2
    int index = 2;

	void Start ()
    {
        lineRenderer = null;
        linesInScene = new List<GameObject>();
	}

    void Update ()
    {
        if (Input.touchCount > 0)
            DrawOnTouch();
	}

    void DrawOnTouch()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                InitializeLineRenderer(i);
            }
            else if (Input.GetTouch(i).phase == TouchPhase.Moved && lineRenderer != null)
            {
                UpdateLineRenderer(i);
            }
            else if (Input.GetTouch(i).phase == TouchPhase.Ended)
            {
                index = 2;
                lineRenderer = null;
            }
        }
    }

    void InitializeLineRenderer(int i = 0)
    {
        if (lineRenderer == null && !ButtonPressed(i))
        {
            Vector3 screenPoint = GetScreenPoint(i);

            lineRenderer = Instantiate(line, Camera.main.transform.position, Quaternion.identity).GetComponent<LineRenderer>();

            linesInScene.Add(lineRenderer.gameObject);

            // draw a point if the user just want to draw a point
            lineRenderer.SetPosition(0, screenPoint - lineRenderer.transform.position);
            lineRenderer.SetPosition(1, screenPoint - lineRenderer.transform.position);
        }
    }

    void UpdateLineRenderer(int i = 0)
    {
        Vector3 newPos = GetScreenPoint(i);

        if(PositionChanged(newPos))
        {
            if (index >= lineRenderer.positionCount)
                lineRenderer.positionCount++;

            lineRenderer.SetPosition(index++, newPos - lineRenderer.transform.position);
            // subtracting to always draw in front of the camera
        }
    }

    bool ButtonPressed(int i = 0)
    {
        GameObject canvas = GameObject.Find("Canvas");
        EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        if (canvas != null && eventSystem != null)
        {
            PointerEventData data = new PointerEventData(eventSystem);
            data.position = Input.GetTouch(i).position;

            List<RaycastResult> results = new List<RaycastResult>();
            canvas.GetComponent<GraphicRaycaster>().Raycast(data, results);

            foreach (RaycastResult res in results)
                if (res.gameObject.tag != "Canvas")
                    return true;
        }
        return false;
    }

    // get the position to draw the line on the screen
    Vector3 GetScreenPoint(int i = 0)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, distanceFromCamera));
    }

    bool PositionChanged(Vector3 newPos)
    {
        Vector3 pos = lineRenderer.GetPosition(index - 1);
        Vector3 diff = new Vector3(Mathf.Abs(pos.x), Mathf.Abs(pos.y), Mathf.Abs(pos.z)) -
            new Vector3(Mathf.Abs(newPos.x), Mathf.Abs(newPos.y), Mathf.Abs(newPos.z));

        if (Mathf.Abs(diff.x) + Mathf.Abs(diff.y) + Mathf.Abs(diff.z) >= 0.01f)
            return true;
        return false;
    }
}

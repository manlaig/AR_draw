using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;
using System.Collections.Generic;

public class DrawLine : MonoBehaviour
{
    [SerializeField] float distanceFromCamera = 1f;
    [SerializeField] GameObject line;
    [SerializeField] float tolerance = 2f;
    [SerializeField] float deltaBetweenPoints = 0.03f;

    // reference to current lineRenderer to add more points
    LineRenderer lineRenderer;

    GameObject canvas;
    EventSystem eventSystem;

	// to show a point when user just draws a point, so index = 2
    int index = 2;

	void Start ()
    {
        lineRenderer = null;
        canvas = GameObject.Find("Canvas");
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
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
            else if (Input.GetTouch(i).phase == TouchPhase.Moved || Input.GetTouch(i).phase == TouchPhase.Stationary)
            {
                if(lineRenderer != null)
                    UpdateLineRenderer(i);
            }
            else if (Input.GetTouch(i).phase == TouchPhase.Ended)
            {
                index = 2;
                if(lineRenderer != null)
                    lineRenderer.Simplify(tolerance);
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

            // draw a point if the user just wants to draw a point
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

            // subtracting to always draw in front of the camera
            lineRenderer.SetPosition(index++, newPos - lineRenderer.transform.position);
        }
    }

    bool ButtonPressed(int i = 0)
    {
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
        float diff = Vector3.Distance(newPos, pos);

        if (diff >= deltaBetweenPoints)
            return true;
        return false;
    }

    GameObject[] GetAllLinesInScene()
    {
        return GameObject.FindGameObjectsWithTag("Line");
    }

    public void Undo()
    {
        Debug.Log("Undo button pressed");

        GameObject[] linesInScene = GetAllLinesInScene();

        if(linesInScene.Length > 0)
            Destroy(linesInScene[linesInScene.Length - 1]);
    }

    public void NewDocument()
    {
        foreach(GameObject go in GetAllLinesInScene())
            Destroy(go);

        WorldMapManager.loadedWorldMapName = "";
    }
}

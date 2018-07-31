using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;
using System.Collections.Generic;

public class DrawLine : MonoBehaviour
{
    [SerializeField] float distanceFromCamera = 1f;
    [SerializeField] GameObject line;

    LineRenderer lineRenderer;
    int index = 2;

	void Start ()
    {
        lineRenderer = null;
        //UnityARSessionNativeInterface.ARUserAnchorAddedEvent += NewAnchorAdded;
	}

    /* debugging functions
    void NewAnchorAdded(ARUserAnchor anchor)
    {
        //GameObject.Find("Text").GetComponent<UpdateWorldMappingStatus>().ChangeTextTo("New User Anchor!!!");
    }

    void OnDestroy()
    {
        UnityARSessionNativeInterface.ARUserAnchorAddedEvent -= NewAnchorAdded;
    }
    end debugging functions*/

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
            Vector3 screenPoint = GetScreenPoint(i); // the position where we will draw the line

            lineRenderer = Instantiate(line, Camera.main.transform.position, Quaternion.identity).GetComponent<LineRenderer>();

            lineRenderer.SetPosition(0, screenPoint - lineRenderer.transform.position);
            lineRenderer.SetPosition(1, screenPoint - lineRenderer.transform.position);
        }
    }

    void UpdateLineRenderer(int i = 0)
    {
        Vector3 newPos = GetScreenPoint(i);
        // TODO: update the position if the position has changed significantly since the last pos
        if(PositionChanged(newPos))
        {
            if (index >= lineRenderer.positionCount)
                lineRenderer.positionCount++;

            lineRenderer.SetPosition(index++, newPos - lineRenderer.transform.position); // subtracting to always draw in front of the camera
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

    Vector3 GetScreenPoint(int i = 0)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, distanceFromCamera));
    }

    bool PositionChanged(Vector3 newPos)
    {
        // take the absolute value of the vectors and check the sum of it to be bigger than 0.1
        //Vector3 diff = lineRenderer.GetPosition(index - 1) - newPos;
        //if(diff.x + diff.y + diff.z >= 0.01f)
        Vector3 pos = lineRenderer.GetPosition(index - 1);
        Vector3 diff = new Vector3(Mathf.Abs(pos.x), Mathf.Abs(pos.y), Mathf.Abs(pos.z)) -
            new Vector3(Mathf.Abs(newPos.x), Mathf.Abs(newPos.y), Mathf.Abs(newPos.z));

        if (Mathf.Abs(diff.x) + Mathf.Abs(diff.y) + Mathf.Abs(diff.z) >= 0.01f)
            return true;
        return false;
    }
}

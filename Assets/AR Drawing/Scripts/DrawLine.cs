using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] float distanceFromCamera = 1f;
    [SerializeField] GameObject line;

    LineRenderer lineRenderer;
    int index = 2;

	void Start ()
    {
        lineRenderer = null;
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
        if (lineRenderer == null)
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
        //UpdateWidthOnPressure(i);
        // TODO: update the position if the position has changed significantly since the last pos
        if(PositionChanged(newPos))
        {
            if (index >= lineRenderer.positionCount)
                lineRenderer.positionCount++;

            lineRenderer.SetPosition(index++, newPos - lineRenderer.transform.position);
        }
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

        if (lineRenderer.GetPosition(index - 1) != newPos)
            return true;
        return false;
    }

    void UpdateWidthOnPressure(int i = 0)
    {
        if (!Input.touchPressureSupported)
            return;
        //lineRenderer.startWidth = Input.GetTouch(i).pressure / 10;
        lineRenderer.endWidth = Input.GetTouch(i).pressure / 100;
    }
}

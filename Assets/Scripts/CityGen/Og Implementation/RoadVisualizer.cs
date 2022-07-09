using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadVisualizer : MonoBehaviour
{

    public GameObject nodePrefab;
    public Material lineMaterial;

    public void DrawCity()
    {
        foreach(Segment s in CityManager.instance.segs)
        {
            Instantiate(nodePrefab, s.start, Quaternion.identity);
            DrawLine(s.start, s.end, s.highway ? Color.red : Color.blue);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject line = new GameObject("line");
        line.transform.position = start;
        var lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

}

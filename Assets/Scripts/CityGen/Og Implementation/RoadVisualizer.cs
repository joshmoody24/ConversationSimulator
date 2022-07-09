using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadVisualizer : MonoBehaviour
{
    public Transform visualizationParent;
    public GameObject nodePrefab;
    public Material lineMaterial;

    public void DrawCity()
    {
        foreach(Transform child in visualizationParent)
        {
            Destroy(child.gameObject);
        }
        foreach(Segment s in CityManager.instance.segments)
        {
            if(s.isBranch) Instantiate(nodePrefab, s.start, Quaternion.identity, visualizationParent);
            DrawLine(s.start, s.end, s.highway ? Color.red : Color.blue, s, s.highway ? CityManager.instance.options.HIGHWAY_WIDTH : CityManager.instance.options.DEFAULT_WIDTH);
        }
    }

    // draws the city in a more logical way for road name generation
    public void DrawCityDebug(Segment root1, Segment root2)
    {
        foreach (Transform child in visualizationParent)
        {
            Destroy(child.gameObject);
        }
        DrawSegment(root1);
        DrawSegment(root2);
    }

    void DrawSegment(Segment segment, int depth = 0)
    {
        if (depth > 100) return;
        if (segment.links.front.Count == 0) return;
        if (segment.links.front.Count == 1) Instantiate(nodePrefab, segment.start, Quaternion.identity, visualizationParent);
        DrawLine(segment.start, segment.end, segment.highway ? Color.red : Color.blue, segment, segment.highway ? CityManager.instance.options.HIGHWAY_WIDTH : CityManager.instance.options.DEFAULT_WIDTH);
        foreach(Segment s in segment.links.front)
        {
            DrawSegment(s, depth + 1);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color, Segment segment, float width = 0.1f)
    {
        GameObject line = new GameObject("line");
        line.transform.parent = visualizationParent;
        line.transform.position = start;
        var lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

}

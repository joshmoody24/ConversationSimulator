using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadVisualizer : MonoBehaviour
{
    public Transform visualizationParent;
    public GameObject nodePrefab;
    public Material lineMaterial;

    public HashSet<Segment> drawnSegments;

    // draws the city in a more logical way for road name generation
    public void DrawCity(Segment root1, Segment root2)
    {
        // the hashset shouldn't be necessary (severed) but bugs
        drawnSegments = new HashSet<Segment>();
        foreach (Transform child in visualizationParent)
        {
            Destroy(child.gameObject);
        }
        DrawSegment(root1);
        DrawSegment(root2);
    }

    void DrawSegment(Segment segment, int depth = 0)
    {
        if (drawnSegments.Contains(segment)) return;
        drawnSegments.Add(segment);
        if (depth > 100) return;
        Instantiate(nodePrefab, segment.start, Quaternion.identity);
        DrawLine(segment.start, segment.end, segment.highway ? Color.red : Color.blue, segment.highway ? CityManager.instance.options.HIGHWAY_WIDTH : CityManager.instance.options.DEFAULT_WIDTH);
        if (segment.links.front.Count == 0) return;
        if (segment.severed) return;

        foreach (Segment s in segment.links.front)
        {
            DrawSegment(s, depth + 1);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color, float width = 0.1f)
    {
        GameObject line = new GameObject("line");
        line.transform.parent = visualizationParent;
        line.transform.position = start;
        var lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color * .3f;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

}

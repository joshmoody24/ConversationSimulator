using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadVisualizer : MonoBehaviour
{
    public Transform visualizationParent;
    public GameObject nodePrefab;
    public Material lineMaterial;

    public HashSet<Rode> drawnRodes;

    // draws the city in a more logical way for road name generation
    public void DrawCity(Rode root1, Rode root2)
    {
        // the hashset shouldn't be necessary (severed) but bugs
        drawnRodes = new HashSet<Rode>();
        foreach (Transform child in visualizationParent)
        {
            Destroy(child.gameObject);
        }
        DrawRode(root1);
        DrawRode(root2);
    }

    void DrawRode(Rode rode, int depth = 0)
    {
        if (drawnRodes.Contains(rode)) return;
        drawnRodes.Add(rode);
        if (depth > 1000) return;
        Instantiate(nodePrefab, rode.position, Quaternion.identity, visualizationParent);
        foreach(Rode conn in rode.GetConnections())
        {
            if (drawnRodes.Contains(conn)) continue;
            Color color = rode.highway && conn.highway ? Color.red : Color.blue;
            float width = rode.highway && conn.highway ? CityManager.instance.options.HIGHWAY_WIDTH : CityManager.instance.options.DEFAULT_WIDTH;
            DrawLine(rode.position, conn.position, color, width);
        }
        if (rode.end) return;

        foreach (Rode c in rode.GetConnections())
        {
            DrawRode(c, depth + 1);
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

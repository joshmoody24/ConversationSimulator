using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Polygon
{
    public List<Vector2> vertices;

    public Polygon(List<Vector2> vertices)
    {
        this.vertices = vertices;
    }

    public Polygon()
    {
        this.vertices = new List<Vector2>();
    }

    public bool IsEmpty()
    {
        return vertices.Count == 0;
    }

    public int VertexCount()
    {
        return vertices.Count;
    }

    public Vector2 Center()
    {
        Vector2 avg = Vector2.zero;
        foreach(Vector2 v in vertices)
        {
            avg += v;
        }
        avg /= vertices.Count;
        return avg;
    }

    public string Hash()
    {
        Vector2 c = Center();
        return Mathf.Round(c.x) + ", " + Mathf.Round(c.y);
    }

    // if same vertices but shifted
    public bool Equals(Polygon p)
    {

        return p.vertices.ToHashSet().Equals(vertices.ToHashSet());

        /* doesn't work
        // repeat the first polygon and then find the first one in it
        List<Vector2> firstPolygonRepeated = new List<Vector2>();
        firstPolygonRepeated.AddRange(vertices);
        firstPolygonRepeated.AddRange(vertices);
        bool result =  ContainsSubsequence<Vector2>(firstPolygonRepeated, p.vertices);
        return result;
        */
    }

    static bool ContainsSubsequence<T>(List<T> sequence, List<T> subsequence)
    {
        return
            Enumerable
                .Range(0, sequence.Count - subsequence.Count + 1)
                .Any(n => sequence.Skip(n).Take(subsequence.Count).SequenceEqual(subsequence));
    }
}

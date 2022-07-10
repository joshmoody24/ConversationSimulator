using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CityMeshBuilder : MonoBehaviour
{
    // takes raw city segments and generates solid road mesh from it
    public List<Polygon> plots;

    private HashSet<Rode> completedRodes;

    private HashSet<string> plotChecker;

    private void Start()
    {
        completedRodes = new HashSet<Rode>();
        plotChecker = new HashSet<string>();
        plots = new List<Polygon>();
        GeneratePlots(TestMap());
    }

    public Rode TestMap()
    {
        //    9 8 7
        //    a   6
        //    r r 5
        //      3 4
        Rode leftRoot = new Rode(Vector2.zero, true, null);
        Rode rightRoot = new Rode(new Vector2(10, 0), true, leftRoot);
        leftRoot.parent = rightRoot;
        leftRoot.AddConnection(rightRoot);

        Rode r3 = new Rode(new Vector2(10, -10), true, rightRoot);
        Rode r4 = new Rode(new Vector2(20, -10), true, r3);
        Rode r5 = new Rode(new Vector2(20, 0), true, r4);
        Rode r6 = new Rode(new Vector2(20, 10), true, r5);
        Rode r7 = new Rode(new Vector2(20, 20), true, r6);
        Rode r8 = new Rode(new Vector2(10, 20), true, r7);
        Rode r9 = new Rode(new Vector2(0, 20), true, r8);
        Rode ra = new Rode(new Vector2(0, 10), true, r9);
        leftRoot.AddConnection(ra);
        ra.AddConnection(leftRoot);


        r5.AddConnection(rightRoot);
        rightRoot.AddConnection(r5);

        return rightRoot;
    }

    public void GeneratePlots(Rode rode)
    {
        if (completedRodes.Contains(rode)) return;
        foreach(Polygon poly in GetAllPolygonsTouching(rode))
        {
            // todo: speed up this check exponentially with some sort of hashmap
            // I was too lazy to figure it out earlier
            // (maybe a hash based on the center of mass?)
            if (!plotChecker.Contains(poly.Hash())) {
                plots.Add(poly);
                plotChecker.Add(poly.Hash());
                Debug.Log(poly.Hash());
            }
        }
        completedRodes.Add(rode);
        foreach (Rode conn in rode.GetConnections())
        {
            GeneratePlots(conn);
        }
    }


    public List<Polygon> GetAllPolygonsTouching(Rode rode)
    {
        // find all the polygons touching this vertex
        List<Polygon> polygons = new List<Polygon>();
        foreach(Rode conn in rode.GetConnections())
        {
            Polygon polygon = GetPolygon(conn, rode, rode);
            if (polygon.VertexCount() > 0) polygons.Add(polygon);
        }
        return polygons;
    }

    public Polygon GetPolygon(Rode current, Rode from, Rode start, List<Vector2> vertices = null, int depth = 0)
    {
        // initial state
        if (depth == 0) vertices = new List<Vector2>();

        // success state
        if (current == start && depth != 0) return new Polygon(vertices);

        // fail state
        if (vertices.Contains(current.position)) return new Polygon();

        Rode next = CounterClockwisestConnection(current, from);
        if (next == null) return new Polygon();

        vertices.Add(current.position);

        return GetPolygon(next, current, start, vertices, depth + 1);
    }

    // radians
    float AngleBetween(Vector2 v1, Vector2 v2)
    {
        return Mathf.Acos(
            Vector2.Dot(v1, v2) / (v1.magnitude * v2.magnitude)
        );
    }

    // pick whichever connection brings the total angle without going OVER
    Rode CounterClockwisestConnection(Rode rode, Rode from)
    {
        List<Rode> connections = rode.GetConnections().Except(new List<Rode>() { from }).ToList();
        List<Rode> counterClockwiseConnections = connections.Where(c => Vector3.Cross(rode.position, c.position).z < 0).ToList();
        if (connections.Count == 0) return null;
        if (counterClockwiseConnections.Count == 1) return counterClockwiseConnections[0];
        else if(counterClockwiseConnections.Count > 1)
        {
            // take the one with smallest angle
            Rode smallest = null;
            float smallestAngle = float.PositiveInfinity;
            foreach(Rode r in counterClockwiseConnections)
            {
                //float angle = AngleBetween(rode.position, r.position);
                float angle = rode.Angle(r);
                if(angle < smallestAngle)
                {
                    smallestAngle = angle;
                    smallest = r;
                }
            }
            return smallest;
        }
        else
        {
            // take largest clockwise angle
            Rode largest = null;
            float largestAngle = float.NegativeInfinity;
            foreach (Rode r in connections)
            {
                //float angle = AngleBetween(rode.position, r.position);
                float angle = rode.Angle(r);
                if (angle > largestAngle)
                {
                    largestAngle = angle;
                    largest = r;
                }
            }
            return largest;
        }


        /*
        float angle = rode.Angle(from);
        float highScore = float.NegativeInfinity;
        Rode winner = null;
        foreach (Rode conn in rode.GetConnections())
        {
            if (conn == from) continue;
            float connAngle = conn.Angle(rode);
            float score = angle + connAngle;
            if (score > Mathf.PI * 2) score -= Mathf.PI * 2;

            if (score > highScore)
            {
                highScore = score;
                winner = conn;
            }
        }
        if (winner == from) return null;
        return winner;
        */
    }
}

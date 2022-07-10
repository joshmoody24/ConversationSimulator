using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// a 'road node'
public class Rode : IComparable
{
    public Vector2 position;

    // time-step delay before evaluated
    public int delay = 0;
    public bool highway;
    public bool end;

    // doubly-linked
    private HashSet<Rode> connections;
    public Rode parent;

    public List<Rode> debugConnections;

    public Rode(Vector2 position, bool highway, Rode parent, int delay = 0, bool end = false)
    {
        this.position = position;
        this.end = end;
        this.highway = highway;
        this.delay = delay;
        this.connections = new HashSet<Rode>();
        this.parent = parent;
        if (parent != null) {
            connections.Add(parent);
            parent.AddConnection(this);
         }
    }

    public void AddConnection(Rode r)
    {
        if(r != this)
        {
            connections.Add(r);
        }
        debugConnections = connections.ToList();
    }

    public int NumConnections()
    {
        return connections.Count;
    }

    public HashSet<Rode> GetConnections()
    {
        return connections;
    }

    public static bool ConnectionsOverlap(Vector2 conn1a, Vector2 conn1b, Vector2 conn2a, Vector2 conn2b)
    {
        //Direction of the lines
        Vector2 l1_dir = (conn1a - conn1b).normalized;
        Vector2 l2_dir = (conn2a - conn2b).normalized;

        //If we know the direction we can get the normal vector to each line
        Vector2 l1_normal = new Vector2(-l1_dir.y, l1_dir.x);
        Vector2 l2_normal = new Vector2(-l2_dir.y, l2_dir.x);


        //Step 1: Rewrite the lines to a general form: Ax + By = k1 and Cx + Dy = k2
        //The normal vector is the A, B
        float A = l1_normal.x;
        float B = l1_normal.y;

        float C = l2_normal.x;
        float D = l2_normal.y;

        //To get k we just use one point on the line
        float k1 = (A * conn1a.x) + (B * conn1a.y);
        float k2 = (C * conn2a.x) + (D * conn2a.y);


        //Step 2: are the lines parallel? -> no solutions
        if (IsParallel(l1_normal, l2_normal))
        {
            //Debug.Log("The lines are parallel so no solutions!");
            return false;
        }


        //Step 3: are the lines the same line? -> infinite amount of solutions
        //Pick one point on each line and test if the vector between the points is orthogonal to one of the normals
        if (IsOrthogonal(conn1a - conn2a, l1_normal))
        {
            //Debug.Log("Same line so infinite amount of solutions!");
            //Return false anyway
            return false;
        }


        //Step 4: calculate the intersection point -> one solution
        float x_intersect = (D * k1 - B * k2) / (A * D - B * C);
        float y_intersect = (-C * k1 + A * k2) / (A * D - B * C);

        Vector2 intersectPoint = new Vector2(x_intersect, y_intersect);


        //Step 5: but we have line segments so we have to check if the intersection point is within the segment
        if (IsBetween(conn1a, conn1b, intersectPoint) && IsBetween(conn2a, conn2b, intersectPoint))
        {
            //Debug.Log("We have an intersection point!");
            // Vector2 intersection = intersectPoint;
            return true;
        }

        return false;
    }


    //Are 2 vectors parallel?
    static bool IsParallel(Vector2 v1, Vector2 v2)
    {
        //2 vectors are parallel if the angle between the vectors are 0 or 180 degrees
        if (Vector2.Angle(v1, v2) == 0f || Vector2.Angle(v1, v2) == 180f)
        {
            return true;
        }

        return false;
    }

    //Are 2 vectors orthogonal?
    static bool IsOrthogonal(Vector2 v1, Vector2 v2)
    {
        //2 vectors are orthogonal is the dot product is 0
        //We have to check if close to 0 because of floating numbers
        if (Mathf.Abs(Vector2.Dot(v1, v2)) < 0.000001f)
        {
            return true;
        }

        return false;
    }

    //Is a point c between 2 other points a and b?
    static bool IsBetween(Vector2 a, Vector2 b, Vector2 c)
    {
        bool isBetween = false;

        //Entire line segment
        Vector2 ab = b - a;
        //The intersection and the first point
        Vector2 ac = c - a;

        //Need to check 2 things: 
        //1. If the vectors are pointing in the same direction = if the dot product is positive
        //2. If the length of the vector between the intersection and the first point is smaller than the entire line
        if (Vector2.Dot(ab, ac) > 0f && ab.sqrMagnitude >= ac.sqrMagnitude)
        {
            isBetween = true;
        }

        return isBetween;
    }

    // computes angle using node connection as a base
    public float Angle()
    {
        if (!connections.Contains(parent))
        {
            throw new Exception("Cannot use unconnected Rode as base for angle calculation");
        }
        // compute absolute angle with baseRode
        Vector2 v = parent.position - position;
        // is this actually a cross product? Just copying code lol
        float dir = parent.position.x - position.x;
        float angle = Mathf.Sign(dir) * Vector2.Angle(Vector2.up, v) * (Mathf.PI / 180);
        return angle + Mathf.PI;
    }

    // computes angle using an arbitrary node as the parent
    public float Angle(Rode rode)
    {
        if (!connections.Contains(parent))
        {
            throw new Exception("Cannot use unconnected Rode as base for angle calculation");
        }
        // compute absolute angle with baseRode
        Vector2 v = parent.position - position;
        // is this actually a cross product? Just copying code lol
        float dir = parent.position.x - position.x;
        float angle = Mathf.Sign(dir) * Vector2.Angle(Vector2.up, v) * (Mathf.PI / 180);
        return angle + Mathf.PI;
    }

    public float Distance(Vector2 point)
    {
        return Vector2.Distance(position, point);
    }

    public int CompareTo(object obj)
    {
        Rode s = obj as Rode;
        return this.delay.CompareTo(s.delay);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Segment : IComparable {

    public Vector2 start;
    public Vector2 end;

    // time-step delay before segment is evaluated
    public int delay = 0;
    public bool highway;
    public bool severed;
    // basically forms a double-linked list but with branches
    public Links links;

    float width;

    public Segment(Vector2 start, Vector2 end, bool highway, int delay = 0, bool severed = false){
        this.start = start;
        this.end = end;
        this.highway = highway;
        this.delay = delay;
        this.width = highway ? CityManager.instance.options.HIGHWAY_WIDTH : CityManager.instance.options.DEFAULT_WIDTH;
        links = new Links();
        this.severed = severed;
    }

    // clockwise direction
    public float GetDirection(){
        Vector2 v = end - start;
        // is this actually a cross product? Just copying code lol
        float dir = end.x - start.x;
        return Mathf.Sign(dir) * Vector2.Angle(Vector2.up, v) * (Mathf.PI / 180);
    }

    public float GetLength(){
        return Vector2.Distance(start, end);
    }

    public Bounds GetLimits(){
        Vector2 center = (start + end) / 2f;
        float width = Mathf.Abs(start.x - end.x);
        float height = Mathf.Abs(start.y - end.y);
        Bounds b = new Bounds(center, new Vector2(width, height));
        return b;
    }

    public int CompareTo(object obj)
    {
        Segment s = obj as Segment;
        return this.delay.CompareTo(s.delay);
    }

    //Check if the lines are interesecting in 2d space
    public bool Intersects(Segment other, out Vector2? position)
    {
        //Direction of the lines
        Vector2 l1_dir = (end - start).normalized;
        Vector2 l2_dir = (other.end - other.start).normalized;

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
        float k1 = (A * start.x) + (B * start.y);
        float k2 = (C * other.start.x) + (D * other.start.y);


        //Step 2: are the lines parallel? -> no solutions
        if (IsParallel(l1_normal, l2_normal))
        {
            //Debug.Log("The lines are parallel so no solutions!");

            position = null;
            return false;
        }


        //Step 3: are the lines the same line? -> infinite amount of solutions
        //Pick one point on each line and test if the vector between the points is orthogonal to one of the normals
        if (IsOrthogonal(start - other.start, l1_normal))
        {
            // Debug.Log("Same line so infinite amount of solutions!");

            //Return false anyway
            position = null;
            return false;
        }


        //Step 4: calculate the intersection point -> one solution
        float x_intersect = (D * k1 - B * k2) / (A * D - B * C);
        float y_intersect = (-C * k1 + A * k2) / (A * D - B * C);

        Vector2 intersectPoint = new Vector2(x_intersect, y_intersect);


        //Step 5: but we have line segments so we have to check if the intersection point is within the segment
        if (IsBetween(start, end, intersectPoint) && IsBetween(other.start, other.end, intersectPoint))
        {
            //Debug.Log("We have an intersection point!");
            position = intersectPoint;
            return true;
        }

        position = null;
        return false;
    }

    //Are 2 vectors parallel?
    bool IsParallel(Vector2 v1, Vector2 v2)
    {
        //2 vectors are parallel if the angle between the vectors are 0 or 180 degrees
        if (Vector2.Angle(v1, v2) == 0f || Vector2.Angle(v1, v2) == 180f)
        {
            return true;
        }

        return false;
    }

    //Are 2 vectors orthogonal?
    bool IsOrthogonal(Vector2 v1, Vector2 v2)
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
    bool IsBetween(Vector2 a, Vector2 b, Vector2 c)
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
}

public class Links
{
    public List<Segment> back;
    public List<Segment> front;

    public Links()
    {
        back = new List<Segment>();
        front = new List<Segment>();
    }
}
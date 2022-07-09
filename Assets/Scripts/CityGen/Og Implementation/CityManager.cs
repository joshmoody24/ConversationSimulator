using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CityManager : MonoBehaviour {

    public static CityManager instance;

    public CityOptions options;

    public List<Segment> segs;

    public RoadVisualizer visualizer;

    private void Start()
    {
        segs = GenerateCity();
        visualizer.DrawCity();
    }

    // singleton (gasp)
    void Awake(){
        if(instance == null) instance = this;
        else Destroy(this);
    }
    public List<Segment> GenerateCity(){
        PriorityQueue<Segment> Q = new PriorityQueue<Segment>();
        Q.EnqueueRange(InitialRoad());

        // this is the final list of all road segments
        List<Segment> segments = new List<Segment>();

        // temp
        int iteration = 0;

        while(Q.Count > 0 && iteration < options.MAX_ITERATIONS){
            Segment minSegment = Q.Dequeue();
            // resolve conflicts
            Vector2 oldPos = minSegment.end;
            bool accepted = ApplyLocalConstraints(ref minSegment, segments);
            Vector2 newPos = minSegment.end;
            if (oldPos != newPos)
            {
                Debug.Log("Position changed: " + oldPos + " to " + newPos + " of length " + minSegment.GetLength());
            }
            if(accepted){
                segments.Add(minSegment);
                Q.EnqueueRange(GlobalGoalsGenerate(minSegment));
            }
            iteration += 1;
        }

        return segments;
    }

    // merely generates two highway segments going in opposite directions
    public List<Segment> InitialRoad(){
        Segment s1 = new Segment(
            Vector2.zero,
            new Vector2(options.HIGHWAY_SEGMENT_LENGTH, 0),
            true
        );
        // mirror it
        Segment s2 = new Segment(
            Vector2.zero,
            new Vector2(s1.start.x - options.HIGHWAY_SEGMENT_LENGTH, 0),
            true
        );

        s1.links.back.Add(s2);
        s2.links.back.Add(s1);
        return new List<Segment>() {s1, s2};
    }

    // checks a segment to make sure it's not doing anything funky
    // (like intersecting weird or whatever)
    public bool ApplyLocalConstraints(ref Segment segment, List<Segment> segments){
        List<Segment> intersecting = new List<Segment>();
        foreach (Segment other in segments)
        {
            Vector2? intersectionPoint;
            if (segment.Intersects(other, out intersectionPoint))
            {
                intersecting.Add(other);
            }
        }
        foreach (Segment other in intersecting)
        {
            // if too similar to the road it's intersecting
            if (Mathf.Abs(other.GetDirection() - segment.GetDirection()) < options.MIN_DEGREE_DIFFERENCE) return false;

            Vector2? intersectionPoint;
            segment.Intersects(other, out intersectionPoint);
            if (intersectionPoint == null) continue;
            SplitSegment(other, (Vector2)intersectionPoint, ref segments, ref segment);
            segment.end = (Vector2)intersectionPoint;
        }

        return true;


        /*
        // if the new segment is intersecting another one
        List<Segment> intersecting = new List<Segment>();
        foreach(Segment other in segments)
        {
            Vector2? intersectionPoint;
            if (segment.Intersects(other, out intersectionPoint))
            {
                intersecting.Add(other);
            }
        }

        foreach(Segment s in intersecting)
        {
            // create an intersection
            Vector2? intersectionPoint;
            segment.Intersects(s, out intersectionPoint);
            if (intersectionPoint == null) continue;
            SplitSegment(segment, (Vector2)intersectionPoint, ref segments, ref segment);
            segment.end = (Vector2)intersectionPoint;
        }


        return true;
        */
    }

    // generates new segments from an input segment
    // (does not check for whether they are doing funky stuff)
    public List<Segment> GlobalGoalsGenerate(Segment segment){
        List<Segment> newSegments = new List<Segment>();

        if (segment.severed) return newSegments;

        // TODO: sample population here


        // continue straight
        float length = segment.highway ? options.HIGHWAY_SEGMENT_LENGTH : options.DEFAULT_SEGMENT_LENGTH;
        float dir = segment.GetDirection();
        Vector2 end = new Vector2(segment.end.x + Mathf.Sin(dir) * length, segment.end.y + Mathf.Cos(dir) * length);
        Segment straight = new Segment(segment.end, end, segment.highway, segment.delay + 1);
        // chain it to the previous one
        straight.links.back.Add(segment);
        segment.links.front.Add(straight);
        newSegments.Add(straight);

        float branchProbability = segment.highway ? options.HIGHWAY_BRANCH_PROBABILITY : options.DEFAULT_BRANCH_PROBABILITY;
        
        if(Random.Range(0f,1f) < branchProbability && segment.severed == false)
        {
            float dirSign = Mathf.Sign(Random.Range(-1f, 1f));
            float branchDir = segment.GetDirection() + (90f + Random.Range(-options.BRANCH_ANGLE_VARIATION, options.BRANCH_ANGLE_VARIATION)) * (Mathf.PI / 180f) * dirSign;
            Vector2 branchEnd = new Vector2(segment.end.x + Mathf.Sin(branchDir) * length, segment.end.y + Mathf.Cos(branchDir) * length);
            int branchDelay = segment.delay + 1;
            bool highway = segment.highway;
            if(highway && Random.Range(0f,1f) < options.HIGHWAY_TO_STREET_PROBABILITY)
            {
                highway = false;
                Debug.Log("converted to street");
                branchDelay += 1;
            }
            Segment branch = new Segment(segment.end, branchEnd, highway, branchDelay);
            newSegments.Add(branch);

        }
        return newSegments;
    }


    
    public void SplitSegment(Segment segment, Vector2 point, ref List<Segment> segments, ref Segment third)
    {
        segments.Remove(segment);

        // turn that segment into two new segments
        Segment first = new Segment(segment.start, point, segment.highway);
        first.links.back = segment.links.back;
        Segment second = new Segment(point, segment.end, segment.highway);
        second.links.front = segment.links.front;

        first.links.front.Add(second);
        second.links.back.Add(first);

        third.severed = true;
        third.links.front.Add(first);
        third.links.front.Add(second);
        first.links.front.Add(third);
        second.links.back.Add(third);

        segments.Add(first);
        segments.Add(second);
    }
    

}
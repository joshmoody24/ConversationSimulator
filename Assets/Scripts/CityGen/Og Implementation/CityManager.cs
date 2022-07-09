using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CityManager : MonoBehaviour {

    public static CityManager instance;

    public CityOptions options;

    public List<Segment> segments;
    PriorityQueue<Segment> Q;

    public RoadVisualizer visualizer;

    private void Start()
    {
        GenerateCity();
        visualizer.DrawCity(segments[0], segments[1]);
    }

    // singleton (gasp)
    void Awake(){
        if(instance == null) instance = this;
        else Destroy(this);
    }

    public void GenerateCity(){
        Q = new PriorityQueue<Segment>();
        Q.EnqueueRange(InitialRoad());

        // temp
        int iteration = 0;

        while (Q.Count > 0 && iteration < options.MAX_ITERATIONS)
        {
            GenerationStep();
            iteration++;
        }
         
    }

    public void GenerationStep()
    {
        Segment minSegment = Q.Dequeue();
        // resolve conflicts
        bool accepted = ApplyLocalConstraints(ref minSegment, segments);

        if (accepted)
        {
            // now we can link it up
            if(minSegment.links.back.Count > 0) minSegment.links.back[0].links.front.Add(minSegment);
            segments.Add(minSegment);
            Q.EnqueueRange(GlobalGoalsGenerate(minSegment));
        }
        else if (minSegment.highway)
        {
            Debug.Log("Dead highway");
        }
    }

    // merely generates two highway segments going in opposite directions
    public List<Segment> InitialRoad(){
        Segment s1 = new Segment(
            Vector2.zero,
            new Vector2(options.HIGHWAY_SEGMENT_LENGTH, 0),
            true,
            0
        );
        // mirror it
        Segment s2 = new Segment(
            Vector2.zero,
            new Vector2(s1.start.x - options.HIGHWAY_SEGMENT_LENGTH, 0),
            true,
            0
        );

        // s1.links.back.Add(s2);
        //s2.links.back.Add(s1);
        return new List<Segment>() {s1, s2};
    }

    // checks a segment to make sure it's not doing anything funky
    // (like intersecting weird or whatever)
    public bool ApplyLocalConstraints(ref Segment segment, List<Segment> segments){

        // TODO: speed this up with a quadtree
        // (currently 10,o00 iterations is about max before exponential explosion

        if (segments.Count == 0) return true;

        // use the quadtree to snag

        List<Segment> intersecting = new List<Segment>();
        foreach (Segment other in segments)
        {
            Vector2? intersectionPoint;
            if (segment.Intersects(other, out intersectionPoint))
            {
                intersecting.Add(other);
            }
        }
        if (intersecting.Count > 1) return false;

        // find nearest line (Linq has a bug for some reason)
        Segment closestLine = segments[0];
        float closestDist = float.PositiveInfinity;
        foreach(Segment s in segments)
        {
            float currentDist = s.DistToPoint(segment.end);
            if (currentDist < closestDist)
            {
                closestDist = currentDist;
                closestLine = s;
            }
        }
        Vector2 closestPoint = closestLine.NearestPointOnSegment(segment.end);

        if (closestDist < options.INTERSECTION_RADIUS)
        {
            // ideally, join up at an already-existing intersection
            float distToStart = Vector2.Distance(segment.end, closestLine.start);
            float distToEnd = Vector2.Distance(segment.end, closestLine.end);
            float distToIntersection = Mathf.Min(distToStart, distToEnd);
            if(distToIntersection < options.INTERSECTION_RADIUS)
            {
                Vector2 endpoint = distToStart < distToEnd ? closestLine.start : closestLine.end;
                segment.end = endpoint;
            }
            else
            {
                // but if we have to, join up in the middle of a street
                segment.end = closestPoint;
                // break up the other road at that point
                SplitSegment(closestLine, closestPoint, ref segments, ref segment);
            }

            segment.severed = true;
        }


        // even after doing all that stuff, handle any intersections not handled already
        // messier and not as ideal, but it is necessary
        foreach (Segment other in intersecting)
        {
            // if too similar to the road it's intersecting
            if (Mathf.Abs(other.GetDirection() - segment.GetDirection()) < options.MIN_DEGREE_DIFFERENCE)
            {
                return false;
            }

            Vector2? intersectionPoint;
            segment.Intersects(other, out intersectionPoint);
            if (intersectionPoint == null) continue;
            SplitSegment(other, (Vector2)intersectionPoint, ref segments, ref segment);
            segment.end = (Vector2)intersectionPoint;
        }

        // some final checks just in case
        //if (segment.GetLength() < options.MIN_LENGTH) return false;

        return true;
    }

    // generates new segments from an input segment
    // (does not check for whether they are doing funky stuff)
    public List<Segment> GlobalGoalsGenerate(Segment segment){
        List<Segment> newSegments = new List<Segment>();

        if (segment.severed) return newSegments;

        // TODO: sample population here


        // continue straight

        // TODO: instead of using wiggle amount to set the direction, sample various perlin noise directions
        // and move to the area with the highest population
        // but this might be good enough if I'm lazy
        float length = segment.highway ? options.HIGHWAY_SEGMENT_LENGTH : options.DEFAULT_SEGMENT_LENGTH;
        float dir = segment.GetDirection();
        float wiggleMax = CityManager.instance.options.BRANCH_ANGLE_VARIATION * (Mathf.PI / 180);
        float wiggleAmount = Random.Range(-wiggleMax, wiggleMax);
        Vector2 end = new Vector2(segment.end.x + Mathf.Sin(dir + wiggleAmount) * length, segment.end.y + Mathf.Cos(dir + wiggleAmount) * length);
        Segment straight = new Segment(segment.end, end, segment.highway, segment.delay + 1);
        // don't chain to the previous one until it's accepted
        // segment.links.front.Add(straight);
        straight.links.back.Add(segment);
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
                branchDelay += options.STREET_EXTRA_DELAY;
            }
            Segment branch = new Segment(segment.end, branchEnd, highway, branchDelay);
            branch.links.back.Add(segment);
            // segment.links.front.Add(branch);
            branch.isBranch = true;
            newSegments.Add(branch);

        }
        return newSegments;
    }


    
    public void SplitSegment(Segment segment, Vector2 point, ref List<Segment> segments, ref Segment third)
    {
        segments.Remove(segment);

        // turn that segment into two new segments
        Segment first = new Segment(segment.start, point, segment.highway);
        Segment second = new Segment(point, segment.end, segment.highway);

        first.links.back = segment.links.back;
        first.links.front.Add(second);
        second.links.back.Add(first);
        second.links.front = segment.links.front;

        second.isBranch = true;

        third.severed = true;
        //third.links.front.Add(first);
        //third.links.front.Add(second);
        first.links.front.Add(third);

        segments.Add(first);
        segments.Add(second);
    }
    

}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CityManager : MonoBehaviour {

    public static CityManager instance;

    public CityOptions options;

    public HashSet<Rode> rodes;
    private Rode rootRode1;
    private Rode rootRode2;
    PriorityQueue<Rode> Q;

    public RoadVisualizer visualizer;
    public CityMeshBuilder meshBuilder;

    public List<Rode> debugView;

    private void Start()
    {
        GenerateCity();
        visualizer.DrawCity(rootRode1, rootRode2);
        //StartCoroutine(SlowCityGeneration());

        // meshBuilder.GeneratePlots(rootRode1);
    }

    // singleton (gasp)
    void Awake(){
        if(instance == null) instance = this;
        else Destroy(this);
    }

    public void GenerateCity(){
        Q = new PriorityQueue<Rode>();
        Q.EnqueueRange(InitialRoad());
        rodes = new HashSet<Rode>();

        // temp
        int iteration = 0;
        while (Q.Count > 0 && iteration < options.MAX_ITERATIONS)
        {
            GenerationStep();
            iteration++;
        }

        debugView = rodes.ToList();
    }

    public void GenerationStep()
    {
        Rode candidate = Q.Dequeue();
        // resolve conflicts
        Rode toAdd = ApplyLocalConstraints(candidate, rodes);

        if (toAdd != null)
        {
            rodes.Add(toAdd);
            toAdd.parent.AddConnection(toAdd);
            Q.EnqueueRange(GlobalGoalsGenerate(toAdd));
        }
    }

    public IEnumerator SlowCityGeneration()
    {
        Q = new PriorityQueue<Rode>();
        Q.EnqueueRange(InitialRoad());
        rodes = new HashSet<Rode>();

        // temp
        int iteration = 0;
        while (Q.Count > 0 && iteration < options.MAX_ITERATIONS)
        {
            Rode candidate = Q.Dequeue();
            // resolve conflicts
            Rode toAdd = ApplyLocalConstraints(candidate, rodes);

            if (toAdd != null)
            {
                print(toAdd.position);
                rodes.Add(toAdd);
                toAdd.parent.AddConnection(toAdd);
                Q.EnqueueRange(GlobalGoalsGenerate(toAdd));
            }
            iteration++;
            visualizer.DrawCity(rootRode1, rootRode2);        
            debugView = rodes.ToList();
            yield return new WaitForSeconds(options.ITERATION_TIME);
        }

    }

    // merely generates two highway segments going in opposite directions
    public List<Rode> InitialRoad(){

        Rode left = new Rode(new Vector2(-options.HIGHWAY_SEGMENT_LENGTH/2f, 0), true, null);
        Rode right = new Rode(new Vector2(options.HIGHWAY_SEGMENT_LENGTH/2f, 0), true, left);
        left.parent = right;
        left.AddConnection(right);
        rootRode1 = left;
        rootRode2 = right;
        return new List<Rode>() {left, right};
    }

    // checks a segment to make sure it's not doing anything funky
    // (like intersecting weird or whatever)
    // returns either a valid rode to add or null if failed
    public Rode ApplyLocalConstraints(Rode rode, HashSet<Rode> rodes){
        // TODO: speed this up with a quadtree
        // (currently 10,000 iterations is about max before exponential explosion

        if (rodes.Count == 0) return rode;
        if (rode.parent == null) throw new System.Exception("null parent");

        // TODO: use the quadtree to optimize

        // snap to an already-existing rode if one is pretty close
        // instead of creating a new one
        bool snapToExistingRode = false;
        float closestRodeDist = rodes.Min(r => r.Distance(rode.position));
        Rode closestRode = null;
        if (closestRodeDist < options.INTERSECTION_RADIUS)
        {
            closestRode = rodes.First(r => r.Distance(rode.position) == closestRodeDist);
            if (closestRode.NumConnections() < options.MAX_CONNECTIONS_PER_INTERSECTION)
            {
                // don't do anything immediately, just in case this intersects something
                // but PROBABLY add a connection instead of adding a new rode
                snapToExistingRode = true;
                //rode.parent.AddConnection(closestRode);
                //closestRode.AddConnection(rode.parent);
                //return null;
            }
            else return null;
        }

        // dont add if it intersects anything
        Vector2 newConnectionStart = rode.parent.position;
        Vector2 newConnectionEnd = snapToExistingRode ? closestRode.position : rode.position;
        foreach (Rode r in rodes)
        {
            Vector2 oldConnectionStart = r.position;
            foreach(Rode c in r.GetConnections())
            {
                if (c == rode.parent) continue;
                Vector2 oldConnectionEnd = c.position;
                if (newConnectionEnd == oldConnectionEnd || newConnectionStart == oldConnectionStart) continue;
                if (Rode.ConnectionsOverlap(newConnectionStart, newConnectionEnd, oldConnectionStart, oldConnectionEnd))
                {
                    return null;
                }
            }
        }

        // if we got this far, add the new rode
        // or snap to existing one
        if (snapToExistingRode)
        {
            rode.parent.AddConnection(closestRode);
            closestRode.AddConnection(rode.parent);
            return null;
        }
        else
        {
            return rode;
        }
    }

    // generates new segments from an input segment
    // (does not check for whether they are doing funky stuff)
    public List<Rode> GlobalGoalsGenerate(Rode rode){
        List<Rode> newRodes = new List<Rode>();

        if (rode.end) return newRodes;
        if (rode.parent == null) return newRodes;

        // TODO: sample population here

        // continue straight

        // TODO: instead of using wiggle amount to set the direction, sample various perlin noise directions
        // and move to the area with the highest population
        // but this might be good enough if I'm lazy
        float length = rode.highway ? options.HIGHWAY_SEGMENT_LENGTH : options.DEFAULT_SEGMENT_LENGTH;
        // Vector2 newPos = (rode.position - rode.parent.position) + rode.position;
        float dir = rode.Angle();
        float wiggleMax = CityManager.instance.options.BRANCH_ANGLE_VARIATION * (Mathf.PI / 180);
        float wiggleAmount = Random.Range(-wiggleMax, wiggleMax);
        Vector2 newPos = new Vector2(rode.position.x + Mathf.Sin(dir + wiggleAmount) * length, rode.position.y + Mathf.Cos(dir + wiggleAmount) * length);
        Rode straight = new Rode(newPos, rode.highway, rode, rode.delay + 1);
        // don't chain to the previous one until it's accepted
        // rode.AddConnection(straight);
        newRodes.Add(straight);

        float branchProbability = rode.highway ? options.HIGHWAY_BRANCH_PROBABILITY : options.DEFAULT_BRANCH_PROBABILITY;
        
        if(Random.Range(0f,1f) < branchProbability && rode.end == false)
        {
            float dirSign = Mathf.Sign(Random.Range(-1f, 1f));
            float branchDir = rode.Angle() + (90f + Random.Range(-options.BRANCH_ANGLE_VARIATION, options.BRANCH_ANGLE_VARIATION)) * (Mathf.PI / 180f) * dirSign;
            Vector2 branchPos = new Vector2(rode.position.x + Mathf.Sin(branchDir) * length, rode.position.y + Mathf.Cos(branchDir) * length);
            int branchDelay = rode.delay + 1;
            bool highway = rode.highway;
            if(highway && Random.Range(0f,1f) < options.HIGHWAY_TO_STREET_PROBABILITY)
            {
                highway = false;
                branchDelay += options.STREET_EXTRA_DELAY;
            }
            Rode branch = new Rode(branchPos, highway, rode, branchDelay);
            branch.AddConnection(rode);
            // segment.links.front.Add(branch);
            newRodes.Add(branch);

        }
        return newRodes;
    }
}
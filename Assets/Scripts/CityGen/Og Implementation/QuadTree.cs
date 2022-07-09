using UnityEngine;
using System.Collections.Generic;

class QuadTree<T> {
    // arbitrary number
    const int LEAF_NODE_CAPACITY = 4;

    Bounds bounds;

    List<T> objects;
    List<Bounds> objectsBounds;

    QuadTree<T> northWest;
    QuadTree<T> northEast;
    QuadTree<T> southWest;
    QuadTree<T> southEast;

    public QuadTree(Bounds b){
        this.bounds = b;
        objects = new List<T>();
    }

    public bool Insert(Bounds b, T obj){
        if(bounds.Intersects(b) == false) return false;

        if(objects.Count < LEAF_NODE_CAPACITY && IsLeafNode()){
            objects.Add(obj);
            objectsBounds.Add(b);
            return true;
        }

        // if there wasn't space
        if(IsLeafNode()) Subdivide();

        if(northWest.Insert(b, obj)) return true;
        if(northEast.Insert(b, obj)) return true;
        if(southWest.Insert(b, obj)) return true;
        if(southEast.Insert(b, obj)) return true;

        // this should never happen
        return false;
    }

    public bool IsLeafNode(){
        return northWest == null;
    }

    // create four children that divide this quad
    // into 4 equally-sized smaller quads
    private void Subdivide(){

    }

    public List<T> QueryRange(Bounds range){
        List<T> objectsInRange = new List<T>();

        if(bounds.Intersects(range) == false) return objectsInRange;

        foreach(Bounds objB in objectsBounds){
            if (range.Intersects(objB))
            {
                int index = objectsBounds.IndexOf(objB);
                objectsInRange.Add(objects[index]);
            }
        }

        if(IsLeafNode()) return objectsInRange;

        // add all points from children if haven't returned yet
        objectsInRange.AddRange(northWest.QueryRange(range));
        objectsInRange.AddRange(northEast.QueryRange(range));
        objectsInRange.AddRange(southWest.QueryRange(range));
        objectsInRange.AddRange(southEast.QueryRange(range));

        return objectsInRange;
    }
}
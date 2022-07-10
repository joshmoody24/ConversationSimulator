using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Module", menuName = "ProceduralCity/Module Prototype", order = 1)]
public class MapModulePrototype : ScriptableObject
{
    public GameObject gameObject;
    public Symmetry symmetry;
    public enum Symmetry { X, T, I, L, N }

    public Connection up;
    public Connection right;
    public Connection down;
    public Connection left;

    public enum Rotation { Degree0, Degree90, Degree180, Degree270 }

    public List<MapModule> generateAllModules()
    {
        List<MapModule> modules = new List<MapModule>();
        modules.Add(generateModule(0));
        if (symmetry == Symmetry.X) return modules;
        modules.Add(generateModule(90));
        if (symmetry == Symmetry.I) return modules;
        modules.Add(generateModule(270));
        modules.Add(generateModule(180));
        return modules;
    }

    public MapModule generateModule(int rotation)
    {
        int cycles = 0;
        switch (rotation)
        {
            case 0:
                cycles = 0;
                break;
            case 90:
                cycles = 3;
                break;
            case 180:
                cycles = 2;
                break;
            case 270:
                cycles = 1;
                break;
            default:
                cycles = 0;
                break;
        }

        // rotate connectors
        Queue<Connection> shuffler = new Queue<Connection>();
        shuffler.Enqueue(up);
        shuffler.Enqueue(right);
        shuffler.Enqueue(down);
        shuffler.Enqueue(left);
        for (int i = 0; i < cycles; i++)
        {
            shuffler.Enqueue(shuffler.Dequeue());
        }

        var ups = shuffler.Dequeue();
        var rights = shuffler.Dequeue();
        var downs = shuffler.Dequeue();
        var lefts = shuffler.Dequeue();

        return new MapModule(
            this,
            gameObject,
            ups,
            rights,
            downs,
            lefts,
            rotation);
    }
}

[System.Serializable]
public class Connection
{
    public int id;
    public List<MapModulePrototype> excluded;

    public Connection()
    {
        id = 0;
        excluded = new List<MapModulePrototype>();
    }


    public Connection(int id)
    {
        this.id = id;
        excluded = new List<MapModulePrototype>();
    }

    public Connection(int id, List<MapModulePrototype> exclude)
    {
        this.id = id;
        excluded = exclude;
    }
}
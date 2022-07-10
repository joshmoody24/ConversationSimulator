using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Module", menuName = "ProceduralCity/Module Prototype", order = 1)]
public class MapModulePrototype : ScriptableObject
{
    public GameObject gameObject;
    public Symmetry symmetry;
    public enum Symmetry { X, T, I, L, N }
    public int upId;
    public int rightId;
    public int downId;
    public int leftId;

    public enum Rotation { Degree0, Degree90, Degree180, Degree270 }

    public List<MapModule> generateAllModules()
    {
        List<MapModule> modules = new List<MapModule>();
        modules.Add(generateModule(Rotation.Degree0));
        if (symmetry == Symmetry.X) return modules;
        modules.Add(generateModule(Rotation.Degree90));
        if (symmetry == Symmetry.I) return modules;
        modules.Add(generateModule(Rotation.Degree270));
        modules.Add(generateModule(Rotation.Degree180));
        return modules;
    }

    public MapModule generateModule(Rotation rotation)
    {
        int cycles = 0;
        switch (rotation)
        {
            case Rotation.Degree0:
                cycles = 0;
                break;
            case Rotation.Degree90:
                cycles = 3;
                break;
            case Rotation.Degree180:
                cycles = 2;
                break;
            case Rotation.Degree270:
                cycles = 1;
                break;
        }

        // rotate connectors
        Queue<int> shuffler = new Queue<int>();
        shuffler.Enqueue(upId);
        shuffler.Enqueue(rightId);
        shuffler.Enqueue(downId);
        shuffler.Enqueue(leftId);
        for (int i = 0; i < cycles; i++)
        {
            shuffler.Enqueue(shuffler.Dequeue());
        }

        int up = shuffler.Dequeue();
        int right = shuffler.Dequeue();
        int down = shuffler.Dequeue();
        int left = shuffler.Dequeue();

        return new MapModule(gameObject, up, right, down, left, rotation);
    }
}

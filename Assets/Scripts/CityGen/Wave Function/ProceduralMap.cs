using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralMap : MonoBehaviour
{
    public List<MapModulePrototype> modulePrototypes;
    public int mapSize = 20;
    public int sizeOfTile = 42;

    public List<MapModule> generatedModules;

    public List<MapSlot> slots;

    public int debugIterations = 1;

    private void Start()
    {
        GenerateModules();
        InitializeMap();
        for(int i = 0; i < debugIterations; i++)
        {
            GetLowestEntropy()?.Collapse();
        }
        Visualize();
    }

    private void Visualize()
    {
        foreach (MapSlot slot in slots){
            if(slot.possibleModules.Count == 1)
            {
                GameObject module = Instantiate(slot.possibleModules[0].gameObj, slot.position * sizeOfTile, Quaternion.identity);
                module.transform.Rotate(new Vector3(0, 180, slot.possibleModules[0].rotation));
            }
        }
    }

    private void GenerateModules()
    {
        generatedModules = new List<MapModule>();
        foreach (MapModulePrototype prototype in modulePrototypes)
        {
            generatedModules.AddRange(prototype.generateAllModules());
        }
    }

    private void InitializeMap()
    {
        slots = new List<MapSlot>();
        for(int x = 0; x < mapSize; x++)
        {
            for(int y = 0; y < mapSize; y++)
            {
                slots.Add(new MapSlot(generatedModules, x, y));
            }
        }
        // link them all to each other
        foreach(MapSlot slot in slots)
        {
            slot.SetNeighbors(GetNeighbors(slot));
        }
    }

    public MapSlot GetLowestEntropy()
    {
        if (slots.Count == 0) return null;
        List<MapSlot> fuzzy = slots.Where(s => s.possibleModules.Count > 1).ToList();
        if (fuzzy.Count == 0) return null;
        int lowest = fuzzy.Min(s => s.possibleModules.Count);
        List<MapSlot> lowestList = slots.Where(s => s.possibleModules.Count == lowest).ToList();
        return lowestList[Random.Range(0, lowestList.Count)];
    }

    

    // u, r, d, l
    public (MapSlot, MapSlot,  MapSlot, MapSlot) GetNeighbors(MapSlot slot)
    {
        MapSlot up = slots.FirstOrDefault(s => s.position.x == slot.position.x && s.position.y == slot.position.y + 1);
        MapSlot right = slots.FirstOrDefault(s => s.position.x == slot.position.x + 1 && s.position.y == slot.position.y);
        MapSlot down = slots.FirstOrDefault(s => s.position.x == slot.position.x && s.position.y == slot.position.y - 1);
        MapSlot left = slots.FirstOrDefault(s => s.position.x == slot.position.x - 1 && s.position.y == slot.position.y);
        return (up, right, down, left);
    }

}

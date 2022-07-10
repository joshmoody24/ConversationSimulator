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
    public List<MapModulePrototype> mainStreetAllowed;

    public Dictionary<Vector2Int, MapSlot> slots;

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
        foreach (MapSlot slot in slots.Values){
            if(slot.possibleModules.Count == 1)
            {
                GameObject module = Instantiate(slot.possibleModules[0].gameObj, new Vector3(slot.position.x, 0, slot.position.y) * sizeOfTile, Quaternion.identity);
                module.transform.Rotate(new Vector3(-90, 180, slot.possibleModules[0].rotation));
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
        slots = new Dictionary<Vector2Int, MapSlot>();
        for(int x = 0; x < mapSize; x++)
        {
            for(int y = 0; y < mapSize; y++)
            {
                slots[new Vector2Int(x,y)] = new MapSlot(generatedModules, x, y);
            }
        }
        // link them all to each other
        foreach(MapSlot slot in slots.Values)
        {
            slot.SetNeighbors(GetNeighbors(slot));
        }
    }

    public MapSlot GetLowestEntropy()
    {
        if (slots.Count == 0) return null;
        List<MapSlot> fuzzy = slots.Values.Where(s => s.possibleModules.Count > 1).ToList();
        if (fuzzy.Count == 0) return null;
        int lowest = fuzzy.Min(s => s.possibleModules.Count);
        List<MapSlot> lowestList = slots.Values.Where(s => s.possibleModules.Count == lowest).ToList();
        return lowestList[Random.Range(0, lowestList.Count)];
    }

    MapSlot GetSlot(int x, int y)
    {
        MapSlot slot = null;
        slots.TryGetValue(new Vector2Int(x, y), out slot);
        return slot;
    }

    // u, r, d, l
    public (MapSlot, MapSlot,  MapSlot, MapSlot) GetNeighbors(MapSlot slot)
    {
        Vector2Int p = slot.position;
        int x = p.x;
        int y = p.y;
        MapSlot up = GetSlot(x, y+1);
        MapSlot right = GetSlot(x + 1, y);
        MapSlot down = GetSlot(x, y - 1);
        MapSlot left = GetSlot(x - 1, y);
        return (up, right, down, left);
    }
}

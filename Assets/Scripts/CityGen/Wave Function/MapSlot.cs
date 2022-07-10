using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class MapSlot
{
    public List<MapModule> possibleModules;
    public Vector2 position;

    public MapSlot neighborAbove;
    public MapSlot neighborRight;
    public MapSlot neighborBelow;
    public MapSlot neighborLeft;

    public bool collapsed = false;

    public MapSlot(List<MapModule> possibleModules, int x, int y)
    {
        this.position = new Vector2(x, y);
        this.possibleModules = possibleModules;
    }

    public void SetNeighbors((MapSlot, MapSlot, MapSlot, MapSlot) slots)
    {
        neighborAbove = slots.Item1;
        neighborRight = slots.Item2;
        neighborBelow = slots.Item3;
        neighborLeft = slots.Item4;
    }

    public void Collapse()
    {
        MapModule selected = RandomChoice(possibleModules);
        possibleModules = new List<MapModule>() { selected };
        neighborAbove?.Reevaluate();
        neighborRight?.Reevaluate();
        neighborBelow?.Reevaluate();
        neighborLeft?.Reevaluate();
        collapsed = true;
    }

    // returns true if possibilities changed
    public bool Reevaluate()
    {
        if (collapsed) return false;
        // get possible connections
        List<int> topConnectors = neighborAbove?.possibleModules.Select(m => m.downId).Distinct().ToList();
        List<int> rightConnectors = neighborRight?.possibleModules.Select(m => m.leftId).Distinct().ToList();
        List<int> downConnectors = neighborBelow?.possibleModules.Select(m => m.upId).Distinct().ToList();
        List<int> leftConnectors = neighborLeft?.possibleModules.Select(m => m.rightId).Distinct().ToList();

        List<MapModule> newlyPossible = possibleModules
            .Where(m => topConnectors == null || topConnectors.Contains(m.upId))
            .Where(m => rightConnectors == null || rightConnectors.Contains(m.rightId))
            .Where(m => downConnectors == null || downConnectors.Contains(m.downId))
            .Where(m => leftConnectors == null || leftConnectors.Contains(m.leftId))
            .ToList();

        bool changed = newlyPossible.Count < possibleModules.Count;
        possibleModules = newlyPossible;
        if(possibleModules.Count == 1)
        {
            collapsed = true;
        }

        if (changed)
        {
            neighborAbove?.Reevaluate();
            neighborRight?.Reevaluate();
            neighborBelow?.Reevaluate();
            neighborLeft?.Reevaluate();
        }

        return changed;

    }

    private T RandomChoice<T>(List<T> list)
    {
        if (list.Count == 0) throw new System.Exception("Tried to choose from empty list");
        return list[Random.Range(0, list.Count)];
    }
}

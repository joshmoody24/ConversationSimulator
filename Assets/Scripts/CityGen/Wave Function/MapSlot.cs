using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class MapSlot
{
    public List<MapModule> possibleModules;
    public Vector2Int position;

    public MapSlot neighborAbove;
    public MapSlot neighborRight;
    public MapSlot neighborBelow;
    public MapSlot neighborLeft;

    public bool collapsed = false;

    public MapSlot(List<MapModule> possibleModules, int x, int y)
    {
        this.position = new Vector2Int(x, y);
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
        List<MapModule> abovePossibilities = neighborAbove?.possibleModules.ToList();
        List<MapModule> rightPossibilities = neighborRight?.possibleModules.ToList();
        List<MapModule> belowPossibilities = neighborBelow?.possibleModules.ToList();
        List<MapModule> leftPossibilities = neighborLeft?.possibleModules.ToList();

        List<MapModule> newlyPossible = possibleModules
            .Where(m => abovePossibilities == null || abovePossibilities.Where(c => c.CanConnect(m, Direction.Down)).Count() > 0)
            .Where(m => rightPossibilities == null || rightPossibilities.Where(c => c.CanConnect(m, Direction.Left)).Count() > 0)
            .Where(m => belowPossibilities == null || belowPossibilities.Where(c => c.CanConnect(m, Direction.Up)).Count() > 0)
            .Where(m => leftPossibilities == null || leftPossibilities.Where(c => c.CanConnect(m, Direction.Right)).Count() > 0)
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

    public enum Direction { Up, Right, Down, Left };
}

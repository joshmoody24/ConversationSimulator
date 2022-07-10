using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapModule
{
    public GameObject gameObj;
    public MapModulePrototype prototype;

    public Connection up;
    public Connection right;
    public Connection down;
    public Connection left;

    public int rotation;

    public MapModule(MapModulePrototype p, GameObject obj, Connection up, Connection right, Connection down, Connection left, int rotation)
    {
        prototype = p;
        gameObj = obj;
        this.up = up;
        this.right= right;
        this.down = down;
        this.left = left;
        this.rotation = rotation;
    }

    public bool CanConnect(MapModule other, MapSlot.Direction dir)
    {
        switch (dir)
        {
            case MapSlot.Direction.Up:
                return other.down.id == up.id && !up.excluded.Contains(other.prototype);
            case MapSlot.Direction.Right:
                return other.left.id == right.id && !right.excluded.Contains(other.prototype);
            case MapSlot.Direction.Down:
                return other.up.id == down.id && !down.excluded.Contains(other.prototype);
            case MapSlot.Direction.Left:
                return other.right.id == left.id && !left.excluded.Contains(other.prototype);
        }

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapModule
{
    public GameObject gameObj;
    public int upId;
    public int rightId;
    public int downId;
    public int leftId;
    public int rotation;

    public MapModule(GameObject obj, int up, int right, int down, int left, MapModulePrototype.Rotation rotation)
    {
        gameObj = obj;
        upId = up;
        rightId = right;
        downId = down;
        leftId = left;
        switch (rotation)
        {
            case MapModulePrototype.Rotation.Degree0:
                this.rotation = 0;
                break;
            case MapModulePrototype.Rotation.Degree90:
                this.rotation = 90;
                break;
            case MapModulePrototype.Rotation.Degree180:
                this.rotation = 180;
                break;
            case MapModulePrototype.Rotation.Degree270:
                this.rotation = 270;
                break;
        }
    }
}

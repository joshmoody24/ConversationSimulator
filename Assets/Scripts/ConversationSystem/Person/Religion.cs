using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Religion", menuName = "Conversation/Religion", order = 1)]
public class Religion : ScriptableObject
{
    public int numberOfFollowers;
    public float chanceOfLeaving;

    public static List<Religion> AllReligions()
    {
        List<Religion> rList = new List<Religion>();
        string[] assetNames = AssetDatabase.FindAssets("t:Religion");
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            Religion r = AssetDatabase.LoadAssetAtPath<Religion>(SOpath);
            rList.Add(r);
        }
        return rList;
    }
}

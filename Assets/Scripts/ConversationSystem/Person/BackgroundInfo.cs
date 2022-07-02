using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Info", menuName = "Conversation/Background Info", order = 1)]
public class BackgroundInfo : ScriptableObject
{
    public Religion prerequisiteReligion;
    public float likelihood;

    public static List<BackgroundInfo> All()
    {
        List<BackgroundInfo> rList = new List<BackgroundInfo>();
        string[] assetNames = AssetDatabase.FindAssets("t:BackgroundInfo");
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            BackgroundInfo r = AssetDatabase.LoadAssetAtPath<BackgroundInfo>(SOpath);
            rList.Add(r);
        }
        return rList;
    }
}

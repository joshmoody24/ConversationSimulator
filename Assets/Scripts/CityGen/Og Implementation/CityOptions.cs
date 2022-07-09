using UnityEngine;

[CreateAssetMenu(fileName = "CityOptions", menuName = "ProceduralCity/CityOptions", order = 1)]
public class CityOptions : ScriptableObject {
    public float HIGHWAY_WIDTH = 1f;
    public float DEFAULT_WIDTH = 1f;
    public float HIGHWAY_SEGMENT_LENGTH = 10f;
    public float DEFAULT_SEGMENT_LENGTH = 10f;
    [Range(0f, 1f)]
    public float HIGHWAY_BRANCH_PROBABILITY = .05f;
    [Range(0f, 1f)]
    public float DEFAULT_BRANCH_PROBABILITY = 0.2f;
    public float HIGHWAY_TO_STREET_PROBABILITY = 0.25f;

    [Range(0f, 60f)]
    public float BRANCH_ANGLE_VARIATION = 0f;
    [Range(0f, 30f)]
    public float MIN_DEGREE_DIFFERENCE = 5f;

    public float INTERSECTION_RADIUS = 3f;

    public int MAX_ITERATIONS = 10;
}
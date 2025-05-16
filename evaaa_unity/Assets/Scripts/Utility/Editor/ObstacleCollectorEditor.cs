using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObstacleCollector))]
public class ObstacleCollectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObstacleCollector collector = (ObstacleCollector)target;
        if (GUILayout.Button("Collect Obstacles"))
        {
            collector.CollectObstacles();
        }
    }
}
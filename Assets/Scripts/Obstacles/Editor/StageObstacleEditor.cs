using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StagedObstacle))]
public class StageObstacleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        StagedObstacle stagedObstacle = (StagedObstacle)target;
        if (!stagedObstacle.IsMoving)
        {
            if (GUILayout.Button("Go To Next Stage"))
            {
                stagedObstacle.GoToNextStage();
            }
        }
    }
}

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Level Data", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("levelId"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnInterval"));

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("objectsToSpawn"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("obstacles"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponSlots"), true);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        LevelData levelData = (LevelData)target;

        EditorGUILayout.HelpBox(
            $"Objects: {levelData.objectsToSpawn.Count}\n" +
            $"Obstacles: {levelData.obstacles.Count}\n" +
            $"Weapon Slots: {levelData.weaponSlots.Count}",
            MessageType.Info
        );

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Open Level Editor"))
        {
            LevelEditorWindow.Open(levelData);
        }

        if (GUILayout.Button("Save Asset"))
        {
            EditorUtility.SetDirty(levelData);
            AssetDatabase.SaveAssetIfDirty(levelData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Repaint();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            Debug.Log($"Saved LevelData from Inspector: {levelData.name}");
        }

        EditorGUILayout.EndHorizontal();
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }
}
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BulbLevelData))]
public class BulbLevelDataEditor : Editor
{
    private bool[] foldouts;

    private void OnEnable()
    {
        var data = (BulbLevelData)target;
        foldouts = new bool[data.Levels.Count];
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        BulbLevelData data = (BulbLevelData)target;

        // Ensure foldouts array matches the level count
        if (foldouts == null || foldouts.Length != data.Levels.Count)
        {
            foldouts = new bool[data.Levels.Count];
        }

        for (int i = 0; i < data.Levels.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], $"Level {i + 1}", true, EditorStyles.foldoutHeader);

            if (foldouts[i])
            {
                LevelData level = data.Levels[i];

                for (int j = 0; j < level.bulbs.Length; j++)
                {
                    level.bulbs[j] = (BulbState)EditorGUILayout.EnumPopup($"Bulb {j + 1}", level.bulbs[j]);
                }

                if (GUILayout.Button("Remove Level"))
                {
                    data.Levels.RemoveAt(i);
                    break; // Exit loop to avoid index errors
                }
            }

            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Add New Level"))
        {
            data.Levels.Add(new LevelData());
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);
        }
    }
}

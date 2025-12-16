#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapDesign))]
public class MapDesignEditor : Editor
{
    SerializedProperty mapMakeCountProp;
    SerializedProperty middleTilesProp;

    void OnEnable()
    {
        mapMakeCountProp = serializedObject.FindProperty("mapMakeCount");
        middleTilesProp = serializedObject.FindProperty("middleTileConfigs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 1) 기본 인스펙터 그대로 보여주기 (mapMakeCount 포함)
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        // 2) 버튼들
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Sync Tiles"))
            {
                (target as MapDesign)?.SyncMiddleTiles();
                EditorUtility.SetDirty(target);
                serializedObject.Update();
            }

            if (GUILayout.Button("Validate"))
            {
                Validate();
            }
        }

        int mapMakeCount = Mathf.Max(2, mapMakeCountProp.intValue);
        int middleCount = Mathf.Max(0, mapMakeCount - 2);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField($"Middle Tiles: {middleCount} (Editable)", EditorStyles.boldLabel);

        // 3) 사이즈 불일치 경고
        if (middleTilesProp.arraySize != middleCount)
        {
            EditorGUILayout.HelpBox(
                $"middleTileConfigs size({middleTilesProp.arraySize}) != middleCount({middleCount}). " +
                $"Press 'Sync Tiles'.",
                MessageType.Warning
            );
        }

        // 4) 가운데 타일 카드 UI
        int drawCount = Mathf.Min(middleTilesProp.arraySize, middleCount);
        for (int i = 0; i < drawCount; i++)
        {
            DrawTileCard(i);
            EditorGUILayout.Space(6);
        }

        serializedObject.ApplyModifiedProperties();
    }

    void DrawTileCard(int index)
    {
        var tileProp = middleTilesProp.GetArrayElementAtIndex(index);

        var typeProp = tileProp.FindPropertyRelative("contentType");
        var presetProp = tileProp.FindPropertyRelative("monsterPreset");
        var mulProp = tileProp.FindPropertyRelative("monsterCountMultiplier");
        var npcPrefabProp = tileProp.FindPropertyRelative("npcPrefab");
        var npcCountProp = tileProp.FindPropertyRelative("npcCount");

        int tileNumber = index + 2; // 1번=시작, 마지막=보스라 가운데는 2번부터

        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField($"Tile {tileNumber}", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(typeProp, new GUIContent("Content Type"));
            var ct = (TileContentType)typeProp.enumValueIndex;

            if (ct == TileContentType.Monster || ct == TileContentType.MonsterAndNPC)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Monster", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(presetProp, new GUIContent("Preset"));
                EditorGUILayout.PropertyField(mulProp, new GUIContent("Count Multiplier"));

                var preset = presetProp.objectReferenceValue as MonsterSpawnPreset;
                if (preset != null)
                {
                    int baseCount = preset.GetTotalCount();
                    int mul = Mathf.Max(1, mulProp.intValue);
                    EditorGUILayout.LabelField($"Total: {baseCount} x {mul} = {baseCount * mul}");
                }
            }

            if (ct == TileContentType.NPC || ct == TileContentType.MonsterAndNPC)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("NPC", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(npcPrefabProp, new GUIContent("NPC Prefab"));

                //EditorGUILayout.PropertyField(npcCountProp, new GUIContent("NPC Count"));
            }

            if (ct == TileContentType.None)
                EditorGUILayout.HelpBox("This tile spawns nothing.", MessageType.Info);
        }
    }

    void Validate()
    {
        int errors = 0;

        for (int i = 0; i < middleTilesProp.arraySize; i++)
        {
            var tileProp = middleTilesProp.GetArrayElementAtIndex(i);
            var ct = (TileContentType)tileProp.FindPropertyRelative("contentType").enumValueIndex;

            var preset = tileProp.FindPropertyRelative("monsterPreset").objectReferenceValue as MonsterSpawnPreset;
            var npcPrefab = tileProp.FindPropertyRelative("npcPrefab").objectReferenceValue as GameObject;

            int tileNumber = i + 2;

            if ((ct == TileContentType.Monster || ct == TileContentType.MonsterAndNPC) && preset == null)
            {
                Debug.LogWarning($"[Validate] Tile {tileNumber}: Monster인데 Preset이 비어있음", target);
                errors++;
            }

            if ((ct == TileContentType.NPC || ct == TileContentType.MonsterAndNPC) && npcPrefab == null)
            {
                Debug.LogWarning($"[Validate] Tile {tileNumber}: NPC인데 NPC Prefab이 비어있음", target);
                errors++;
            }
        }

        if (errors != 0) Debug.LogWarning($"[Validate] 경고 {errors}개", target);
    }
}
#endif

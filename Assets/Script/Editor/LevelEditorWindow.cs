using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private enum PlacementMode
    {
        None,
        SpawnObject,
        Obstacle,
        WeaponSlot
    }

    private enum SelectedElementType
    {
        None,
        SpawnObject,
        Obstacle,
        WeaponSlot
    }

    private LevelData currentLevel;
    private SerializedObject serializedLevel;

    private PlacementMode placementMode = PlacementMode.None;

    private bool useGridSnap = true;
    private float gridSize = 0.5f;

    // Template for SpawnObject
    private string objectId = "Object";
    private PixelShapeData shape;
    private MaterialData materialData;
    private float scale = 1f;
    private float rotationZ = 0f;
    private bool destructible = true;
    private int xpPerPixel = 1;

    // Template for Obstacle
    private Vector2 obstacleSize = Vector2.one;
    private Sprite obstacleSprite;

    // Selection
    private SelectedElementType selectedType = SelectedElementType.None;
    private int selectedIndex = -1;

    private const float PICK_DISTANCE = 0.35f;

    [MenuItem("Tools/Pixel Destruction/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    public static void Open(LevelData levelData)
    {
        LevelEditorWindow window = GetWindow<LevelEditorWindow>("Level Editor");
        window.SetLevel(levelData);
        window.Show();
        window.Focus();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        RefreshSerializedObject();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void SetLevel(LevelData levelData)
    {
        currentLevel = levelData;
        RefreshSerializedObject();
        Repaint();
        SceneView.RepaintAll();
    }

    private void RefreshSerializedObject()
    {
        serializedLevel = currentLevel != null ? new SerializedObject(currentLevel) : null;
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        LevelData newLevel = (LevelData)EditorGUILayout.ObjectField("Level Data", currentLevel, typeof(LevelData), false);
        if (EditorGUI.EndChangeCheck())
        {
            SetLevel(newLevel);
        }

        if (currentLevel == null)
        {
            EditorGUILayout.HelpBox("Please assign a LevelData asset.", MessageType.Warning);

            if (GUILayout.Button("Create New LevelData"))
            {
                CreateNewLevelData();
            }
            return;
        }

        if (serializedLevel == null || serializedLevel.targetObject != currentLevel)
        {
            RefreshSerializedObject();
        }

        serializedLevel.Update();

        EditorGUILayout.Space();
        DrawLevelInfoSection();

        EditorGUILayout.Space();
        DrawPlacementSection();

        EditorGUILayout.Space();
        DrawSelectionSection();

        EditorGUILayout.Space();
        DrawBottomButtons();

        serializedLevel.ApplyModifiedProperties();
    }

    private void DrawLevelInfoSection()
    {
        EditorGUILayout.LabelField("Level Info", EditorStyles.boldLabel);

        SerializedProperty levelIdProp = serializedLevel.FindProperty("levelId");
        SerializedProperty spawnIntervalProp = serializedLevel.FindProperty("spawnInterval");
        SerializedProperty objectsProp = serializedLevel.FindProperty("objectsToSpawn");
        SerializedProperty obstaclesProp = serializedLevel.FindProperty("obstacles");
        SerializedProperty weaponSlotsProp = serializedLevel.FindProperty("weaponSlots");

        EditorGUILayout.PropertyField(levelIdProp);
        EditorGUILayout.PropertyField(spawnIntervalProp);

        EditorGUILayout.HelpBox(
            $"Objects: {objectsProp.arraySize}\n" +
            $"Obstacles: {obstaclesProp.arraySize}\n" +
            $"Weapon Slots: {weaponSlotsProp.arraySize}",
            MessageType.Info
        );

        useGridSnap = EditorGUILayout.Toggle("Use Grid Snap", useGridSnap);
        using (new EditorGUI.DisabledScope(!useGridSnap))
        {
            gridSize = Mathf.Max(0.01f, EditorGUILayout.FloatField("Grid Size", gridSize));
        }
    }

    private void DrawPlacementSection()
    {
        EditorGUILayout.LabelField("Placement", EditorStyles.boldLabel);

        placementMode = (PlacementMode)EditorGUILayout.EnumPopup("Placement Mode", placementMode);

        switch (placementMode)
        {
            case PlacementMode.None:
                EditorGUILayout.HelpBox("Choose a placement mode. Use Shift + Left Click in Scene to place.", MessageType.None);
                break;

            case PlacementMode.SpawnObject:
                objectId = EditorGUILayout.TextField("Object Id", objectId);
                shape = (PixelShapeData)EditorGUILayout.ObjectField("Shape", shape, typeof(PixelShapeData), false);
                materialData = (MaterialData)EditorGUILayout.ObjectField("Material", materialData, typeof(MaterialData), false);
                scale = Mathf.Max(0.01f, EditorGUILayout.FloatField("Scale", scale));
                rotationZ = EditorGUILayout.FloatField("Rotation Z", rotationZ);
                destructible = EditorGUILayout.Toggle("Destructible", destructible);
                xpPerPixel = Mathf.Max(0, EditorGUILayout.IntField("XP Per Pixel", xpPerPixel));
                break;

            case PlacementMode.Obstacle:
                obstacleSize = EditorGUILayout.Vector2Field("Size", obstacleSize);
                obstacleSize.x = Mathf.Max(0.01f, obstacleSize.x);
                obstacleSize.y = Mathf.Max(0.01f, obstacleSize.y);
                obstacleSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", obstacleSprite, typeof(Sprite), false);
                break;

            case PlacementMode.WeaponSlot:
                EditorGUILayout.HelpBox("WeaponSlot only needs position. Use Shift + Left Click in Scene to place.", MessageType.None);
                break;
        }
    }

    private void DrawSelectionSection()
    {
        EditorGUILayout.LabelField("Selection", EditorStyles.boldLabel);

        if (selectedType == SelectedElementType.None || selectedIndex < 0)
        {
            EditorGUILayout.HelpBox("No element selected. Use Ctrl + Left Click near an element in Scene to select.", MessageType.None);
            return;
        }

        SerializedProperty targetElement = GetSelectedElementProperty();
        if (targetElement == null)
        {
            EditorGUILayout.HelpBox("Selected element is invalid.", MessageType.Warning);
            ClearSelection();
            return;
        }

        EditorGUILayout.LabelField("Selected Type", selectedType.ToString());
        EditorGUILayout.LabelField("Selected Index", selectedIndex.ToString());

        switch (selectedType)
        {
            case SelectedElementType.SpawnObject:
                DrawSelectedSpawnObject(targetElement);
                break;

            case SelectedElementType.Obstacle:
                DrawSelectedObstacle(targetElement);
                break;

            case SelectedElementType.WeaponSlot:
                DrawSelectedWeaponSlot(targetElement);
                break;
        }

        GUILayout.Space(8);

        if (GUILayout.Button("Delete Selected"))
        {
            DeleteSelected();
        }
    }

    private void DrawSelectedSpawnObject(SerializedProperty element)
    {
        EditorGUILayout.PropertyField(element.FindPropertyRelative("objectId"));
        EditorGUILayout.PropertyField(element.FindPropertyRelative("shape"));
        EditorGUILayout.PropertyField(element.FindPropertyRelative("materialData"));
        EditorGUILayout.PropertyField(element.FindPropertyRelative("spawnPosition"));
        EditorGUILayout.PropertyField(element.FindPropertyRelative("scale"));
        EditorGUILayout.PropertyField(element.FindPropertyRelative("rotationZ"));
        EditorGUILayout.PropertyField(element.FindPropertyRelative("destructible"));
        EditorGUILayout.PropertyField(element.FindPropertyRelative("xpPerPixel"));

        if (useGridSnap)
        {
            SerializedProperty posProp = element.FindPropertyRelative("spawnPosition");
            posProp.vector2Value = ApplySnap(posProp.vector2Value);
        }
    }

    private void DrawSelectedObstacle(SerializedProperty element)
    {
        EditorGUILayout.PropertyField(element.FindPropertyRelative("position"));
        EditorGUILayout.PropertyField(element.FindPropertyRelative("size"));
        EditorGUILayout.PropertyField(element.FindPropertyRelative("sprite"));

        if (useGridSnap)
        {
            SerializedProperty posProp = element.FindPropertyRelative("position");
            posProp.vector2Value = ApplySnap(posProp.vector2Value);
        }

        SerializedProperty sizeProp = element.FindPropertyRelative("size");
        Vector2 size = sizeProp.vector2Value;
        size.x = Mathf.Max(0.01f, size.x);
        size.y = Mathf.Max(0.01f, size.y);
        sizeProp.vector2Value = size;
    }

    private void DrawSelectedWeaponSlot(SerializedProperty element)
    {
        EditorGUILayout.PropertyField(element.FindPropertyRelative("position"));

        if (useGridSnap)
        {
            SerializedProperty posProp = element.FindPropertyRelative("position");
            posProp.vector2Value = ApplySnap(posProp.vector2Value);
        }
    }

    private void DrawBottomButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear All"))
        {
            if (EditorUtility.DisplayDialog("Clear All", "Delete all objects, obstacles, and weapon slots in this level?", "Yes", "No"))
            {
                Undo.RecordObject(currentLevel, "Clear Level Data");

                serializedLevel.FindProperty("objectsToSpawn").ClearArray();
                serializedLevel.FindProperty("obstacles").ClearArray();
                serializedLevel.FindProperty("weaponSlots").ClearArray();

                serializedLevel.ApplyModifiedProperties();
                ClearSelection();
                MarkDirtyAndRefresh();
            }
        }

        if (GUILayout.Button("Save Asset"))
        {
            SaveCurrentLevel();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (currentLevel == null) return;
        if (serializedLevel == null || serializedLevel.targetObject != currentLevel)
            RefreshSerializedObject();

        Event e = Event.current;
        if (e == null) return;

        serializedLevel.Update();

        DrawSceneOverlay();
        DrawScenePreview();
        DrawSelectedHandle();
        HandleSceneInput(e);

        serializedLevel.ApplyModifiedProperties();
    }

    private void DrawSceneOverlay()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, 10, 290, 115), "Level Editor", GUI.skin.window);
        GUILayout.Label("Shift + Left Click: place");
        GUILayout.Label("Ctrl + Left Click: select nearest");
        GUILayout.Label($"Selected: {selectedType}" + (selectedIndex >= 0 ? $" [{selectedIndex}]" : ""));
        GUILayout.Label($"Mode: {placementMode}");
        GUILayout.Label(useGridSnap ? $"Grid Snap: ON ({gridSize})" : "Grid Snap: OFF");
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private void DrawScenePreview()
    {
        SerializedProperty objectsProp = serializedLevel.FindProperty("objectsToSpawn");
        SerializedProperty obstaclesProp = serializedLevel.FindProperty("obstacles");
        SerializedProperty weaponSlotsProp = serializedLevel.FindProperty("weaponSlots");

        for (int i = 0; i < objectsProp.arraySize; i++)
        {
            SerializedProperty element = objectsProp.GetArrayElementAtIndex(i);
            Vector2 pos = element.FindPropertyRelative("spawnPosition").vector2Value;
            float itemScale = Mathf.Max(0.01f, element.FindPropertyRelative("scale").floatValue);
            string id = element.FindPropertyRelative("objectId").stringValue;

            bool isSelected = selectedType == SelectedElementType.SpawnObject && selectedIndex == i;
            Handles.color = isSelected ? Color.yellow : Color.green;
            Handles.DrawWireDisc(pos, Vector3.forward, Mathf.Max(0.15f, itemScale * 0.25f));
            Handles.Label(pos + Vector2.up * 0.2f, $"Obj {i}: {id}");
        }

        for (int i = 0; i < obstaclesProp.arraySize; i++)
        {
            SerializedProperty element = obstaclesProp.GetArrayElementAtIndex(i);
            Vector2 pos = element.FindPropertyRelative("position").vector2Value;
            Vector2 size = element.FindPropertyRelative("size").vector2Value;

            bool isSelected = selectedType == SelectedElementType.Obstacle && selectedIndex == i;
            Handles.color = isSelected ? Color.yellow : Color.red;
            Handles.DrawWireCube(pos, new Vector3(size.x, size.y, 0f));
            Handles.Label(pos + Vector2.up * (size.y * 0.5f + 0.1f), $"Obs {i}");
        }

        for (int i = 0; i < weaponSlotsProp.arraySize; i++)
        {
            SerializedProperty element = weaponSlotsProp.GetArrayElementAtIndex(i);
            Vector2 pos = element.FindPropertyRelative("position").vector2Value;

            bool isSelected = selectedType == SelectedElementType.WeaponSlot && selectedIndex == i;
            Handles.color = isSelected ? Color.yellow : Color.cyan;
            Handles.DrawWireDisc(pos, Vector3.forward, 0.2f);
            Handles.DrawSolidDisc(pos, Vector3.forward, 0.03f);
            Handles.Label(pos + Vector2.up * 0.2f, $"Slot {i}");
        }
    }

    private void DrawSelectedHandle()
    {
        SerializedProperty element = GetSelectedElementProperty();
        if (element == null) return;

        EditorGUI.BeginChangeCheck();

        switch (selectedType)
        {
            case SelectedElementType.SpawnObject:
                {
                    SerializedProperty posProp = element.FindPropertyRelative("spawnPosition");
                    Vector3 newPos = Handles.PositionHandle(posProp.vector2Value, Quaternion.identity);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(currentLevel, "Move Spawn Object");
                        posProp.vector2Value = ApplySnap(newPos);
                        MarkDirtyAndRefresh();
                    }
                    break;
                }

            case SelectedElementType.Obstacle:
                {
                    SerializedProperty posProp = element.FindPropertyRelative("position");
                    Vector3 newPos = Handles.PositionHandle(posProp.vector2Value, Quaternion.identity);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(currentLevel, "Move Obstacle");
                        posProp.vector2Value = ApplySnap(newPos);
                        MarkDirtyAndRefresh();
                    }
                    break;
                }

            case SelectedElementType.WeaponSlot:
                {
                    SerializedProperty posProp = element.FindPropertyRelative("position");
                    Vector3 newPos = Handles.PositionHandle(posProp.vector2Value, Quaternion.identity);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(currentLevel, "Move Weapon Slot");
                        posProp.vector2Value = ApplySnap(newPos);
                        MarkDirtyAndRefresh();
                    }
                    break;
                }
        }
    }

    private void HandleSceneInput(Event e)
    {
        if (e.type != EventType.MouseDown || e.button != 0)
            return;

        Vector2 worldPos = ScreenToWorld(e.mousePosition);

        if (e.shift)
        {
            PlaceElement(worldPos);
            e.Use();
        }
        else if (e.control)
        {
            SelectNearest(worldPos);
            e.Use();
        }
    }

    private void PlaceElement(Vector2 worldPos)
    {
        if (currentLevel == null) return;
        if (serializedLevel == null) RefreshSerializedObject();

        Undo.RecordObject(currentLevel, "Place Level Element");

        worldPos = ApplySnap(worldPos);

        switch (placementMode)
        {
            case PlacementMode.SpawnObject:
                {
                    SerializedProperty objectsProp = serializedLevel.FindProperty("objectsToSpawn");
                    int index = objectsProp.arraySize;
                    objectsProp.InsertArrayElementAtIndex(index);

                    SerializedProperty element = objectsProp.GetArrayElementAtIndex(index);
                    element.FindPropertyRelative("objectId").stringValue = objectId;
                    element.FindPropertyRelative("shape").objectReferenceValue = shape;
                    element.FindPropertyRelative("materialData").objectReferenceValue = materialData;
                    element.FindPropertyRelative("spawnPosition").vector2Value = worldPos;
                    element.FindPropertyRelative("scale").floatValue = scale;
                    element.FindPropertyRelative("rotationZ").floatValue = rotationZ;
                    element.FindPropertyRelative("destructible").boolValue = destructible;
                    element.FindPropertyRelative("xpPerPixel").intValue = xpPerPixel;

                    selectedType = SelectedElementType.SpawnObject;
                    selectedIndex = index;
                    break;
                }

            case PlacementMode.Obstacle:
                {
                    SerializedProperty obstaclesProp = serializedLevel.FindProperty("obstacles");
                    int index = obstaclesProp.arraySize;
                    obstaclesProp.InsertArrayElementAtIndex(index);

                    SerializedProperty element = obstaclesProp.GetArrayElementAtIndex(index);
                    element.FindPropertyRelative("position").vector2Value = worldPos;
                    element.FindPropertyRelative("size").vector2Value = obstacleSize;
                    element.FindPropertyRelative("sprite").objectReferenceValue = obstacleSprite;

                    selectedType = SelectedElementType.Obstacle;
                    selectedIndex = index;
                    break;
                }

            case PlacementMode.WeaponSlot:
                {
                    SerializedProperty weaponSlotsProp = serializedLevel.FindProperty("weaponSlots");
                    int index = weaponSlotsProp.arraySize;
                    weaponSlotsProp.InsertArrayElementAtIndex(index);

                    SerializedProperty element = weaponSlotsProp.GetArrayElementAtIndex(index);
                    element.FindPropertyRelative("position").vector2Value = worldPos;

                    selectedType = SelectedElementType.WeaponSlot;
                    selectedIndex = index;
                    break;
                }

            case PlacementMode.None:
                return;
        }

        serializedLevel.ApplyModifiedProperties();
        MarkDirtyAndRefresh();
    }

    private void SelectNearest(Vector2 worldPos)
    {
        float bestDistance = float.MaxValue;
        SelectedElementType bestType = SelectedElementType.None;
        int bestIndex = -1;

        SerializedProperty objectsProp = serializedLevel.FindProperty("objectsToSpawn");
        SerializedProperty obstaclesProp = serializedLevel.FindProperty("obstacles");
        SerializedProperty weaponSlotsProp = serializedLevel.FindProperty("weaponSlots");

        for (int i = 0; i < objectsProp.arraySize; i++)
        {
            Vector2 pos = objectsProp.GetArrayElementAtIndex(i).FindPropertyRelative("spawnPosition").vector2Value;
            float d = Vector2.Distance(worldPos, pos);
            if (d < bestDistance && d <= PICK_DISTANCE)
            {
                bestDistance = d;
                bestType = SelectedElementType.SpawnObject;
                bestIndex = i;
            }
        }

        for (int i = 0; i < obstaclesProp.arraySize; i++)
        {
            SerializedProperty element = obstaclesProp.GetArrayElementAtIndex(i);
            Vector2 pos = element.FindPropertyRelative("position").vector2Value;
            Vector2 size = element.FindPropertyRelative("size").vector2Value;

            float d = DistanceToRect(worldPos, pos, size);
            if (d < bestDistance && d <= PICK_DISTANCE)
            {
                bestDistance = d;
                bestType = SelectedElementType.Obstacle;
                bestIndex = i;
            }
        }

        for (int i = 0; i < weaponSlotsProp.arraySize; i++)
        {
            Vector2 pos = weaponSlotsProp.GetArrayElementAtIndex(i).FindPropertyRelative("position").vector2Value;
            float d = Vector2.Distance(worldPos, pos);
            if (d < bestDistance && d <= PICK_DISTANCE)
            {
                bestDistance = d;
                bestType = SelectedElementType.WeaponSlot;
                bestIndex = i;
            }
        }

        selectedType = bestType;
        selectedIndex = bestIndex;

        Repaint();
        SceneView.RepaintAll();
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    private void DeleteSelected()
    {
        if (currentLevel == null || selectedType == SelectedElementType.None || selectedIndex < 0)
            return;

        Undo.RecordObject(currentLevel, "Delete Selected Element");

        switch (selectedType)
        {
            case SelectedElementType.SpawnObject:
                {
                    SerializedProperty prop = serializedLevel.FindProperty("objectsToSpawn");
                    if (selectedIndex >= 0 && selectedIndex < prop.arraySize)
                        prop.DeleteArrayElementAtIndex(selectedIndex);
                    break;
                }

            case SelectedElementType.Obstacle:
                {
                    SerializedProperty prop = serializedLevel.FindProperty("obstacles");
                    if (selectedIndex >= 0 && selectedIndex < prop.arraySize)
                        prop.DeleteArrayElementAtIndex(selectedIndex);
                    break;
                }

            case SelectedElementType.WeaponSlot:
                {
                    SerializedProperty prop = serializedLevel.FindProperty("weaponSlots");
                    if (selectedIndex >= 0 && selectedIndex < prop.arraySize)
                        prop.DeleteArrayElementAtIndex(selectedIndex);
                    break;
                }
        }

        serializedLevel.ApplyModifiedProperties();
        ClearSelection();
        MarkDirtyAndRefresh();
    }

    private SerializedProperty GetSelectedElementProperty()
    {
        if (serializedLevel == null || currentLevel == null || selectedIndex < 0)
            return null;

        switch (selectedType)
        {
            case SelectedElementType.SpawnObject:
                {
                    SerializedProperty prop = serializedLevel.FindProperty("objectsToSpawn");
                    return selectedIndex < prop.arraySize ? prop.GetArrayElementAtIndex(selectedIndex) : null;
                }

            case SelectedElementType.Obstacle:
                {
                    SerializedProperty prop = serializedLevel.FindProperty("obstacles");
                    return selectedIndex < prop.arraySize ? prop.GetArrayElementAtIndex(selectedIndex) : null;
                }

            case SelectedElementType.WeaponSlot:
                {
                    SerializedProperty prop = serializedLevel.FindProperty("weaponSlots");
                    return selectedIndex < prop.arraySize ? prop.GetArrayElementAtIndex(selectedIndex) : null;
                }
        }

        return null;
    }

    private void SaveCurrentLevel()
    {
        if (currentLevel == null) return;

        serializedLevel.ApplyModifiedProperties();
        EditorUtility.SetDirty(currentLevel);
        AssetDatabase.SaveAssetIfDirty(currentLevel);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = currentLevel;
        EditorUtility.FocusProjectWindow();
        Repaint();
        SceneView.RepaintAll();
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

        ShowNotification(new GUIContent($"Saved: {currentLevel.name}"));
        Debug.Log($"Saved LevelData: {currentLevel.name}");
    }

    private void MarkDirtyAndRefresh()
    {
        if (currentLevel == null) return;

        EditorUtility.SetDirty(currentLevel);
        Repaint();
        SceneView.RepaintAll();
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    private void CreateNewLevelData()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create LevelData",
            "NewLevelData",
            "asset",
            "Choose where to save the LevelData asset."
        );

        if (string.IsNullOrEmpty(path)) return;

        LevelData asset = CreateInstance<LevelData>();
        asset.levelId = System.IO.Path.GetFileNameWithoutExtension(path);

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        SetLevel(asset);
        Selection.activeObject = asset;
    }

    private void ClearSelection()
    {
        selectedType = SelectedElementType.None;
        selectedIndex = -1;
    }

    private Vector2 ScreenToWorld(Vector2 mousePosition)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        return ray.origin;
    }

    private Vector2 ApplySnap(Vector2 position)
    {
        if (!useGridSnap) return position;

        return new Vector2(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize
        );
    }

    private float DistanceToRect(Vector2 point, Vector2 rectCenter, Vector2 rectSize)
    {
        Vector2 half = rectSize * 0.5f;
        float dx = Mathf.Max(Mathf.Abs(point.x - rectCenter.x) - half.x, 0f);
        float dy = Mathf.Max(Mathf.Abs(point.y - rectCenter.y) - half.y, 0f);
        return Mathf.Sqrt(dx * dx + dy * dy);
    }
}
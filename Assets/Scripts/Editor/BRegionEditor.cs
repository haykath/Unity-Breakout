using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BlockRegion))]
public class BRegionEditor : Editor
{
    RectTransform rectTransform;

    bool initializeChangeTest = false;
    Vector3 lastScale;
    Vector2 lastDimensions;

    public override void OnInspectorGUI()
    {
        if (!rectTransform)
        {
            if (!UpdateRectTransform())
            {
                EditorGUILayout.LabelField("Please attach a RectTransform component.");
                return;
            }
        }
        BlockRegion t = (BlockRegion)target;
        DrawDefaultInspector();
        EditorGUILayout.LabelField("Columns", t.columnCount.ToString());
        EditorGUILayout.LabelField("Lines", t.lineCount.ToString());

        EditorGUILayout.Separator();
        if (GUILayout.Button("Recalculate Blocks") || GUI.changed)
        {
            t.Recalculate();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

    }

    void OnSceneGUI()
    {
        if (!rectTransform)
        {
            if (!UpdateRectTransform())
            {
                EditorGUILayout.LabelField("Please attach a RectTransform component.");
                return;
            }
        }

        if (SceneViewChanged())
        {
            BlockRegion t = (BlockRegion)target;
            if (t.blockCount > 0)
            {
                t.Recalculate();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }
    }

    bool UpdateRectTransform()
    {
        BlockRegion t = (BlockRegion) target;
        rectTransform = t.GetComponent<RectTransform>();
        if (!rectTransform)
        {
            throw new System.NullReferenceException("Block region " + t.name + " does not have a RectTransform component attached.");
        }

        return true;
    }

    bool SceneViewChanged()
    {
        if (!initializeChangeTest)
        {
            lastScale = rectTransform.localScale;
            lastDimensions = rectTransform.sizeDelta;
            initializeChangeTest = true;
        }

        if (
        lastScale != rectTransform.localScale ||
        lastDimensions != rectTransform.sizeDelta)
            return true;

        return false;
    }

}

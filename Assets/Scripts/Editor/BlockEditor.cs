using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUI.changed)
        {
            Block tBlock = (Block) target;
            if (!tBlock.spRenderer)
                tBlock.spRenderer = tBlock.GetComponent<SpriteRenderer>();
            tBlock.UpdateSprite();
        }
    }
}

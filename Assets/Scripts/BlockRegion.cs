using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class BlockRegion : MonoBehaviour
{
    public enum BlockRegionMode { SINGLE_BLOCK, BLOCK_LIST };

    public BlockRegionMode regionType;
    public int blockCount;
    public List<GameObject> blockList;
    private List<GameObject> orderedBlockList;
    public GameObject blockPrefab;
    public Vector2 spacing;
    public Vector2 offset;
    public Vector2 blockScale = 2 * Vector2.one;
    public bool flipX = false;
    public bool flipY = false;

    private RectTransform rectTransform;
    public int currentBlockCount;

    private int lnCt = -1;
    private int clCt = -1;

    [HideInInspector]
    public int lineCount { get { return lnCt; } }
    [HideInInspector]
    public int columnCount { get { return clCt; } }

    void Awake()
    {
        orderedBlockList = new List<GameObject>();
        foreach (var v in blockList) orderedBlockList.Add(v);
        rectTransform = GetComponent<RectTransform>();
        currentBlockCount = blockCount;
        Recalculate();
    }

    public void Recalculate()
    {
        switch (regionType)
        {
            case BlockRegionMode.SINGLE_BLOCK:
                RecalculateSingleBlock();
                break;
            case BlockRegionMode.BLOCK_LIST:
                if (orderedBlockList == null)
                    orderedBlockList = new List<GameObject>();
                HandleFlips();
                RecalculateBlockList();
                break;
            default:
                throw new System.NotImplementedException("Block region type not implemented");
        }
    }

    #region Recalculate Functions

    public void RecalculateSingleBlock()
    {
        if (!blockPrefab) Debug.LogError("Block region " + name + " does not have a block prefab attached.");

        if (blockList == null)
        {
            blockList = new List<GameObject>();
        }
        foreach (Transform t in transform) blockList.Add(t.gameObject);
        blockList.ForEach(t => DestroyImmediate(t));
        blockList.Clear();


        float lineX = offset.x - rectTransform.sizeDelta.x / 2f;
        float lineY = offset.y - rectTransform.sizeDelta.y / 2f;
        bool colCt = true; clCt = 0;
        lnCt = 1;

        for (int i = 0; i < blockCount; i++)
        {
#if UNITY_EDITOR
            GameObject b = PrefabUtility.InstantiatePrefab(blockPrefab) as GameObject;
#else
            GameObject b = Instantiate(blockPrefab) as GameObject;
#endif
            SpriteRenderer sr = b.GetComponent<SpriteRenderer>();
            if (!b || !sr)
            {
                Debug.LogError("Block prefab is not a block");
                return;
            }

            b.transform.SetParent(transform);
            b.transform.localScale = new Vector3(blockScale.x, blockScale.y, 1);
            b.name = b.name + string.Format("({0})", transform.childCount);

            if (lineX + sr.bounds.size.x > rectTransform.sizeDelta.x / 2f)
            {
                lineX = offset.x - rectTransform.sizeDelta.x / 2f;
                lineY += spacing.y + sr.bounds.size.y;
                colCt = false;
                if (lineY + sr.bounds.size.y > rectTransform.sizeDelta.y / 2f)
                {
                    Debug.LogWarning("Block region " + name + " is too small for " + blockCount.ToString() + " blocks.");
                    DestroyImmediate(b);
                    break;
                }

                lnCt++;
            }
            if (colCt) clCt++;

            b.transform.localPosition = new Vector3(lineX + sr.bounds.extents.x, -lineY - sr.bounds.extents.y);
            lineX += sr.bounds.size.x + spacing.x;
            b.transform.rotation = transform.rotation;
            blockList.Add(b);
        }
    }

    public void RecalculateBlockList()
    {
        if (blockList == null)
        {
            blockList = new List<GameObject>();
            return;
        }

        float lineX = offset.x - rectTransform.sizeDelta.x / 2f;
        float lineY = offset.y - rectTransform.sizeDelta.y / 2f;
        bool colCt = true; clCt = 0;
        bool lnCt = true; this.lnCt = 1;
        foreach (GameObject b in orderedBlockList)
        {
            SpriteRenderer sr = b.GetComponent<SpriteRenderer>();
            if (b.GetComponent<Block>() == null || !sr)
            {
                Debug.LogError("Block prefab is not a block");
                return;
            }

            b.transform.SetParent(transform);
            b.transform.localScale = new Vector3(blockScale.x, blockScale.y, 1);
            b.name = string.Format("Block ({0})", blockList.IndexOf(b) + 1);

            if (lineX + sr.bounds.size.x > rectTransform.sizeDelta.x / 2f)
            {
                lineX = offset.x - rectTransform.sizeDelta.x / 2f;
                lineY += spacing.y + sr.bounds.size.y;
                colCt = false;
                if (lineY + sr.bounds.size.y > rectTransform.sizeDelta.y / 2f)
                {
                    lnCt = false;
                    Debug.LogWarning("Block region " + name + " is too small. Unparenting block: " + b.name);
                    b.transform.SetParent(null);
                    b.transform.position = Vector2.zero;
                }

                if (lnCt) this.lnCt++;
            }
            if (colCt) clCt++;
            b.transform.localPosition = new Vector3(lineX + sr.bounds.extents.x, -lineY - sr.bounds.extents.y);
            lineX += sr.bounds.size.x + spacing.x;
            b.transform.rotation = transform.rotation;
        }

        blockCount = blockList.Count;
    }

    void HandleFlips()
    {

        if (flipX == flipY)
        {
            orderedBlockList.Clear();
            foreach (var v in blockList) orderedBlockList.Add(v);
            if (flipX == true) orderedBlockList.Reverse();
            return;
        }

        RecalculateColLnCount();
        orderedBlockList.Clear();

        for (int i = 0; i < blockList.Count; i++)
        {
            int index = (i / clCt) * clCt + ((clCt - 1) - (i % clCt));
            if (index >= blockList.Count)
                continue;
            orderedBlockList.Add(blockList[index]);
        }

        if (flipY) orderedBlockList.Reverse();
    }

    void RecalculateColLnCount()
    {
        if (blockList == null)
        {
            this.lnCt = 0;
            clCt = 0;
            return;
        }
        if(blockList.Count == 0)
        {
            this.lnCt = 0;
            clCt = 0;
        }

        float lineX = offset.x - rectTransform.sizeDelta.x / 2f;
        float lineY = offset.y - rectTransform.sizeDelta.y / 2f;
        bool colCt = true; clCt = 0;
        bool lnCt = true; this.lnCt = 1;
        foreach (GameObject b in blockList)
        {
            SpriteRenderer sr = b.GetComponent<SpriteRenderer>();
            if (b.GetComponent<Block>() == null || !sr)
                continue;

            if (lineX + sr.bounds.size.x > rectTransform.sizeDelta.x / 2f)
            {
                lineX = offset.x - rectTransform.sizeDelta.x / 2f;
                lineY += spacing.y + sr.bounds.size.y;
                colCt = false;
                if (lineY + sr.bounds.size.y > rectTransform.sizeDelta.y / 2f)
                    break;

                if (lnCt) this.lnCt++;
            }
            if (colCt) clCt++;
            lineX += sr.bounds.size.x + spacing.x;
        }
        blockCount = blockList.Count;
    }

#endregion
}

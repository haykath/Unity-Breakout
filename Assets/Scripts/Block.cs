using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Block : MonoBehaviour
{

    public enum PowerUpType { NONE, EXTRABALL, SPEED }

    public int life = 3;
    public int value = 0;
    public Color[] spriteColors;

    [HideInInspector]
    public SpriteRenderer spRenderer;
    private Renderer lightSpr;

    private Light backLight;
    private BlockRegion parentRegion;

    // Use this for initialization
    void Awake()
    {
        parentRegion = transform.GetComponentInParent<BlockRegion>();

        spRenderer = GetComponent<SpriteRenderer>();
        spRenderer.receiveShadows = true;
        spRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        spRenderer.useLightProbes = true;

        lightSpr = GetComponentsInChildren<Renderer>()[1];
    }

    void Start()
    {
        GameManager.AddBlock();
    }

    // Update is called once per frame
    public void Update()
    {
        if (life > 0)
        {
            UpdateSprite();
        }
        else
        {
            BlockDisable();
        }
    }

    //Collision handling
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ball")
        {
            col.gameObject.GetComponent<BallController>().Hit(transform.position, spRenderer.bounds.extents);
            life--;
        }
    }
    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ball")
        {
            col.gameObject.GetComponent<BallController>().Hit(transform.position, spRenderer.bounds.extents);
        }
    }

    public void UpdateSprite()
    {
        if (life <= spriteColors.Length)
        {
            Color c = spriteColors[Mathf.Max(0, life - 1)];
            spRenderer.color = c;
            if (Application.isPlaying)
            {
                spRenderer.material.SetColor("_EmissionColor", c);
                lightSpr = GetComponentsInChildren<Renderer>()[1];
                lightSpr.material.SetColor("_TintColor", new Color(c.r, c.g, c.b, lightSpr.material.GetColor("_TintColor").a));
            }
        }
    }

    public void SetParentRegion(BlockRegion region)
    {
        parentRegion = region;
    }

    void BlockDisable()
    {
        GameManager.RemoveBlock(value);
        if (parentRegion) parentRegion.currentBlockCount--;
        gameObject.SetActive(false);
    }
}

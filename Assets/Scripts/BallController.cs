using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BallController : MonoBehaviour
{

    public Vector3 direction;
    public float speed;
    public static float speedUp = 0f;
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip launchSound;

    public float minYVelocity = 0.1f;
    public bool unkillable = false;
    public bool launchOnStart = false;

    Rigidbody2D rb2d;
    SpriteRenderer spRenderer;
    ParticleSystem pSystem;

    bool blockBouncedThisFrame = false;

    // Use this for initialization
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spRenderer = GetComponent<SpriteRenderer>();
        pSystem = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (launchOnStart)
            Launch();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        blockBouncedThisFrame = false;
        ScreenBounceCheck();
        direction.Normalize();
        rb2d.MovePosition(transform.position + direction * (speed + speedUp) * Time.fixedDeltaTime);
    }

    public void Launch()
    {
        if (!GameManager.locked)
        {
            audioSource.PlayOneShot(launchSound);
            float angle = Random.Range(45f, 135f) * Mathf.Deg2Rad;
            direction.x = Mathf.Cos(angle);
            direction.y = Mathf.Sin(angle);
        }
    }

    void ScreenBounceCheck()
    {
        Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.min);
        Vector2 topRight = Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.max);

        float halfWidth = spRenderer.bounds.extents.x;
        float halfHeight = spRenderer.bounds.extents.y;

        if (transform.position.x - halfWidth <= bottomLeft.x && direction.x < 0)
            direction.x *= -1;
        if (transform.position.y - halfHeight <= bottomLeft.y && direction.y < 0)
        {
            if (unkillable)
                direction.y *= -1;
            else
                Die();
        }


        if (transform.position.x + halfWidth >= topRight.x && direction.x > 0)
            direction.x *= -1;
        if (transform.position.y + halfHeight >= topRight.y && direction.y > 0)
            direction.y *= -1;
    }

    public void Hit(Vector2 blockPos, Vector2 blockExtents)
    {
        if (!blockBouncedThisFrame)
        {
            if (!audioSource.isPlaying) audioSource.PlayOneShot(hitSound);
            Vector2 blockDist = (Vector2)transform.position - blockPos;

            float dx = Mathf.Abs(transform.position.x - blockPos.x) - blockExtents.x;
            float dy = Mathf.Abs(transform.position.y - blockPos.y) - blockExtents.y;

            if (dx >= dy && Mathf.Sign(direction.x) != Mathf.Sign(blockDist.x))
                direction.x *= -1;
            if (dx <= dy && Mathf.Sign(direction.y) != Mathf.Sign(blockDist.y))
                direction.y *= -1;

            speedUp += GameManager.speedUpFactor;
            direction.Normalize();
            blockBouncedThisFrame = true;
        }
        pSystem.Emit(40);
    }

    public void Die()
    {
        speedUp = 0f;
        audioSource.PlayOneShot(deathSound);
        GameManager.BallDeath();
        direction = Vector3.zero;
    }
}

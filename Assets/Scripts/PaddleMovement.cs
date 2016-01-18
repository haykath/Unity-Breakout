using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PaddleMovement : MonoBehaviour {

    public float speed;
    [Range(0f, 1.5f)]
    public float xSpeedChangeFactor = 0.7f;
    public float direction;
    public AudioClip bounceSound;

    BallController ball;
    float ballExt;

    public SpriteRenderer spRenderer;
    Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        spRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();

        ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallController>();
        ballExt = ball.GetComponent<SpriteRenderer>().bounds.extents.y;
        ball.direction = Vector3.zero;
	}

    public void Move(float dir, float deltaTime)
    {
        direction = dir;
        ScreenBoundsCheck();
        rb2d.MovePosition(transform.position + direction * speed * Vector3.right * deltaTime);
    }

    void ScreenBoundsCheck()
    {
        Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.min);
        Vector2 topRight = Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.max);

        float halfWidth = spRenderer.bounds.size.x / 2f;

        if ((transform.position.x - halfWidth <= bottomLeft.x && direction < 0) || 
            (transform.position.x + halfWidth >= topRight.x && direction > 0))
            direction = 0;
    }

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.tag == "Ball")
        {
            BallController ball = col.gameObject.GetComponent<BallController>();

            if (ball.direction.y < 0)
            {
                ball.audioSource.PlayOneShot(bounceSound);
                ball.direction.y = Mathf.Abs(ball.direction.y);
                ball.direction.x += direction * xSpeedChangeFactor;
                ball.direction.x *= Mathf.Sign((ball.transform.position - transform.position).x) * Mathf.Sign(ball.direction.x); //Optional 1337 mechanics
            }
        }
    }

    void Update()
    {
        if(ball.direction == Vector3.zero || GameManager.locked)
        {
            ball.transform.position = transform.position + (ballExt + spRenderer.bounds.extents.y)*Vector3.up;

            if (Input.GetKeyDown(KeyCode.Space))
                ball.Launch();
        }
    }
}

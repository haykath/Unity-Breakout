using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PaddleMovement))]
public class PaddleAI : MonoBehaviour
{

    public float horizontalDirection;

    PaddleMovement pMovement;
    BallController ball;

    // Use this for initialization
    void Start()
    {
        pMovement = GetComponent<PaddleMovement>();
        ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ball.direction == Vector3.zero) ball.Launch();
        horizontalDirection = (float) System.Math.Truncate(Mathf.Clamp(ball.transform.position.x - transform.position.x, -1, 1));
        pMovement.Move(horizontalDirection, Time.fixedDeltaTime);
    }
}

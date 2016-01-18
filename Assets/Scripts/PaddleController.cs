using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PaddleMovement))]
public class PaddleController : MonoBehaviour {

    public float horizontalDirection;

    PaddleMovement pMovement;

    // Use this for initialization
    void Start () {
        pMovement = GetComponent<PaddleMovement>();	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        horizontalDirection = Input.GetAxisRaw("Horizontal");
        pMovement.Move(horizontalDirection, Time.fixedDeltaTime);
    }
}

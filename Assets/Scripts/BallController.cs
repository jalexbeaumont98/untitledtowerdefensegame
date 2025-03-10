using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    private Camera mainCamera;

    private Vector3 origpos;

    private Rigidbody2D rb;

    public float speed = 300f;
    public Rigidbody2D myRB => rb;

    


    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();   
    }

    private void Start()
    {

        mainCamera = Camera.main;

        origpos = transform.position;

        Invoke(nameof(SetRandomTrajectory), 1f);
    }

    private void SetRandomTrajectory()
    {
        Vector2 force = Vector2.zero;

        force.x = Random.Range(-0.5f, 0.5f);
        force.y = -1f;

        rb.AddForce(force.normalized * speed);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        bool isVisible = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

        if (!isVisible)
        {
            ResetForces();
            ResetPosition();
            Invoke(nameof(SetRandomTrajectory), 1f);
        }
    }

    private void ResetForces()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void ResetPosition()
    {
        transform.position = origpos;

    }
}

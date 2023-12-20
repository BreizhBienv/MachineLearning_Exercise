using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyBehaviour : MonoBehaviour
{
    [SerializeField] private PipeManager PipeManager;

    [SerializeField] LayerMask DeathLayers;

    [SerializeField] float UpwardVelocity = 1.5f;
    [SerializeField] float RotationSpeed = 10f;

    private Rigidbody2D rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            ComputeJump();
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, rb.velocity.y * RotationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((DeathLayers.value & (1 << collision.gameObject.layer)) > 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    void ComputeJump()
    {
        rb.velocity = Vector2.up * UpwardVelocity;
    }

    public void OnResetGame()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
    }
}

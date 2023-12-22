using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using UnityEngine;

public class FlappyBehaviour : MonoBehaviour
{
    [SerializeField] LayerMask DeathLayers;

    [SerializeField] float UpwardVelocity = 1.5f;
    [SerializeField] float RotationSpeed = 10f;

    private Rigidbody2D rb;

    private PipeManager PipeM;

    public BirdIndividual Individual;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PipeM = FindAnyObjectByType<PipeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanJump())
            ComputeJump();

        UpdateFitness();
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, rb.velocity.y * RotationSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((DeathLayers.value & (1 << collision.gameObject.layer)) > 0)
        {
            GameManager.Instance.RemoveAlive(gameObject);
            GameManager.Instance.GameOver();
            Destroy(gameObject);
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

    #region NEAT
    private void UpdateFitness()
    {
        if (Individual == null)
            return;

        Individual.Fitness += Time.deltaTime;
    }

    private bool CanJump()
    {
        if (PipeM.Pipes.Count <= 0)
        {
            if (transform.position.y <= 1.28f)
                return true;

            return false;
        }

        GameObject nextPipe = PipeM.Pipes[0];

        float TopPipeHeight     = nextPipe.GetComponent<PipeBehaviour>().TopPipeHeight.transform.position.y;
        float BottomPipeHeight  = nextPipe.GetComponent<PipeBehaviour>().BottomPipeHeight.transform.position.y;
        
        float BirdHeight        = transform.position.y;
        float TopPipeDist       = Mathf.Abs(BirdHeight - TopPipeHeight);
        float BottomPipeDist    = Mathf.Abs(BirdHeight - BottomPipeHeight);

        float WheightedSum =
            (TopPipeDist * Individual.TopPipeWheight) +
            (BottomPipeDist * Individual.BottomPipeWheight) +
            (BirdHeight * Individual.BirdHeightWheight) +
            Individual.Bias;

        float result = (float)Math.Tanh(WheightedSum);
        bool CanJump = result > 0.5f ? true : false;

        return CanJump;
    }
    #endregion
}

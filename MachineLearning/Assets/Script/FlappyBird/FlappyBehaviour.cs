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

    [Range(0f, 1f)]
    [SerializeField] float RayLength = 0.2f;

    private Rigidbody2D rb;
    private Vector3 CapsuleHalfWidth;

    private PipeManager PipeM;

    #region NEAT
    public BirdIndividual Individual;

    private Vector2 BackRayHitPos = Vector2.zero;
    #endregion
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PipeM = FindAnyObjectByType<PipeManager>();
        CapsuleHalfWidth = new Vector3(GetComponent<CapsuleCollider2D>().size.x / 2f, 0);
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

        Vector2 ray135 = Vector2.up /*+ Vector2.left*/;
        Vector2 ray225 = Vector2.down/* + Vector2.left*/;

        Vector3 pos = transform.position - CapsuleHalfWidth;

        Debug.DrawRay(pos, ray135 * RayLength);
        Debug.DrawRay(pos, ray225 * RayLength);

        RaycastHit2D hit135 = Physics2D.Raycast(pos, ray135, RayLength, DeathLayers.value);
        RaycastHit2D hit225 = Physics2D.Raycast(pos, ray225, RayLength, DeathLayers.value);

        if (hit135.collider != null)
            BackRayHitPos.x = hit135.point.y;
        else
            BackRayHitPos.x = 0;
        
        if (hit225.collider != null)
            BackRayHitPos.y = hit225.point.y;
        else
            BackRayHitPos.y = 0;
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
            //(BirdHeight * Individual.BirdHeightWheight) +
            Individual.Bias;

        if (BackRayHitPos.x != 0)
        {
            float TopSafeGuard = Mathf.Abs(BirdHeight - BackRayHitPos.x);
            //float TopSafeGuard = BackRayHitPos.x;
            WheightedSum += TopSafeGuard * Individual.BackRay.x;
        }

        if (BackRayHitPos.y != 0)
        {
            float BottomSafeGuard = Mathf.Abs(BirdHeight - BackRayHitPos.y);
            //float BottomSafeGuard = BackRayHitPos.y;
            WheightedSum += BottomSafeGuard * Individual.BackRay.y;
        }

        float result = (float)Math.Tanh(WheightedSum);
        bool CanJump = result > 0f ? true : false;

        return CanJump;
    }
    #endregion
}

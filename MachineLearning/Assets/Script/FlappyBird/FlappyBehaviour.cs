using System;
using UnityEngine;

public class FlappyBehaviour : MonoBehaviour
{
    [SerializeField] LayerMask DeathLayers;

    [SerializeField] float UpwardVelocity = 1.5f;
    [SerializeField] float RotationSpeed = 10f;

    [SerializeField] float RayLength = 0.2f;

    private Rigidbody2D rb;
    private Vector3 CapsuleHalfWidth;

    private PipeManager PipeM;

    #region NEAT
    public BirdIndividual Individual;

    private Vector2 RayHitPos = Vector2.zero;
    #endregion
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PipeM = FindAnyObjectByType<PipeManager>();
        CapsuleHalfWidth = new Vector3(GetComponent<CapsuleCollider2D>().size.x / 4f, 0);
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

        Vector2 rayTop = Vector2.up + Vector2.left;
        Vector2 rayBot = Vector2.down + Vector2.left;

        Vector3 pos = transform.position + CapsuleHalfWidth;

        Debug.DrawRay(pos, rayTop * RayLength, Color.blue);
        Debug.DrawRay(pos, rayBot * RayLength, Color.blue);

        RaycastHit2D hitTop = Physics2D.Raycast(pos, rayTop, RayLength, DeathLayers.value);
        RaycastHit2D hitBot = Physics2D.Raycast(pos, rayBot, RayLength, DeathLayers.value);

        RayHitPos.x = float.NaN;
        RayHitPos.y = float.NaN;

        PipeBehaviour pipe;

        if (hitTop.collider != null)
        {
            pipe = hitTop.transform.GetComponentInParent<PipeBehaviour>();

            if (pipe != null)
                RayHitPos.x = pipe.TopPipeHeight.transform.position.y;
            else
                RayHitPos.x = hitTop.point.y;
        }

        if (hitBot.collider != null)
        {
            pipe = hitBot.transform.GetComponentInParent<PipeBehaviour>();

            if (pipe != null)
                RayHitPos.y = pipe.BottomPipeHeight.transform.position.y;
            else
                RayHitPos.y = hitBot.point.y;
        }
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
            Individual.Bias;

        if (!float.IsNaN(RayHitPos.x))
        {
            float TopSafeGuard = Mathf.Abs(BirdHeight - RayHitPos.x);
            WheightedSum += TopSafeGuard * Individual.RayWeight.x;
        }

        if (!float.IsNaN(RayHitPos.y))
        {
            float BottomSafeGuard = Mathf.Abs(BirdHeight - RayHitPos.y);
            WheightedSum += BottomSafeGuard * Individual.RayWeight.y;
        }

        float result = (float)Math.Tanh(WheightedSum);
        bool CanJump = result > 0f ? true : false;

        return CanJump;
    }
    #endregion
}

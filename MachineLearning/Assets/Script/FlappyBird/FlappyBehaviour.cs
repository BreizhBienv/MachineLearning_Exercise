using System;
using UnityEngine;

public class FlappyBehaviour : MonoBehaviour
{
    [SerializeField] LayerMask DeathLayers;

    [SerializeField] float UpwardVelocity = 1.5f;
    [SerializeField] float RotationSpeed = 10f;

    private Rigidbody2D rb;

    private PipeManager PipeM;

    #region NEAT
    public Network NeuralN;
    #endregion
    
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
            Kill();
    }

    public void Kill()
    {
        GameManager.Instance.RemoveAlive(gameObject);
        GameManager.Instance.GameOver();
        Destroy(gameObject);
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
        if (NeuralN == null)
            return;

        NeuralN.Fitness += Time.deltaTime;
    }

    private void CalculateHeightDistance(PipeBehaviour pSumTarget, out float oTopDist, out float oBotDist)
    {
        float topHeight     = pSumTarget.TopPipeHeight.transform.position.y;
        float botHeight     = pSumTarget.BotPipeHeight.transform.position.y;

        float birdHeight    = transform.position.y;
        oTopDist            = Mathf.Abs(topHeight - birdHeight);
        oBotDist            = Mathf.Abs(botHeight - birdHeight);
    }

    private float CalculateInvHorizDist(PipeBehaviour pSumTarget)
    {
        float xPosPipe = pSumTarget.transform.position.x;

        float xPosBird = transform.position.x;
        float xDist = Mathf.Abs(xPosPipe - xPosBird);

        return 1f / xDist;
    }

    private bool CanJump()
    {
        if (PipeM.Pipes.Count <= 0)
        {
            if (transform.position.y <= 1.28f)
                return true;

            return false;
        }

        return NEATWithLayer() > 0f ? true : false;
    }

    private double NEATWithLayer()
    {
        PipeBehaviour nextPipe = PipeM.Pipes[0];
        PipeBehaviour lastPipe = PipeM.LastPipe;

        float[] inputs = new float[NeuralN.NumInput];

        float topDist, botDist;
        CalculateHeightDistance(nextPipe, out  topDist, out botDist);

        inputs[0] = topDist;
        inputs[1] = botDist;
        inputs[2] = transform.position.y;
        //inputs[3] = 0;
        //inputs[4] = 0;
        //inputs[5] = 0;

        //if (lastPipe != null)
        //{
        //    CalculateHeightDistance(lastPipe, out topDist, out botDist);

        //    inputs[2] = topDist;
        //    inputs[3] = botDist;

        //    inputs[4] = CalculateInvHorizDist(nextPipe);
        //    inputs[5] = CalculateInvHorizDist(lastPipe);
        //}

        return NeuralN.ComputeNetwork(inputs)[0];
    }
    #endregion
}

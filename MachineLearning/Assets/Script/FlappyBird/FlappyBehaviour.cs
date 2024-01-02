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
    public BirdIndividual Individual;
    public NeuralNetwork NeuralN = new NeuralNetwork();
    #endregion
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PipeM = FindAnyObjectByType<PipeManager>();

        NeuralN.GenerateBlankNetwork();
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
        if (Individual == null)
            return;

        Individual.Fitness += Time.deltaTime;
    }

    private float CalculateHeightWeightSum(PipeBehaviour pSumTarget)
    {
        float topHeight     = pSumTarget.TopPipeHeight.transform.position.y;
        float botHeight     = pSumTarget.BotPipeHeight.transform.position.y;

        float birdHeight    = transform.position.y;
        float topDist       = Mathf.Abs(topHeight - birdHeight);
        float botDist       = Mathf.Abs(botHeight - birdHeight);

        return (topDist * Individual.TopHeightW) +
            (botDist * Individual.BotHeightW);
    }

    private float CalculateDistanceWeight(PipeBehaviour pSumTarget, float pWeight)
    {
        float xPosPipe = pSumTarget.transform.position.x;

        float xPosBird = transform.position.x;
        float xDist = Mathf.Abs(xPosPipe - xPosBird);

        float invValue = 1f / xDist;

        return invValue * pWeight;
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
        //return NEATWithoutLayer() > 0f ? true : false;
    }

    private double NEATWithLayer()
    {
        PipeBehaviour nextPipe = PipeM.Pipes[0];
        PipeBehaviour lastPipe = PipeM.LastPipe;

        float[] inputs = new float[NeuralN.NumInput];

        float topHeight = nextPipe.TopPipeHeight.transform.position.y;
        float botHeight = nextPipe.BotPipeHeight.transform.position.y;

        float birdHeight = transform.position.y;
        float topDist = Mathf.Abs(topHeight - birdHeight);
        float botDist = Mathf.Abs(botHeight - birdHeight);

        inputs[0] = topDist;
        inputs[1] = botDist;
        inputs[2] = 0;
        inputs[3] = 0;
        inputs[4] = 0;
        inputs[5] = 0;

        if (lastPipe != null)
        {
            float topHeightLast = lastPipe.TopPipeHeight.transform.position.y;
            float botHeightLast = lastPipe.BotPipeHeight.transform.position.y;

            float topDistLast = Mathf.Abs(topHeightLast - birdHeight);
            float botDistLast = Mathf.Abs(botHeightLast - birdHeight);

            inputs[2] = topDistLast;
            inputs[3] = botDistLast;

            float xPosPipe = nextPipe.transform.position.x;

            float xPosBird = transform.position.x;
            float xDist = Mathf.Abs(xPosPipe - xPosBird);

            float invValue = 1f / xDist;

            inputs[4] = invValue;

            xPosPipe = lastPipe.transform.position.x;

            xPosBird = transform.position.x;
            xDist = Mathf.Abs(xPosPipe - xPosBird);

            invValue = 1f / xDist;

            inputs[5] = invValue;
        }

        return NeuralN.ComputeNetwork(inputs)[0];
    }

    private double NEATWithoutLayer()
    {
        PipeBehaviour nextPipe = PipeM.Pipes[0];
        PipeBehaviour lastPipe = PipeM.LastPipe;

        float weightSum = CalculateHeightWeightSum(nextPipe);

        if (lastPipe != null)
        {
            float lastWeightSum = CalculateHeightWeightSum(lastPipe);

            float nextDistW = CalculateDistanceWeight(nextPipe, Individual.NextDistW);
            float lastDistW = CalculateDistanceWeight(lastPipe, Individual.LastDistW);

            weightSum = (weightSum * nextDistW) + (lastWeightSum * lastDistW);
        }

        return (float)Math.Tanh(weightSum);
    }
    #endregion
}

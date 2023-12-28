using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PipeManager : MonoBehaviour
{
    [NonSerialized] public List<PipeBehaviour> Pipes;
    [NonSerialized] public PipeBehaviour LastPipe;

    [SerializeField] private GameObject PipesPrefab;

    [SerializeField] private float SpawnDelay = 1.5f;
    private float Timer;

    [SerializeField] private float HeightRangeOffset = 0.5f;
    [SerializeField] private float YTopClamp;
    [SerializeField] private float YBottomClamp;

    private float SecondsToDestroy = 10f;

    private float HalfPipeWidth;

    private void Start()
    {
        Pipes = new List<PipeBehaviour>();
        HalfPipeWidth = PipesPrefab.GetComponentInChildren<BoxCollider2D>().size.x / 2f;
        LastPipe = null;
    }

    // Update is called once per frame
    void Update()
    {
        PassedPipe();
        AdvanceSpawnPipeDelay();
    }

    void AdvanceSpawnPipeDelay()
    {
        Timer += Time.deltaTime;

        if (Timer <= SpawnDelay)
            return;

        Timer = 0f;

        SpawnPipe();
    }

    void SpawnPipe()
    {
        float yPos = UnityEngine.Random.Range(-HeightRangeOffset, HeightRangeOffset);
        yPos = Mathf.Clamp(yPos, YBottomClamp, YTopClamp);

        Vector3 spawnPos = transform.position + Vector3.up * yPos;

        PipeBehaviour newPipes = Instantiate(PipesPrefab, spawnPos, Quaternion.identity).GetComponent<PipeBehaviour>();

        Pipes.Add(newPipes);

        Destroy(newPipes, SecondsToDestroy);
    }

    public void OnResetGame()
    {
        var foundPipes = FindObjectsOfType<PipeBehaviour>();
        foreach (var pipe in foundPipes)
            Destroy(pipe.gameObject);

        Pipes.Clear();

        Timer = 0f;
    }

    public void PassedPipe()
    {
        FlappyBehaviour flappy = FindAnyObjectByType<FlappyBehaviour>();

        if (flappy == null || Pipes.Count <= 0)
            return;

        Debug.DrawLine(Pipes[0].transform.position + Vector3.up * 0.3f + Vector3.right * HalfPipeWidth,
                Pipes[0].transform.position + Vector3.down * 0.3f + Vector3.right * HalfPipeWidth,
                Color.red);

        PipeBehaviour pipe = Pipes[0].GetComponent<PipeBehaviour>();
        if (flappy.transform.position.x > (pipe.transform.position.x + HalfPipeWidth))
        {
            LastPipe = pipe;
            Pipes.RemoveAt(0); 
        }
    }
}

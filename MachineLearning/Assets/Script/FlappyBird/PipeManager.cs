using System;
using System.Collections.Generic;
using UnityEngine;

public class PipeManager : MonoBehaviour
{
    [NonSerialized] public List<GameObject> Pipes;

    [SerializeField] private GameObject PipesPrefab;

    [SerializeField] private float SpawnDelay = 1.75f;
    private float Timer;

    [SerializeField] private float HeightRangeOffset = 0.5f;
    [SerializeField] private float YTopClamp;
    [SerializeField] private float YBottomClamp;

    [SerializeField] private float SecondsToDestroy = 10f;

    private void Start()
    {
        Pipes = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        if (Timer < SpawnDelay)
            return;

        Timer = 0f;

        SpawnPipe();
    }

    void SpawnPipe()
    {
        float yPos = transform.position.y;

        yPos += UnityEngine.Random.Range(-HeightRangeOffset, HeightRangeOffset);
        yPos = Mathf.Clamp(yPos, YBottomClamp, YTopClamp);

        Vector3 spawnPos = transform.position + Vector3.up * yPos;

        GameObject newPipes = Instantiate(PipesPrefab, spawnPos, Quaternion.identity);

       Pipes.Add(newPipes);

        Destroy(newPipes, SecondsToDestroy);
    }

    public void OnResetGame()
    {
        foreach (var pipe in Pipes)
            Destroy(pipe);

        Pipes.Clear();

        Timer = 0f;
    }
}

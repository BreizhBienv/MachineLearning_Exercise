using System;
using System.Collections.Generic;
using UnityEngine;

public class PipeManager : MonoBehaviour
{
    [NonSerialized] public List<GameObject> Pipes;

    [SerializeField] private GameObject PipesPrefab;

    [SerializeField] private float SpawnDelay = 1.5f;
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

        GameObject newPipes = Instantiate(PipesPrefab, spawnPos, Quaternion.identity);

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
        Pipes.RemoveAt(0);
    }
}

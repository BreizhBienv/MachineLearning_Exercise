using UnityEngine;

public class PipeBehaviour : MonoBehaviour
{
    [SerializeField] private float Speed = 1.5f;

    [SerializeField] public GameObject TopPipeHeight;
    [SerializeField] public GameObject BotPipeHeight;

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * Speed * Time.deltaTime;
    }
}

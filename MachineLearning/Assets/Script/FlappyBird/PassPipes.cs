using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassPipes : MonoBehaviour
{
    private bool PassedOnce = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<FlappyBehaviour>() == null)
            return;

        if (PassedOnce)
            return;

        PassedOnce = true;
        GameManager.Instance.UpdateScore();
    }
}

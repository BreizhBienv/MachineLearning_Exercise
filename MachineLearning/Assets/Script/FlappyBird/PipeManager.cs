using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Gates = new GameObject[2];

    [SerializeField]
    private float GatesSpeed;

    [SerializeField]
    private float HalfScreenWidth;
    private float PipeWidth;

    [SerializeField]
    private float YRangeOffset;
    [SerializeField]
    private float YTopClamp;
    [SerializeField]
    private float YBottomClamp;

    // Start is called before the first frame update
    void Start()
    {
        PipeWidth = Gates[0].GetComponentInChildren<BoxCollider2D>().size.x;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0;  i < Gates.Length; i++)
        {
            GameObject g = Gates[i];
            GameObject prev = Gates[(i + 1) % Gates.Length];

            if (g.transform.position.x <= -HalfScreenWidth - PipeWidth)
            {
                float yOffset = Random.Range(-YRangeOffset, YRangeOffset);

                float newY = yOffset + prev.transform.position.y;
                newY = Mathf.Clamp(newY, YBottomClamp, YTopClamp);

                g.transform.position = new Vector3(
                    HalfScreenWidth + PipeWidth,
                    newY);
            }

            g.transform.position -= new Vector3(GatesSpeed * Time.deltaTime, 0);
        }
    }
}

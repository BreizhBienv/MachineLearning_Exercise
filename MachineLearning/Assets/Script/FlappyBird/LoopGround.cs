using UnityEngine;

public class LoopGround : MonoBehaviour
{
    [SerializeField] private float Speed = 1f;
    [SerializeField] private float Width = 6f;

    private SpriteRenderer SpriteRend;

    private Vector2 StartSize;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRend = GetComponent<SpriteRenderer>();
        StartSize = new Vector2(SpriteRend.size.x, SpriteRend.size.y);
    }

    // Update is called once per frame
    void Update()
    {
        SpriteRend.size = new Vector2(SpriteRend.size.x + Speed * Time.deltaTime, SpriteRend.size.y);
        
        if (SpriteRend.size.x > Width)
        {
            SpriteRend.size = StartSize;
        }
    }
}

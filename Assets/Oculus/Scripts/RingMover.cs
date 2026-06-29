using UnityEngine;

public class RingMover : MonoBehaviour
{
    public float moveSpeedX = 1.2f;
    public float moveSpeedY = 0.8f;
    public float rangeX = 1.5f;
    public float rangeY = 0.5f;

    [Header("Colors")]
    public Color normalColor = Color.yellow;
    public Color shootColor = Color.green;
    public float flashDuration = 0.3f; // how long it stays green

    private float offsetX;
    private float offsetY;
    private Vector3 startPos;
    private Renderer ringRenderer;

    void Start()
    {
        startPos = transform.position;
        offsetX = Random.Range(0f, Mathf.PI * 2f);
        offsetY = Random.Range(0f, Mathf.PI * 2f);
        ringRenderer = GetComponentInChildren<Renderer>();
        SetColor(normalColor);
    }

    void Update()
    {
        float x = startPos.x + Mathf.Sin(Time.time * moveSpeedX + offsetX) * rangeX;
        float y = startPos.y + Mathf.Sin(Time.time * moveSpeedY + offsetY) * rangeY;
        transform.position = new Vector3(x, y, startPos.z);
    }

    // Call this when ring shoots a ball
    public void FlashShoot()
    {
        SetColor(shootColor);
        Invoke(nameof(ResetColor), flashDuration);
    }

    void ResetColor() => SetColor(normalColor);

    void SetColor(Color c)
    {
        // works for URP
        ringRenderer.material.SetColor("_BaseColor", c);
    }
}
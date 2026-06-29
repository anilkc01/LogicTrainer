using UnityEngine;

public class HeartBeat : MonoBehaviour
{
    public float minScale = 0.8f;
    public float maxScale = 1.3f;
    public float speed = 2f;

    private Vector3 originalScale;
    private bool beating = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (!beating) return;

        float scale = Mathf.Lerp(minScale, maxScale,
            (Mathf.Sin(Time.time * speed) + 1f) / 2f);

        transform.localScale = originalScale * scale;
    }

    public void StartBeating() => beating = true;

    public void StopBeating()
    {
        beating = false;
        transform.localScale = originalScale;
    }
}
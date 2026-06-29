using UnityEngine;
using TMPro;
using System.Data;

public class MathBall : MonoBehaviour
{
    public float speed = 10f;
    public TextMeshPro equationText;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip missedCorrectSound;
    public AudioClip wrongHitPlayerSound;

    private string currentEquation;
    private bool moving = false;
    private bool hasBeenProcessed = false;
    private Vector3 targetPosition;
    private Vector3 spawnPosition;
    private Transform playerCam;
    private AudioSource audioSource;
    private Rigidbody rb;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(string equation, bool correct, Transform playerTransform)
    {
        currentEquation = equation;
        playerCam = playerTransform;
        moving = true;

        spawnPosition = transform.position;

        targetPosition = playerTransform.position + new Vector3(
            Random.Range(-0.2f, 0.2f),
            Random.Range(-0.1f, 0.1f),
            0
        );

        equationText.text = equation;
    }

    void Update()
    {
        if (!moving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // always face player
        equationText.transform.LookAt(playerCam.position);
        equationText.transform.Rotate(0, 180, 0);

        // scale whole prefab from 1 when spawned to 0.25 when at player
        float totalDistance = Vector3.Distance(spawnPosition, targetPosition);
        float currentDistance = Vector3.Distance(transform.position, targetPosition);
        float t = 1f - (currentDistance / totalDistance);
        float scale = Mathf.Lerp(0.7f, 0.2f, t);
        transform.localScale = new Vector3(scale, scale, scale);

        if (Vector3.Distance(transform.position, targetPosition) < 0.3f)
        {
            if (hasBeenProcessed) return;

            bool isCurrentlyCorrect = EvaluateEquation() == GameManager.Instance.targetNumber;

            if (isCurrentlyCorrect)
            {
                hasBeenProcessed = true;
                GameManager.Instance.MissedCorrectBall();
                if (missedCorrectSound != null)
                    AudioSource.PlayClipAtPoint(missedCorrectSound, transform.position);
            }
            else
            {
                float distToHead = Vector3.Distance(transform.position, playerCam.position);
                if (distToHead < 0.3f)
                {
                    hasBeenProcessed = true;
                    GameManager.Instance.WrongBallReachedPlayer();
                    if (wrongHitPlayerSound != null)
                        AudioSource.PlayClipAtPoint(wrongHitPlayerSound, transform.position);
                }
            }

            Destroy(gameObject);
        }
    }

    public void OnPunched()
    {
        if (hasBeenProcessed) return;
        hasBeenProcessed = true;
        moving = false;

        bool isCurrentlyCorrect = EvaluateEquation() == GameManager.Instance.targetNumber;

        Vector3 velL = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        Vector3 velR = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        Vector3 punchDir;
        float punchVelocity;

        if (velL.magnitude > velR.magnitude)
        {
            punchDir = velL.normalized;
            punchVelocity = velL.magnitude;
        }
        else
        {
            punchDir = velR.normalized;
            punchVelocity = velR.magnitude;
        }

        if (isCurrentlyCorrect)
        {
            GameManager.Instance.CorrectPunch(punchVelocity);
            if (correctSound != null) audioSource.PlayOneShot(correctSound);
        }
        else
        {
            GameManager.Instance.WrongPunch();
            if (wrongSound != null) audioSource.PlayOneShot(wrongSound);
        }

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = punchDir * 5f + Vector3.up * 2f;
        rb.angularVelocity = Random.insideUnitSphere * 8f;

        Destroy(gameObject, 1.5f);
    }

    private int EvaluateEquation()
    {
        try
        {
            DataTable dt = new DataTable();
            var result = dt.Compute(currentEquation, "");
            return System.Convert.ToInt32(result);
        }
        catch
        {
            Debug.LogError("Failed to parse equation: " + currentEquation);
            return -1;
        }
    }
}
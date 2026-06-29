using UnityEngine;

public class StartBag : MonoBehaviour
{
    public static StartBag Instance;
    public AudioSource punchSound;
    public AudioSource bellSound;
    public float punchThreshold = 0.8f;

    private bool hasBeenPunched = false;
    private Rigidbody rb;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasBeenPunched) return;
        if (!other.CompareTag("Gloves")) return;

        float velL = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch).magnitude;
        float velR = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch).magnitude;

        if (Mathf.Max(velL, velR) > punchThreshold)
        {
            hasBeenPunched = true;

            OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.LTouch);
            OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.RTouch);

            rb.isKinematic = false;
            rb.useGravity = true;

            Vector3 velLV = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
            Vector3 velRV = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            Vector3 punchDir = velLV.magnitude > velRV.magnitude ? velLV : velRV;
            rb.linearVelocity = punchDir * 3f + Vector3.up * 1f;
            rb.angularVelocity = Random.insideUnitSphere * 5f;

            // play punch sound
            punchSound.Play();

            // play bell immediately using PlayClipAtPoint so it works after bag hides
            Invoke(nameof(PlayBellAndStart), punchSound.clip.length);

            // hide bag after 0.3 seconds
            Invoke(nameof(HideBag), 0.3f);
        }
    }

    void HideBag()
    {
        gameObject.SetActive(false);
    }

    void PlayBellAndStart()
    {
        // use PlayClipAtPoint — plays even after GameObject disabled!
        AudioSource.PlayClipAtPoint(bellSound.clip, transform.position, 1f);
        Invoke(nameof(FinishStart), bellSound.clip.length);
    }

    void FinishStart()
    {
        GameManager.Instance.StartGame();
    }

    public void ResetBag()
    {
        CancelInvoke();

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        hasBeenPunched = false;
        gameObject.SetActive(true);
    }
}
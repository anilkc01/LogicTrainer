using UnityEngine;

public class PunchDetector : MonoBehaviour
{
    public OVRInput.Controller controller;
    public float punchThreshold = 1.0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EqBall"))
        {
            Vector3 vel = OVRInput.GetLocalControllerVelocity(controller);
            if (vel.magnitude > punchThreshold)
            {
                other.GetComponent<MathBall>()?.OnPunched();
                OVRInput.SetControllerVibration(0.5f, 0.5f, controller);
                Invoke(nameof(StopHaptics), 0.2f);
            }
        }
    }

    void StopHaptics()
    {
        OVRInput.SetControllerVibration(0, 0, controller);
    }
}
using UnityEngine;

public class HUDFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    void LateUpdate()
    {
        if (player == null) return;
        transform.position = player.position +
            player.TransformDirection(offset);
        transform.LookAt(player.position);
        transform.Rotate(0, 180, 0);
    }
}
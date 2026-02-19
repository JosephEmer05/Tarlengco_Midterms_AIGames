using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Transform homePosition;

    public Vector3 offset = new Vector3(0, 15, -10);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.LookAt(target.position);
        }
        else if (homePosition != null)
        {
            transform.position = Vector3.Lerp(transform.position, homePosition.position, (smoothSpeed / 2) * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, homePosition.rotation, (smoothSpeed / 2) * Time.deltaTime);
        }
    }
}
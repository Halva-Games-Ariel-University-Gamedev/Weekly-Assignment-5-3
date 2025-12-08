using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float targetZoomSize = 1f;

    void Start()
    {
        Camera.main.orthographicSize = targetZoomSize;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Camera.main.orthographicSize = Mathf.Lerp(
            Camera.main.orthographicSize,
            targetZoomSize,
            Time.deltaTime * smoothSpeed * 5f
        );

        Vector3 desiredPosition = target.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed
        );

        transform.position = smoothedPosition;
    }
}
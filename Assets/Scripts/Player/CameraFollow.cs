
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow: MonoBehaviour {
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private float targetHeight = 1.5f;

    [Header("Camera Rotation")]
    [SerializeField] private float yaw = 0f;
    [SerializeField] private float pitch = 40f;
    [SerializeField] private float rotationSpeed = 0.15f;
    [SerializeField] private float minPitch = 25f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Camera Zoom")]
    [SerializeField] private float distance = 12f;
    [SerializeField] private float minDistance = 6f;
    [SerializeField] private float maxDistance = 18f;
    [SerializeField] private float zoomSpeed = 0.01f;

    [Header("Camera Smoothing")]
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float rotationSmooth = 10f;

    [Header("Camera Collision")]
    [SerializeField] private float collisionRadius = 0.25f;
    [SerializeField] private float collisionPadding = 0.2f;
    [SerializeField] private LayerMask collisionLayers = ~0;

    private float currentDistance;
    private Vector3 currentPosition;

    void Start (){
        currentDistance = distance;
        currentPosition = transform.position;
    }

    void LateUpdate (){
        if (target == null){
            return;
        }

        HandleRotation();
        HandleZoom();
        UpdateCameraPosition();
    }

    private void HandleRotation (){
        if (Mouse.current == null){
            return;
        }

        if (Mouse.current.rightButton.isPressed){
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            yaw += mouseDelta.x * rotationSpeed;
            pitch -= mouseDelta.y * rotationSpeed;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
    }

    private void HandleZoom (){
        if (Mouse.current == null){
            return;
        }

        float scroll = Mouse.current.scroll.ReadValue().y;

        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        currentDistance = Mathf.Lerp(currentDistance, distance, followSpeed * Time.deltaTime);
    }

    private void UpdateCameraPosition (){
        Vector3 pivot = target.position + Vector3.up * targetHeight;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 direction = rotation * Vector3.back;

        float allowedDistance = currentDistance;

        if (Physics.SphereCast(pivot, collisionRadius, direction, out RaycastHit hit, currentDistance, collisionLayers, QueryTriggerInteraction.Ignore)){
            allowedDistance = Mathf.Max(0.5f, hit.distance - collisionPadding);
        }

        Vector3 desiredPosition = pivot + direction * allowedDistance;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        Quaternion desiredRotation = Quaternion.LookRotation(pivot - transform.position, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmooth * Time.deltaTime);
    }
}
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float panSensitivity = 0.3f;
    public float tiltSensitivity = 0.2f;
    public float moveSensitivity = 1.0f;
    public float zoomSensitivity = 3.0f;

    public float maxPanArea = 100.0f;
    public float minCameraTilt = 5.0f;
    public float maxCameraTilt = 85.0f;
    public float minCameraZoom = 5.0f;
    public float maxCameraZoom = 120.0f;

    private Transform cameraWorldPoint;
    private Camera cameraComponent;

    private Vector3 previousMouseWorldPoint;
    private Vector3 previousMouseScreenPosition;

    private void Start() {
        cameraWorldPoint = transform.parent;
        cameraComponent = GetComponent<Camera>();
    }

    private void Update() {
        UpdateCameraRotation();
        UpdateCameraTranslation();
        UpdateCameraZoom();
    }

    private void UpdateCameraRotation() {
        Vector3 mouseScreenPosition = Input.mousePosition;

        if (Input.GetMouseButton(2)) {
            Vector3 delta = previousMouseScreenPosition - mouseScreenPosition;
            if (delta.magnitude > 100) {
                delta = Vector3.zero;
            }

            cameraWorldPoint.Rotate(Vector3.up, -delta.x * panSensitivity, Space.World);
            cameraWorldPoint.Rotate(cameraWorldPoint.right, delta.y * tiltSensitivity, Space.World);

            Vector3 angles = cameraWorldPoint.rotation.eulerAngles;
            angles.x = Mathf.Clamp(angles.x, minCameraTilt, maxCameraTilt);
            angles.y %= 360;
            angles.z = 0;
            cameraWorldPoint.rotation = Quaternion.Euler(angles);
        }

        previousMouseScreenPosition = mouseScreenPosition;
    }

    private void UpdateCameraTranslation() {
        Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(ray, out float distance)) {
            Vector3 worldPoint = ray.GetPoint(distance);

            if (Input.GetMouseButton(1)) {
                Vector3 delta = previousMouseWorldPoint - worldPoint;
                delta.y = 0;
                worldPoint += delta;

                cameraWorldPoint.Translate(delta * moveSensitivity, Space.World);

                cameraWorldPoint.position =
                    new Vector3(Mathf.Clamp(cameraWorldPoint.position.x, -maxPanArea, maxPanArea), 0,
                        Mathf.Clamp(cameraWorldPoint.position.z, -maxPanArea, maxPanArea));
            }

            previousMouseWorldPoint = worldPoint;
        }
    }

    private void UpdateCameraZoom() {
        float zoom = -cameraComponent.transform.localPosition.z - Input.mouseScrollDelta.y * zoomSensitivity;
        zoom = Mathf.Clamp(zoom, minCameraZoom, maxCameraZoom);
        cameraComponent.transform.localPosition = Vector3.forward * -zoom;
    }

}
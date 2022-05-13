using UnityEngine;

public class CameraController : MonoBehaviour {

    public float panSensitivity = 0.3f;
    public float tiltSensitivity = 0.2f;
    public float moveSensitivity = 1.0f;

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

            cameraWorldPoint.Rotate(Vector3.up, -delta.x * panSensitivity, Space.World);
            cameraWorldPoint.Rotate(cameraWorldPoint.right, delta.y * tiltSensitivity, Space.World);
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
                cameraWorldPoint.Translate(delta * moveSensitivity, Space.World);
                worldPoint += delta;
            }

            previousMouseWorldPoint = worldPoint;
        }
    }

    private void UpdateCameraZoom() {
        Transform cameraTransform = cameraComponent.transform;
        cameraTransform.localPosition += Vector3.forward * Input.mouseScrollDelta.y;
    }

}
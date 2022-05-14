using UnityEngine;

public class TextController : MonoBehaviour {

    private Transform mainCameraTransform;

    private void Start() {
        mainCameraTransform = Camera.main.transform;
    }

    private void Update() {
        Vector3 pos = mainCameraTransform.position;
        pos.y = transform.position.y;
        transform.LookAt(pos);
    }

}
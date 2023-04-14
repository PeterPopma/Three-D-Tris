using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] Transform cameraLookAtPoint;
    float angle;
    const float cameraDistance = 60;

    void Update()
    {
        angle += 0.3f * Time.deltaTime;
        transform.position = new Vector3(cameraLookAtPoint.position.x + Mathf.Cos(angle) * cameraDistance, cameraLookAtPoint.position.y + 20f, cameraLookAtPoint.position.z + Mathf.Sin(angle) * cameraDistance);
    }
}

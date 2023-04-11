using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] Transform cameraLookAtPoint;
    float angle;
    const float cameraDistance = 60;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        angle += 0.3f * Time.deltaTime;
        transform.position = new Vector3(cameraLookAtPoint.position.x + Mathf.Cos(angle) * cameraDistance, cameraLookAtPoint.position.y + 20f, cameraLookAtPoint.position.z + Mathf.Sin(angle) * cameraDistance);
    }
}

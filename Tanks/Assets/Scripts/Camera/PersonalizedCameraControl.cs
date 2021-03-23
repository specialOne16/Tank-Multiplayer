using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalizedCameraControl : MonoBehaviour
{
    // camera will follow this object
    private GameObject Target;
    // offset between camera and target
    public Vector3 Offset;
    // change this value to get desired smoothness
    public float SmoothTime = 0.3f;

    // This value will change at the runtime depending on target movement. Initialize with zero vector.
    private Vector3 velocity = Vector3.zero;

    public void setTarget(GameObject target)
    {
        Target = target;
        Offset = transform.position - target.transform.position;
    }

    private void LateUpdate()
    {
        if (Target != null)
        {
            // update position
            Vector3 targetPosition = Target.transform.position + Offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);

            // update rotation
            transform.LookAt(Target.transform);
        }
    }
}

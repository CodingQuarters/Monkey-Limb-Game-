using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offSet = new Vector3(5,5, 10);
    public float smoothTime = 0.3f;
    private Vector3 currentVelocity = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void LateUpdate()
    {
        Vector3 targetPosition = target.position + offSet;
        //transform.position = targetPosition; 
        
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref currentVelocity, 
            smoothTime
        );
    }
}

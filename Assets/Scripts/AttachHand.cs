using System.Collections;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AttachHand : MonoBehaviour
{
    //public GameObject targetObject;
    //public KeyCode grappleKey = KeyCode.Mouse0; // tells that the primary wa to grapple is throught he left click
    private ConfigurableJoint activeJoint;
    public GameObject body;
    public Transform armHolder;
    public bool isAttached;
    [SerializeField] private float offSetSnapping;
    [Header("Settings for dragging")]
    [SerializeField] private float dragSensitivity = 0.05f;
    [SerializeField] private float maxDragDistance = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        isAttached = false;
    }
    public void AttachArm(GameObject targetObject)
    {
        Rigidbody playerRigidbody = body.GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            Debug.Log("no rigidbody found on the body");
            return;
        }
        StartCoroutine(LookAtLedge(targetObject));
        if (playerRigidbody.gameObject.GetComponent<ConfigurableJoint>() != null)
        {
            activeJoint = playerRigidbody.gameObject.GetComponent<ConfigurableJoint>();
        }
        else
        {
            activeJoint = playerRigidbody.gameObject.AddComponent<ConfigurableJoint>();
        }


        activeJoint.autoConfigureConnectedAnchor = false;

        // If the ledge is static geometry without a rigidbody, anchor it in world space
        activeJoint.connectedAnchor = targetObject.transform.position;

        activeJoint.anchor = Vector3.zero;

        float grabDistance = Vector3.Distance(armHolder.position, targetObject.transform.position);

        // Lock position to maintain the grab distance
        activeJoint.xMotion = ConfigurableJointMotion.Limited;
        activeJoint.yMotion = ConfigurableJointMotion.Limited;
        activeJoint.zMotion = ConfigurableJointMotion.Limited;
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = grabDistance - offSetSnapping;
        limit.contactDistance = 0.01f;
        activeJoint.linearLimit = limit;
        // Allow free swinging rotation
        activeJoint.angularXMotion = ConfigurableJointMotion.Free;
        activeJoint.angularYMotion = ConfigurableJointMotion.Free;
        activeJoint.angularZMotion = ConfigurableJointMotion.Free;


        // to drag rotation with the delta
        

    }
    public void DetachArm()
    {
        if (activeJoint != null)
        {
            Destroy(activeJoint);
        }
    }
    private IEnumerator LookAtLedge(GameObject targetObject)
    {
        while (isAttached)
        {

            Vector3 direction = targetObject.transform.position - armHolder.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            armHolder.transform.rotation = Quaternion.Euler(0f, 0f, angle + 270);

            yield return null;
        }
        Debug.Log("Stopped looking at direction");
    }
}

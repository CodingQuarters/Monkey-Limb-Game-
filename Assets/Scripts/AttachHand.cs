using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;

public class AttachHand : MonoBehaviour
{
    //public GameObject targetObject;
    //public KeyCode grappleKey = KeyCode.Mouse0; // tells that the primary wa to grapple is throught he left click
    private ConfigurableJoint activeJoint;
    public GameObject body;
    public Transform armTip;
    public bool isAttached;
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

        float grabDistance = Vector3.Distance(armTip.position, targetObject.transform.position);

        // Lock position to maintain the grab distance
        activeJoint.xMotion = ConfigurableJointMotion.Limited;
        activeJoint.yMotion = ConfigurableJointMotion.Limited;
        activeJoint.zMotion = ConfigurableJointMotion.Limited;
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = grabDistance;
        limit.contactDistance = 0.01f;
        activeJoint.linearLimit = limit;
        // Allow free swinging rotation
        activeJoint.angularXMotion = ConfigurableJointMotion.Free;
        activeJoint.angularYMotion = ConfigurableJointMotion.Free;
        activeJoint.angularZMotion = ConfigurableJointMotion.Free;
    }
    public void DetachArm()
    {
        if (activeJoint != null)
        {
            Destroy(activeJoint);
        }
    }
    private IEnumerator LookAtLedge( GameObject targetObject)
    {
        while (isAttached)
        {
            Vector3 targetPosition = new Vector3(targetObject.transform.position.x, armTip.transform.position.y, targetObject.transform.position.z);

            armTip.transform.LookAt(targetPosition);
            yield return null;
        }
        Debug.Log("Stopped looking at direction");
    }
}

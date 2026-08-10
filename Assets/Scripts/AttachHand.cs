using UnityEditor;
using UnityEngine;

public class AttachHand : MonoBehaviour
{
    //public GameObject targetObject;
    //public KeyCode grappleKey = KeyCode.Mouse0; // tells that the primary wa to grapple is throught he left click
    public ConfigurableJoint activeJoint;
    public GameObject body;
    public Transform armTip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void AttachArm(GameObject targetObject)
    {
        if (activeJoint != null) return;


        // Add the joint to the arm's rigidbody
        activeJoint = body.AddComponent<ConfigurableJoint>();
        activeJoint.autoConfigureConnectedAnchor = false;

        // If the ledge is static geometry without a rigidbody, anchor it in world space
        activeJoint.connectedAnchor = targetObject.transform.position;

        activeJoint.anchor = body.transform.InverseTransformDirection(armTip.position);

        // Lock position to maintain the grab distance
        activeJoint.xMotion = ConfigurableJointMotion.Locked;
        activeJoint.yMotion = ConfigurableJointMotion.Locked;
        activeJoint.zMotion = ConfigurableJointMotion.Locked;

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
}

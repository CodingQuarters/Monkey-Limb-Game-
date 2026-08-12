using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScaleToMouse : MonoBehaviour
{
    [SerializeField] private float scaleMultiplier = 0.5f;
    [SerializeField] private float maxLength = 5f;
    [SerializeField] private float minLength = 0.5f;
    [SerializeField] private float distanceMinToAttach = 2f;
    public GameObject armTip;
    public GameObject player;
    public BoxCollider arm;
    public AttachHand attachHand;
    public List<GameObject> Ledges = new List<GameObject>();
    private Camera mainCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] ledgesArray = GameObject.FindGameObjectsWithTag("Ledge");
        Ledges.AddRange(ledgesArray);
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (attachHand.isAttached == false)
        {
            ScaleToMouseAndDirection();
        }
        ClickToGrapple();
    }
    void FixedUpdate()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            player.transform.Translate(Vector3.up * 100f * Time.deltaTime, Space.World);
        }
        
    }
    private void ScaleToMouseAndDirection() // void to organize the script and is used to make the arm always face hte mouse and adjust its scale
    {

        Vector3 mousePosOnScreen = Mouse.current.position.ReadValue();
        mousePosOnScreen.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 mouseWorldPoint = mainCamera.ScreenToWorldPoint(mousePosOnScreen);
        Vector3 direction = mouseWorldPoint - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 270);

        Vector2 objectPos2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 mousePos2D = new Vector2(mouseWorldPoint.x, mouseWorldPoint.y);
        float distance = Vector2.Distance(objectPos2D, mousePos2D);

        float newLength = distance * scaleMultiplier;
        newLength = Mathf.Clamp(newLength, minLength, maxLength);
        transform.localScale = new Vector3(transform.localScale.x, newLength, transform.localScale.z);
//THIS HAS TO BE THE PROBLEM 'CAUSE THERE'S NOTHING OTHERWISE
    }
    private void ClickToGrapple()
    {
        if (TryGetClosestLedge(out GameObject bestLedge, out float shortestDistance))
        {
            if (shortestDistance < distanceMinToAttach)
            {

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    attachHand.isAttached = true;
                    attachHand.AttachArm(bestLedge);
                    Debug.Log("STarted the to attach the arm");
                }
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                attachHand.isAttached = false;
                attachHand.DetachArm();
                Debug.Log("Detached the arm");
            }
        }
    }
    private bool TryGetClosestLedge(out GameObject bestLedge, out float shortestDistance)
    {
        shortestDistance = float.MaxValue;
        bestLedge = null;

        for (int i = 0; i < Ledges.Count; i++)
        {
            Vector3 closestEdgePoint = armTip.transform.position;
            float distance = Vector3.Distance(Ledges[i].transform.position, closestEdgePoint);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                bestLedge = Ledges[i];
            }
        }

        return bestLedge != null;
    }
}

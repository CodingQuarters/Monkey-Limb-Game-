using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class ScaleToMouse : MonoBehaviour
{
    [SerializeField] private float scaleMultiplier = 0.5f;
    [SerializeField] private float maxLength = 5f;
    [SerializeField] private float minLength = 0.5f;
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
        ScaleToMouseAndDirection();
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
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90);

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
            if (shortestDistance < 1f)
            {
                if (Mouse.current.leftButton.isPressed)
                {
                    attachHand.AttachArm(bestLedge);
                    Debug.Log("STarted the to attach the arm");
                }
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
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
            Vector3 closestEdgePoint = arm.ClosestPoint(Ledges[i].transform.position);
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

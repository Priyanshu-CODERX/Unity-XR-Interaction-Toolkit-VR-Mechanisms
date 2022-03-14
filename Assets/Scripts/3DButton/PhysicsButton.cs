using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsButton : MonoBehaviour
{
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private float deadZone = 0.025f;

    private bool isPressed;
    private Vector3 startPos;
    private ConfigurableJoint joint;

    public UnityEvent onPressed, onReleased;

    void Start()
    {
        startPos = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
    }

    void Update()
    {
        if (!isPressed && GetValue() + threshold >= 1)
            Pressed();
        if (isPressed && GetValue() + threshold <= 0)
            Released();
    }

    private float GetValue()
    {
        var val = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;

        if (Mathf.Abs(val) < deadZone)
            val = 0;

        return Mathf.Clamp(val, -1f, 1f);
    }

    public void Pressed()
    {
        isPressed = true;
        onPressed.Invoke();
        Debug.Log("==PRESSED==");
    }

    public void Released()
    {
        isPressed = false;
        onReleased.Invoke();
        Debug.Log("==RELEASED==");
    }

}

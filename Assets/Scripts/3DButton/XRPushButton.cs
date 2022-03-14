using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(BoxCollider))]
public class XRPushButton : XRBaseInteractable
{

    public UnityEvent OnPushed;

    [SerializeField] private float minimalPushDepth;
    [SerializeField] private float maximumPushDepth;

    private XRBaseInteractor pushInteractor = null;
    private bool previouslyPushed = false;
    private float oldPushPosition;

    protected override void OnEnable()
    {
        base.OnEnable();
        hoverEntered.AddListener(StartPush);
        hoverExited.AddListener(EndPush);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        hoverEntered.RemoveListener(StartPush);
        hoverExited.RemoveListener(EndPush);
    }

    private void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        minimalPushDepth = transform.localPosition.y;
        maximumPushDepth = transform.localPosition.y - (boxCollider.bounds.size.y * 0.55f);
    }

    private void StartPush(HoverEnterEventArgs arg0)
    {
        pushInteractor = arg0.interactor;
        oldPushPosition = GetLocalYPosition(arg0.interactor.transform.position);
    }

    private void EndPush(HoverExitEventArgs arg0)
    {
        pushInteractor = null;
        oldPushPosition = 0.0f;
        previouslyPushed = false;
        SetYPosition(minimalPushDepth);
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (pushInteractor)
        {
            float newPushPosition = GetLocalYPosition(pushInteractor.transform.position);
            float pushDifference = oldPushPosition - newPushPosition;

            oldPushPosition = newPushPosition;

            float newPosition = transform.localPosition.y - pushDifference;
            SetYPosition(newPosition);

            CheckPress();
        }
    }

    // Get local position for using this button horizontally and vertically
    private float GetLocalYPosition(Vector3 interactorPosition)
    {
        return transform.root.InverseTransformDirection(interactorPosition).y;
    }

    // Sets Y Position of button without below of above minimum and maximum push
    private void SetYPosition(float yPos)
    {
        Vector3 newPosition = transform.localPosition;
        newPosition.y = Mathf.Clamp(yPos, maximumPushDepth, minimalPushDepth);

        transform.localPosition = newPosition;
    }

    // To check if the button is pressed within a specified range
    private void CheckPress()
    {
        float inRange = Mathf.Clamp(transform.localPosition.y, maximumPushDepth, maximumPushDepth + 0.01f);
        bool isPushedDown = transform.localPosition.y == inRange;

        if (isPushedDown && !previouslyPushed)
            OnPushed.Invoke();

        previouslyPushed = isPushedDown;
    }

    public void TogglePushbuttonState()
    {
        this.enabled = !this.enabled;
    }
}

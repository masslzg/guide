using UnityEngine;
using System.Collections;
using System;

public class TouchTransformationManager : MonoBehaviour
{

    public LayerMask interactableObjects;
    public float minScale = .3f;
    public float maxScale = 3f;

    private Transform selectedTransform;

    private Touch firstTouch;
    private Vector3 firstTouchPosition;
    private Vector3 firstTouchOffset;

    private Touch secondTouch;
    private Vector3 secondTouchPosition;

    private float initialDistance;
    private float currentDistance;
    private Vector3 initialScale;

    private Vector3 lastDirection;
    private Vector3 currentDirection;

    private void Update()
    {
        HandleTouches();
    }

    private void HandleTouches()
    {
        if (Input.touchCount > 0)
        {
            HandleFirstTouch();
            if (Input.touchCount > 1)
            {
                HandleSecondTouch();
            }
        }
    }

    private void HandleFirstTouch()
    {
        // get the first touch's info and world-position
        firstTouch = Input.GetTouch(0);
        firstTouchPosition = Camera.main.ScreenToWorldPoint(firstTouch.position);

        // if there's no object selected
        if (selectedTransform == null)
        {
            // check if a touch just began
            if (firstTouch.phase == TouchPhase.Began)
            {
                // see what is under the touch
                Collider2D hitCollider = Physics2D.OverlapPoint(firstTouchPosition, interactableObjects);
                if (hitCollider != null)
                {
                    // if an object was hit, save it and the distance between touch and object center
                    Debug.Log("touch "+hitCollider.gameObject.name);
                    selectedTransform = hitCollider.transform;
                    selectedTransform.SetAsLastSibling();
                    firstTouchOffset = selectedTransform.position - firstTouchPosition;
                }
            }
        }
        else
        {
            // if there's already an object selected, see if the touch has moved or ended
            switch (firstTouch.phase)
            {
                case TouchPhase.Moved:
                    // if the touch moved, have the object follow if there are no other touches
                    if (Input.touchCount == 1)
                    {
                        SetPosition(firstTouchPosition);
                    }
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    // deselect the object if the touch is lifted
                    selectedTransform = null;
                    break;
            }
        }
    }

    private void HandleSecondTouch()
    {
        // if there is currently a selected object
        if (selectedTransform != null)
        {
            // get the touch info and world position
            secondTouch = Input.GetTouch(1);
            secondTouchPosition = Camera.main.ScreenToWorldPoint(secondTouch.position);

            // if the second touch just began
            if (secondTouch.phase == TouchPhase.Began)
            {
                // save the direction between first and second touch
                currentDirection = secondTouchPosition - firstTouchPosition;
                // initialize the 'last' direction for comparisons
                lastDirection = currentDirection;

                // get the distance between the touches, saving the initial distance for comparison
                currentDistance = (lastDirection).sqrMagnitude;
                initialDistance = currentDistance;

                // save the object's starting scale
                initialScale = selectedTransform.localScale;
            }
            else if (secondTouch.phase == TouchPhase.Moved || firstTouch.phase == TouchPhase.Moved)
            {
                //
                // if either touch moved, update the rotation and scale
                //

                // get the current direction between the touches
                currentDirection = secondTouchPosition - firstTouchPosition;

                // find the angle difference between the last direction and the current
                float angle = Vector3.Angle(currentDirection, lastDirection);

                // Vector3.Angle only outputs positives, so check if it should be a negative angle
                Vector3 cross = Vector3.Cross(currentDirection, lastDirection);
                if (cross.z > 0)
                {
                    angle = -angle;
                }

                // update rotation
                SetRotation(angle);
                // save this direction for next frame's comparison
                lastDirection = currentDirection;

                // get the current distance between touches
                currentDistance = (currentDirection).sqrMagnitude;
                // get what % of the intial distance this new distance is
                float difference = currentDistance / initialDistance;
                // scale by that percentage
                SetScale(difference);
            }

            // if the second touch ended
            if (secondTouch.phase == TouchPhase.Ended || secondTouch.phase == TouchPhase.Canceled)
            {
                // update the first touch offset so dragging will start again from wherever the first touch is now
                firstTouchOffset = selectedTransform.position - firstTouchPosition;
            }
        }
    }

    // update object position without changing Z
    private void SetPosition(Vector3 position)
    {
        if (selectedTransform != null)
        {
            Vector3 newPosition = position + firstTouchOffset;
            newPosition.z = selectedTransform.position.z;
            selectedTransform.position = newPosition;
        }
    }

    // rotate the object by the angle about the Z axis
    private void SetRotation(float angle)
    {
        if (selectedTransform != null)
        {
            selectedTransform.Rotate(Vector3.forward, angle);
        }
    }

    // scale the object by the percentage difference
    // taking into account min/max scale value
    private void SetScale(float percentDifference)
    {
        if (selectedTransform != null)
        {
            Vector3 newScale = initialScale * percentDifference;
            if (newScale.x > minScale && newScale.y > minScale && newScale.x < maxScale && newScale.y < maxScale)
            {
                newScale.z = 1f;
                selectedTransform.localScale = newScale;
            }
        }
    }
}
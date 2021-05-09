using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SwipeEvent : UnityEvent<Vector2> { }

public class GestureManager : MonoBehaviour
{
    public float swipeThreshold = 50f;
    public float tapThreshold = 0.1f;
    public float timeThreshold = 0.3f;
    public float chargeTime = 1.0f;

    public UnityEvent OnTapLeft;
    public UnityEvent OnTapRight;
    public SwipeEvent OnSwipe;

    private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;

    private DateTime fingerDownTime;
    private DateTime fingerUpTime;

    private bool beginTap = false;

    private void Update () 
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            this.fingerDownPos = Input.mousePosition;
            this.fingerUpPos = Input.mousePosition;
            this.fingerDownTime = DateTime.Now;
        }
        if (Input.GetMouseButtonUp(0)) 
        {
            this.fingerDownPos = Input.mousePosition;
            this.fingerUpTime = DateTime.Now;
            this.CheckSwipe();
        }
        foreach (Touch touch in Input.touches) 
        {
            if (touch.phase == TouchPhase.Began) 
            {
                beginTap = true;

                this.fingerDownPos = touch.position;
                this.fingerUpPos = touch.position;
                this.fingerDownTime = DateTime.Now;
                this.CheckTap(fingerDownTime);
            }
            if (touch.phase == TouchPhase.Ended) 
            {
                this.fingerDownPos = touch.position;
                this.fingerUpTime = DateTime.Now;
                //if (beginTap) this.CheckTap(DateTime.Now);
                this.CheckSwipe();
            }
            if (touch.phase == TouchPhase.Stationary)
            {
                this.fingerDownPos = touch.position;
                this.fingerDownTime = DateTime.Now;
            }
        }
    }

    private void CheckTap(DateTime endTime)
    {
        float duration = (float)endTime.Subtract(this.fingerDownTime).TotalSeconds;
        if (duration > this.tapThreshold) return;

        float deltaX = this.fingerDownPos.x - this.fingerUpPos.x;
        float deltaY = fingerDownPos.y - fingerUpPos.y;

        beginTap = false;

        if (Math.Abs(deltaX) < (this.swipeThreshold * 0.5f) && Math.Abs(deltaY) < (this.swipeThreshold * 0.5f))
        {
            if (fingerDownPos.x < Screen.width * 0.5f)
            {
                this.OnTapLeft.Invoke();
            }
            else if (fingerDownPos.x > Screen.width * 0.5f)
            {
                this.OnTapRight.Invoke();
            }
        }
    }
    private void CheckSwipe() 
    {
        float duration = (float)this.fingerUpTime.Subtract(this.fingerDownTime).TotalSeconds;
        if (duration > this.timeThreshold) return;

        float deltaX = this.fingerDownPos.x - this.fingerUpPos.x;
        float deltaY = fingerDownPos.y - fingerUpPos.y;

        Vector2 dir = new Vector2(deltaX, deltaY);
        dir.Normalize();

        if (Mathf.Abs(deltaX) > this.swipeThreshold || Mathf.Abs(deltaY) > this.swipeThreshold) 
        {
            this.OnSwipe.Invoke(dir);
        }

        this.fingerUpPos = this.fingerDownPos;
    }
}
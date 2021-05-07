using System;
using UnityEngine;
using UnityEngine.Events;

public class GestureManager : MonoBehaviour
{
    public float swipeThreshold = 50f;
    public float tapThreshold = 0.1f;
    public float timeThreshold = 0.3f;

    public UnityEvent OnSwipeLeft;
    public UnityEvent OnSwipeRight;
    public UnityEvent OnSwipeUp;
    public UnityEvent OnSwipeDown;
    public UnityEvent OnTapLeft;
    public UnityEvent OnTapRight;


    private Vector2 fingerDown;
    private DateTime fingerDownTime;
    private DateTime fingerPressTime;
    private Vector2 fingerUp;
    private DateTime fingerUpTime;

    private bool beginTap = false;

    private void Update () 
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            this.fingerDown = Input.mousePosition;
            this.fingerUp = Input.mousePosition;
            this.fingerDownTime = DateTime.Now;
        }
        if (Input.GetMouseButtonUp(0)) 
        {
            this.fingerDown = Input.mousePosition;
            this.fingerUpTime = DateTime.Now;
            this.CheckSwipe();
        }
        foreach (Touch touch in Input.touches) 
        {
            if (touch.phase == TouchPhase.Began) 
            {
                if (!beginTap) beginTap = true;

                this.fingerDown = touch.position;
                this.fingerUp = touch.position;
                this.fingerDownTime = DateTime.Now;
            }
            if (touch.phase == TouchPhase.Ended) 
            {
                this.fingerDown = touch.position;
                this.fingerUpTime = DateTime.Now;
                //if (beginTap) this.CheckTap(DateTime.Now);
                this.CheckSwipe();
            }
            if (touch.phase == TouchPhase.Stationary)
            {
                this.fingerDown = touch.position;
                if (beginTap) this.CheckTap(DateTime.Now);
            }
        }
    }

    private void CheckTap(DateTime endTime)
    {
        float duration = (float)endTime.Subtract(this.fingerDownTime).TotalSeconds;
        if (duration > this.tapThreshold) return;

        float deltaX = this.fingerDown.x - this.fingerUp.x;
        float deltaY = fingerDown.y - fingerUp.y;

        beginTap = false;

        if (Math.Abs(deltaX) < this.swipeThreshold && Math.Abs(deltaY) < this.swipeThreshold)
        {
            if (fingerDown.x < Screen.width * 0.5f)
            {
                this.OnTapLeft.Invoke();
            }
            else if (fingerDown.x > Screen.width * 0.5f)
            {
                this.OnTapRight.Invoke();
            }
        }
    }
    private void CheckSwipe() 
    {
        float duration = (float)this.fingerUpTime.Subtract(this.fingerDownTime).TotalSeconds;
        if (duration > this.timeThreshold) return;

        float deltaX = this.fingerDown.x - this.fingerUp.x;
        float deltaY = fingerDown.y - fingerUp.y;

        // Can be 0 (right), 1 (up), 2 (left), or 3 (down)
        int swipeDir = -1;
        if (Mathf.Abs(deltaX) > this.swipeThreshold) 
        {
            if (deltaX > 0 && deltaX > deltaY) 
            {
                swipeDir = 0;
                //this.OnSwipeRight.Invoke();
                //Debug.Log("right");
            } 
            else if (deltaX < 0 && deltaX < deltaY) 
            {
                swipeDir = 2;
                //this.OnSwipeLeft.Invoke();
                //Debug.Log("left");
            }
        }
        if (Mathf.Abs(deltaY) > this.swipeThreshold) 
        {
            if (deltaY > 0 && deltaY > deltaX) 
            {
                swipeDir = 1;
                //this.OnSwipeUp.Invoke();
                //Debug.Log("up");
            } 
            else if (deltaY < 0 && deltaY < deltaX) 
            {
                swipeDir = 3;
                //this.OnSwipeDown.Invoke();
                //Debug.Log("down");
            }
        }

        if (swipeDir == 0) this.OnSwipeRight.Invoke();
        if (swipeDir == 1) this.OnSwipeUp.Invoke();
        if (swipeDir == 2) this.OnSwipeLeft.Invoke();
        if (swipeDir == 3) this.OnSwipeDown.Invoke();

        this.fingerUp = this.fingerDown;
    }
}
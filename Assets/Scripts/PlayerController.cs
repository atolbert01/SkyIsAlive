using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float hopVelocity = 200.0f;
    public float dashVelocity = 600.0f;
    private Rigidbody2D rb2d;
    private bool canDash = false;
    private bool isDashing = false;
    private bool inDashZone = false;

    public float swipeThreshold = 50f;
    public float tapThreshold = 0.1f;
    public float timeThreshold = 0.3f;
    public float chargeTime = 1.0f;

    private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;

    private DateTime fingerDownTime;
    private DateTime fingerUpTime;

    private GameManager gm;
    private Animator anim;

    private bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        int keyLeft = 0; int keyRight = 0;

        if (Input.GetKeyDown("a")) keyLeft = 1;
        if (Input.GetKeyDown("d")) keyRight = 1;
        int move = keyRight - keyLeft;

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
                this.fingerDownPos = touch.position;
                this.fingerUpPos = touch.position;
                this.fingerDownTime = DateTime.Now;
                this.CheckTap(fingerDownTime);
            }
            if (touch.phase == TouchPhase.Ended)
            {
                this.fingerDownPos = touch.position;
                this.fingerUpTime = DateTime.Now;
                this.CheckSwipe();
            }
            if (touch.phase == TouchPhase.Stationary)
            {
                this.fingerDownPos = touch.position;
                this.fingerDownTime = DateTime.Now;
            }
        }

        if (move != 0)
        {
            if (!isDashing && !inDashZone)
            {
                if (move == -1 && facingRight || move == 1 && !facingRight) Flip();
                anim.SetBool("isGrounded", false);
                anim.Play("Bink_Flap", -1, 0);
                rb2d.velocity = Vector2.zero;
                rb2d.AddForce(new Vector2(move * hopVelocity, hopVelocity));
            }
        }

        if (rb2d.velocity == Vector2.zero)
        {
            anim.SetBool("isMoving", false);
        }
        else
        {
            anim.SetBool("isMoving", true);
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void CheckTap(DateTime endTime)
    {
        float duration = (float)endTime.Subtract(this.fingerDownTime).TotalSeconds;
        if (duration > this.tapThreshold) return;

        float deltaX = this.fingerDownPos.x - this.fingerUpPos.x;
        float deltaY = fingerDownPos.y - fingerUpPos.y;

        if (Math.Abs(deltaX) < (this.swipeThreshold * 0.5f) && Math.Abs(deltaY) < (this.swipeThreshold * 0.5f))
        {
            if (fingerDownPos.x < Screen.width * 0.5f)
            {
                //this.OnTapLeft.Invoke();
                this.HopLeft();
            }
            else if (fingerDownPos.x > Screen.width * 0.5f)
            {
                //this.OnTapRight.Invoke();
                this.HopRight();
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
            //this.OnSwipe.Invoke(dir);
            this.DashInDirection(dir);
        }

        this.fingerUpPos = this.fingerDownPos;
    }

    public void DashInDirection(Vector2 dir)
    {
        if (!isDashing && canDash)
        {
            if (dir.x > 0 && !facingRight || dir.x < 0 && facingRight) Flip();
            isDashing = true;
            anim.SetBool("isDashing", isDashing);
            anim.SetBool("isGrounded", false);

            rb2d.gravityScale = 0.0f;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(dir * dashVelocity);
            StartCoroutine("Dash");
        }
    }

    public void HopLeft()
    {
        if (!isDashing && !inDashZone)
        {
            if (facingRight) Flip();
            anim.SetBool("isGrounded", false);
            anim.Play("Bink_Flap", -1, 0);
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(-hopVelocity, hopVelocity));
        }
    }

    public void HopRight()
    {
        if (!isDashing && !inDashZone)
        {
            if (!facingRight) Flip();
            anim.SetBool("isGrounded", false);
            anim.Play("Bink_Flap", -1, 0);
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(hopVelocity, hopVelocity));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "DashZone")
        {
            inDashZone = true;
            rb2d.gravityScale = 0.0f;
            rb2d.velocity = Vector2.zero;
            StartCoroutine("MoveToPoint", (Vector2)other.gameObject.transform.position);
        }

        if (other.gameObject.tag == "Goal")
        {
            gm.GoalReached();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground" && other.GetContact(0).point.y < rb2d.position.y)
        {
            anim.SetBool("isGrounded", true);
        }
        if (other.gameObject.tag == "Hazard")
        {
            this.Die();
        }
    }

    private void Die()
    {
        gm.Restart();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "DashZone")
        {
            canDash = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "DashZone")
        {
            canDash = false;
            inDashZone = false;
        }
        if (other.gameObject.tag == "Map")
        {
            Die();
        }
    }

    private IEnumerator MoveToPoint(Vector2 destination)
    {
        float seconds = 0.333f;
        float elapsed = 0;
        Vector2 startPos = rb2d.position;

        while (elapsed < seconds)
        {
            rb2d.position = Vector2.Lerp(startPos, destination, (elapsed/seconds));
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        rb2d.velocity = Vector2.zero;
        rb2d.gravityScale = 0.0f;
    }

    private IEnumerator Dash()
    {
        yield return new WaitForSeconds(0.5f);
        rb2d.gravityScale = 1.0f;
        isDashing = false;
        anim.SetBool("isDashing", isDashing);
    }
}

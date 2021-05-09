using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float hopVelocity = 200.0f;
    public float dashVelocity = 600.0f;
    private Rigidbody2D rb2d;
    private bool canDash = false;
    private bool isDashing = false;
    private bool moveToRunning = false;
    private bool exitDashZone = false;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        int keyLeft = 0; int keyRight = 0;
        
        if(Input.GetKeyDown("a")) keyLeft = 1;
        if(Input.GetKeyDown("d")) keyRight = 1;
        int move = keyRight - keyLeft;


        if (move != 0)
        {
            if (!isDashing)
            {
                if (moveToRunning)
                {
                    exitDashZone = true;
                }
                rb2d.velocity = Vector2.zero;
                rb2d.AddForce(new Vector2(move*hopVelocity, hopVelocity));
            }
        }
    }
    
    public void DashInDirection(Vector2 dir)
    {
        if (!isDashing && canDash)
        {
            if (moveToRunning)
            {
                exitDashZone = true;
            }
            isDashing = true;
            rb2d.gravityScale = 0.0f;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(dir * dashVelocity);
            StartCoroutine("Dash");
        }
    }

    public void HopLeft()
    {
        if (!isDashing)
        {
            if (moveToRunning)
            {
                exitDashZone = true;
            }
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(-hopVelocity, hopVelocity));
        }
    }

    public void HopRight()
    {
        if (!isDashing)
        {
            if (moveToRunning)
            {
                exitDashZone = true;
            }
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(hopVelocity, hopVelocity));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "DashZone")
        {
            if (!exitDashZone)
            {
                rb2d.gravityScale = 0.0f;
                rb2d.velocity = Vector2.zero;
                StartCoroutine("MoveToPoint", (Vector2)other.gameObject.transform.position);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "DashZone") canDash = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "DashZone")
        {
            if (exitDashZone) exitDashZone = false;
            canDash = false;
        }
    }


    private IEnumerator MoveToPoint(Vector2 destination)
    {
        float seconds = 0.333f;
        float elapsed = 0;
        Vector2 startPos = rb2d.position;
        moveToRunning = true;

        while (elapsed < seconds)
        {
            if (exitDashZone)
            {
                rb2d.position = rb2d.position;
                rb2d.gravityScale = 1.0f;
                //canDash = false;
                moveToRunning = false;
                yield break;
            }
            rb2d.position = Vector2.Lerp(startPos, destination, (elapsed/seconds));
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        rb2d.gravityScale = 1.0f;
        moveToRunning = false;
    }

    private IEnumerator Dash()
    {
        yield return new WaitForSeconds(0.5f);
        rb2d.gravityScale = 1.0f;
        isDashing = false;
    }
}

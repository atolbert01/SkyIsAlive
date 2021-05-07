using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float hopVelocity = 200.0f;
    public float dashVelocity = 600.0f;
    private Rigidbody2D rb2d;
    private bool isDashing = false;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        int keyLeft = 0; int keyRight = 0;
        int keyW = 0; int keyS = 0;
        int keyA = 0; int keyD = 0;
        
        if(Input.GetKeyDown("left")) keyLeft = 1;
        if(Input.GetKeyDown("right")) keyRight = 1;
        int move = keyRight - keyLeft;

        if(Input.GetKeyDown("w")) keyW = 1;
        if(Input.GetKeyDown("s")) keyS = 1;
        int dashV = keyW - keyS;

        if(Input.GetKeyDown("a")) keyA = 1;
        if(Input.GetKeyDown("d")) keyD = 1;
        int dashH = keyD - keyA;

        //velocity = new Vector3(move*HopVelocity, Mathf.Abs(move*HopVelocity), 0);
        //transform.position += velocity * Time.deltaTime;
        if (move != 0)
        {
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(move*hopVelocity, hopVelocity));
        }

        if (dashV != 0)
        {
            rb2d.gravityScale = 0.0f;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(0, dashVelocity*dashV));
            if (!isDashing)
            {
                StartCoroutine("Dash");
                isDashing = false;
            }
        }

        if (dashH != 0)
        {
            rb2d.gravityScale = 0.0f;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(dashVelocity*dashH, 0));
            if (!isDashing)
            {
                StartCoroutine("Dash");
                isDashing = false;
            }
        }
    }

    public void DashLeft()
    {
        if (!isDashing)
        {
            isDashing = true;
            rb2d.gravityScale = 0.0f;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(-dashVelocity, 0));
            StartCoroutine("Dash");
        }
    }
    public void DashRight()
    {
        if (!isDashing)
        {
            isDashing = true;
            rb2d.gravityScale = 0.0f;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(dashVelocity, 0));
            StartCoroutine("Dash");
        }
    }

    public void DashUp()
    {
        if (!isDashing)
        {
            isDashing = true;
            rb2d.gravityScale = 0.0f;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(0, dashVelocity));
            StartCoroutine("Dash");
        }
    }

    public void DashDown()
    {
        if (!isDashing)
        {
            isDashing = true;
            rb2d.gravityScale = 0.0f;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(0, -dashVelocity));
            StartCoroutine("Dash"); 
        }
    }

    public void HopLeft()
    {
        if (!isDashing)
        {
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(-hopVelocity, hopVelocity));
        }
    }

    public void HopRight()
    {
        if (!isDashing)
        {
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(hopVelocity, hopVelocity));
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //rb2d.velocity = Vector2.zero;
    }

    private IEnumerator Dash()
    {
        yield return new WaitForSeconds(0.5f);
        rb2d.gravityScale = 1.0f;
        isDashing = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpPower;
    [SerializeField] private float fallingThreshold = -2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        Debug.Log("Awake");
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // moving left or right
        if (horizontalInput > 0.01f) {
            transform.localScale = Vector3.one;
        } else if (horizontalInput < -0.01f) {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // animator params
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("falling", body.velocity.y < fallingThreshold);
        anim.SetBool("grounded", IsGrounded());

        // wall jump logic
        if (wallJumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (OnWall() && !IsGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
            {
                body.gravityScale = 2.5f;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }
        else
        {
            Debug.Log("Cooldown in action");

            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void Jump()
    {
        Debug.Log("Jump");
        if (IsGrounded())
        {
            Debug.Log("IsGrounded, jumping");
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("jump");
        }
        else if (OnWall() && !IsGrounded())
        {
            Debug.Log("On Wall, jumping");
            if (horizontalInput == 0)
            {
                Debug.Log("On Wall, No Horizontal Input");
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 6, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 2, 3);

            wallJumpCooldown = 0;
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Ground")
    //    {
    //        //grounded = true;
    //    }
    //}

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool OnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}

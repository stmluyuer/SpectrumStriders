// using UnityEngine;

// public class PlayerMovement : MonoBehaviour
// {
//     public float initialspeed = 10f;
//     public float temporaryspeed = 10f;
//     public float initialjumpForce = 5f;
//     public float temporaryjumpForce = 5f;
//     public int maxjump = 2;
//     private int jumpCount = 0;
//     private bool isGrounded;
//     public bool doublejump = false;

//     private Rigidbody2D rbody;
//     public SpriteRenderer spriteRenderer;
//     // 用于射线检测的参数
//     private float groundCheckDistance = 0.3f;
//     public LayerMask groundLayer;

//     void Start()
//     {
//         rbody = GetComponent<Rigidbody2D>();
//         groundLayer = LayerMask.GetMask("Ground");
//     }

//     void Update()
//     {
//         // Movement
//         float move = Input.GetAxis("Horizontal");
//         if (move != 0)
//             rbody.velocity = new Vector2(move * temporaryspeed, rbody.velocity.y);

//         // Jumping
//         if (Input.GetButtonDown("Jump") && (isGrounded || (jumpCount < (maxjump - 1) && doublejump)))
//         {
//             rbody.velocity = new Vector2(rbody.velocity.x, temporaryjumpForce);
//             jumpCount++;
//         }

//         if (isGrounded)
//         {
//             jumpCount = 0;
//         }
//     }

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (collision.gameObject.CompareTag("Ground"))
//         {
//             isGrounded = true;
//         }
//     }

//     private void OnCollisionExit2D(Collision2D collision)
//     {
//         if (collision.gameObject.CompareTag("Ground"))
//         {
//             isGrounded = false;
//         }
//     }
// }

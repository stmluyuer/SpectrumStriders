using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public PlayerData playerData; // 引用数据类

    public int MaxJump { get; private set; }

    private Rigidbody2D rbody;
    public Tilemap tilemap; // Tilemap
    private float currentSpeedMultiplier = 1f; // 当前速度倍率
    private int jumpCount = 0;
    private float coyoteTimeCounter;
    private bool isGrounded; 
    public float testSpeed;
    private TilemapHandler tilemapHandler;
    private PowerUpManager powerUpManager;

    public void Initialize(Tilemap Tilemap)
    {
        tilemap = Tilemap;
        playerData = new PlayerData(); 
        MaxJump = playerData.MaxJump;
        tilemapHandler = gameObject.GetComponent<TilemapHandler>();
        // 以后传入参数时使用
        // rbody = rigidbody;
        // spriteRenderer = renderer;
        // temporaryspeed = speed;
        // temporaryjumpForce = jumpForce;
        // accelerationRate = acceleration;
        // maxSpeedMultiplier = maxMultiplier;
        powerUpManager = FindObjectOfType<PowerUpManager>();
        rbody = GetComponent<Rigidbody2D>();
        // customGravityScale = rbody.gravityScale;
        // spriteRenderer = GetComponent<SpriteRenderer>();
        playerData.GroundLayer = LayerMask.GetMask("Ground");  
        Debug.Log("" + playerData.GroundLayer.value);
        // powerUpEffects = new Dictionary<Color,(PowerUpType,float)>
    }


    public void UpdateMovement()
    {
        {
            if (isGrounded)
            {
                coyoteTimeCounter = playerData.CoyoteTime; //进入地面重置计时
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime; // 离开地面开始计时
            }
            // 获取水平移动输入
            float move = Input.GetAxis("Horizontal");
            if (powerUpManager.isOnBlueCube && move != 0)
            {
                // 计算加速
                currentSpeedMultiplier += playerData.AccelerationRate * Time.deltaTime;
                currentSpeedMultiplier = Mathf.Min(currentSpeedMultiplier, playerData.MaxSpeedMultiplier);
                testSpeed = playerData.Temporaryspeed * currentSpeedMultiplier; // 测试
                // print(testSpeed +"加速度倍率"+currentSpeedMultiplier);
                rbody.velocity = new Vector2(move * testSpeed, rbody.velocity.y);
            }
            else
            {
                // 如果不在蓝色方块上或没有移动，重置速度倍率
                currentSpeedMultiplier = 1f;
                rbody.velocity = new Vector2(move * playerData.Temporaryspeed * currentSpeedMultiplier, rbody.velocity.y);
            }
            // rbody.velocity = new Vector2(move * playerData.Temporaryspeed * currentSpeedMultiplier, rbody.velocity.y);
        }
        // print(isGrounded);
        
        // print(playerData.DoubleJump);
        if (Input.GetButtonDown("Jump") && (coyoteTimeCounter > 0  || (jumpCount < (MaxJump-1) && powerUpManager.doublejump)))
        {
            Vector2 gravityDirection = Vector2.down * Mathf.Sign(rbody.gravityScale);
            // Debug.Log("重力"+rbody.gravityScale+gravityDirection);
            rbody.velocity = new Vector2(rbody.velocity.x, playerData.TemporaryJumpForce * -gravityDirection.y); 

            jumpCount++;
        }
        if (isGrounded)
        {
            jumpCount = 0;
        }

        foreach ((TileBase tile, Vector3Int cellPos) in tilemapHandler.collidedTiles)
        {   
            // Debug.Log("遍历collidedTiles tile："+tile+cellPos);
            if (tile != null)
            { 
                // 获取 Tile 的颜色
                Color tileColor = tilemap.GetColor(cellPos);
                // Debug.Log("脚下 Tile 的颜色：" + tileColor);
                powerUpManager.ApplyTileEffect(tileColor);
                // 检查 tile 是否为 null
                if (tile != null)
                {
                    tilemapHandler.collidedTiles.Remove((tile, cellPos));
                }
                // 如果只需要处理一个 Tile，可以使用 break 跳出循环
                break; 
            }
        }
        
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log(isGrounded);
        if (((1 << collision.gameObject.layer) & playerData.GroundLayer) != 0)
        {
            // print("在地面上");
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerData.GroundLayer) != 0)
        {
            // print("不在地面上");
            isGrounded = false;
        }
    }
}
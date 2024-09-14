using Mirror;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : NetworkBehaviour
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

    public GameObject playerPrefab; // 将 Player 预制体拖放到 Inspector 面板中
    private float respawnHeight = -10f;

    // public Transform[] spawnPoints = ; // 设置多个可能的生成点
    public void Initialize(Tilemap Tilemap)
    {
        // GameObject playerPrefab = Resources.Load<GameObject>("playerPrefab");
        // print(playerPrefab);
        // if (PhotonNetwork.IsConnectedAndReady)
        // {
        //     SpawnPlayer();
        // }

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
        // if (Input.GetButtonDown("Jump") && powerUpManager.isOnMagentaCube)
        // {
        //     rbody.position = powerUpManager.positioshortest;
        // }
        if (Input.GetButtonDown("Jump") && powerUpManager.isOnMagentaCube)
        {
            rbody.position = powerUpManager.positioshortest;
        }
        else if (Input.GetButtonDown("Jump") && (coyoteTimeCounter > 0 || (jumpCount < (MaxJump - 1) && powerUpManager.doublejump)))
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
                if (tile != null)
                {
                    tilemapHandler.collidedTiles.Remove((tile, cellPos));
                }
                // 如果只需要处理一个 Tile，可以使用 break 跳出循环
                break;
            }
        }

        if (transform.position.y < respawnHeight || transform.position.y >-respawnHeight)
        {
            SpawnPlayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = CheckGrounded();
        // // 检查射线是否碰到地面
        // if (hit.collider != null)
        // {
        //     // 如果碰到地面，设置 isGrounded 为 true
        //     isGrounded = true;
        //     // 你可以在这里添加其他逻辑，比如打印调试信息
        //     // Debug.Log("在地面上");
        // }
    }

    bool CheckGrounded()
    {
        float gravityScale  = rbody.gravityScale;
        print(gravityScale);
        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection;
        if (gravityScale > 0)
        {    
            rayDirection = Vector2.down; // 向下发射射线
        }
        else
        {    
            rayDirection = Vector2.up; // 向上发射射线，重力为负
        }
        float rayLength = 0.4f;
        Debug.DrawRay(rayOrigin, rayDirection  * rayLength, Color.red);
        return Physics2D.Raycast(rayOrigin, rayDirection , rayLength, playerData.GroundLayer); 
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // isGrounded = CheckGrounded();   
        if (((1 << collision.gameObject.layer) & playerData.GroundLayer) != 0)
        {
            // print("不在地面上");
            isGrounded = false;
        }
    }

    private void SpawnPlayer()
    {
        float minX = -10f;
        float maxX = 10f;
        float minY = 0f; 
        float maxY = 0f;
        float minZ = -10f;
        float maxZ = 10f;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        float randomZ = Random.Range(minZ, maxZ);
        transform.position = new Vector3(0, 0, 0);
    }
}
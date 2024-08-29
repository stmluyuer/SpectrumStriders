
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rbody;
    public Tilemap originalTilemap;
    private Tilemap newTilemap;    
    public Tilemap tilemap; // Tilemap 
    private List<(TileBase tile, Vector3Int cellPos)> collidedTiles = new List<(TileBase tile, Vector3Int cellPos)>();
    public float initialspeed = 10f;
    public float temporaryspeed = 10f;
    public float initialjumpForce = 5f;
    public float temporaryjumpForce = 6f;
    public int maxjump = 2;
    private int jumpCount = 0;
    public Color carriedColor;
    private bool isGrounded;
    private bool doublejump = false;
    // private float GravityRatio = -1f;
    public float customGravityScale = 1f; // 重力
    // 用于射线检测的参数
    private float groundCheckDistance =0.3f;

    public LayerMask groundLayer; 

    private Dictionary<Color,(PowerUpType,float)> powerUpEffects;
    private Dictionary<Color, System.Action> applyEffects;
    private Dictionary<Color, System.Action> removeEffects;
    public enum PowerUpType
    {
        None = 0,
        JumpingHeightRatio = 1 << 0, // 0000000001 红色
        DoubleJump = 1 << 1, // 0000000010 橙色
        SpeedBoost = 1 << 2, // 0000000100 黄色
        Invincible = 1 << 3, // 0000001000 绿色
        Invinciblea = 1 << 4, // 0000010000 蓝色
        Feather = 1 << 5, // 0000100000 靛色
        Invinciblec = 1 << 6, // 0001000000 紫色
        Transfer = 1 << 7, // 0010000000 品红
        GravityInversion = 1 << 8, // 0100000000 白色
        DeGravityInversion = 1 << 9, // 1000000000 黑色
        Invincibleg = 1 << 10, // 100000000000
    }
     // 存储当前激活的效果

    private PowerUpType activePowerUps = PowerUpType.None; 
    void Start()
    {
        // 创建新的 Tilemap 对象
        GameObject newTilemapGO = new GameObject("Duplicated Tilemap");
        Tilemap newTilemap = newTilemapGO.AddComponent<Tilemap>();
        newTilemap.transform.parent = originalTilemap.transform.parent; // 设置父对象，保持层级关系

        // 复制 Tile 数据
        BoundsInt bounds = originalTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                TileBase tile = originalTilemap.GetTile(tilePosition);
                if (tile != null)
                {
                    newTilemap.SetTile(tilePosition, Instantiate(tile)); // 创建 Tile 副本并设置到新 Tilemap
                }
            }
        }
        newTilemap.color = originalTilemap.color;
        
        // Vector2 gravity2D = Physics2D.gravity;
        // Debug.Log("2D Gravity: " + gravity2D); 
        Vector2 gravity2D = Physics2D.gravity; // 全局重力
        rbody = GetComponent<Rigidbody2D>();
        // customGravityScale = rbody.gravityScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        groundLayer = LayerMask.GetMask("Ground");
        Debug.Log("" + groundLayer);

        powerUpEffects = new Dictionary<Color,(PowerUpType,float)>
        {
            { new Color(1f, 0f, 0f), (PowerUpType.JumpingHeightRatio, 1.42f) }, 
            { new Color(1f, 0.5f, 0f), (PowerUpType.DoubleJump, 0f) }, 
            { new Color(1f, 1f, 0f), (PowerUpType.SpeedBoost, 1.5f) }, 
            { new Color(0f, 1f, 0f), (PowerUpType.Invincible, 1f) },
            { new Color(0f, 0f, 1f), (PowerUpType.Invinciblea, 1f) },
            { new Color(0.3f, 0f, 0.5f), (PowerUpType.Feather, 0.3f) },
            { new Color(0.5f, 0f, 1f), (PowerUpType.Invinciblec, 1f) },
            { new Color(1f, 0f, 1f), (PowerUpType.Transfer, 1f) }, 
            { new Color(1f, 1f, 1f), (PowerUpType.GravityInversion, 1f) }, 
            { new Color(0f, 0f, 0f), (PowerUpType.DeGravityInversion, -1f) } 
        };

        applyEffects = new Dictionary<Color, System.Action>
        {
            { new Color(1f, 0f, 0f), ApplyRedEffect },
            { new Color(1f, 0.5f, 0f), ApplyOrangeEffect },
            { new Color(1f, 1f, 0f), ApplyYellowEffect },
            { new Color(0f, 1f, 0f), ApplyGreenEffect },  
            { new Color(0f, 0f, 1f), ApplyBlueEffect },    
            { new Color(0.3f, 0f, 0.5f), ApplyIndigoEffect },  
            { new Color(0.5f, 0f, 1f), ApplyVioletEffect },  
            { new Color(1f, 0f, 1f), ApplyMagentaEffect }, 
            { new Color(1f, 1f, 1f), ApplyWhiteEffect },   
            { new Color(0f, 0f, 0f), ApplyBlackEffect }  
        };

        removeEffects = new Dictionary<Color, System.Action>
        {
            { new Color(1f, 0f, 0f), RemoveRedEffect },
            { new Color(1f, 0.5f, 0f), RemoveOrangeEffect },
            { new Color(1f, 1f, 0f), RemoveYellowEffect },
            { new Color(0f, 1f, 0f), RemoveGreenEffect },  
            { new Color(0f, 0f, 1f), RemoveBlueEffect },    
            { new Color(0.3f, 0f, 0.5f), RemoveIndigoEffect },  
            { new Color(0.5f, 0f, 1f), RemoveVioletEffect },  
            { new Color(1f, 0f, 1f), RemoveMagentaEffect }, 
            { new Color(1f, 1f, 1f), RemoveWhiteEffect },   
            { new Color(0f, 0f, 0f), RemoveBlackEffect }  
        };

    }
    void LogEffectStatus(Color color, string status)
    {
        string colorName = GetColorName(color); 
        // Debug.Log($"{colorName} {status}");
    }

    // 颜色name
    string GetColorName(Color color)
    {
        if (color == new Color(1f, 0f, 0f))
            return "Red";
        else if (color == new Color(1f, 0.5f, 0f))
            return "Orange";
        else if (color == new Color(1f, 1f, 0f))
            return "Yellow";
        else
            return "Unknown Color";
    }

    void FixedUpdate()
    {
        // Debug.Log("1111");
        // 获取物体的位置
        Vector2 currentPosition = transform.position;

        // 定义检测区域的半径
        float detectionRadius = 0.35f; 

        // 使用 Physics2D 检测物体是否与 Tilemap 碰撞
        Collider2D[] colliders = Physics2D.OverlapCircleAll(currentPosition, detectionRadius);

        foreach (var collider in colliders)
        {
            // 检查碰撞到的物体是否是 Tilemap
            if (collider.gameObject == tilemap.gameObject)
            {
                // 获取碰撞点
                Vector2 hitPoint = collider.ClosestPoint(currentPosition);
                // 偏移量
                Vector2 direction = (hitPoint - currentPosition).normalized; 
                // 计算偏移之后碰撞的对象
                Vector2 offsetPosition = hitPoint + direction * groundCheckDistance;
                // 计算调整后的碰撞点
                // Debug.Log("碰撞点" + offsetPosition);
                // 获取 Tilemap 中的单元格位置
                Vector3Int cellPos = tilemap.WorldToCell(offsetPosition);

                // 获取单元格中的 Tile
                TileBase tile = tilemap.GetTile(cellPos);
                if (tile != null)
                {
                    collidedTiles.Add((tile, cellPos));
                    // Debug.Log("角色碰撞到了 Tile：" + tile.name + "，位置：" + cellPos);
                }
                
                if (tile != null && tile is Tile spriteTile)
        {
                    // 修改 Tile 的颜色
                    // tilemap.SetColor(cellPos, Color.red);
                    // spriteTile.color = new Color(1f,1f,1f);
                        // print("修改了颜色"+ cellPos+tilemap.GetColor(cellPos));
                        // 刷新 Tile 的显示
                    tilemap.RefreshTile(cellPos);
                }
                // 打印检测信息
                // Debug.Log("Tile: " + tile);
                // Debug.Log("Cell Position: " + cellPos);
            }
        }
    }
    void Update()
    {
        // 获取水平移动输入
        float move = Input.GetAxis("Horizontal");
        if (move != 0)
        rbody.velocity = new Vector2(move * temporaryspeed, rbody.velocity.y);
        
        // 使用 foreach 循环遍历 collidedTiles 列表
        foreach ((TileBase tile, Vector3Int cellPos) in collidedTiles)
        {   
            // Debug.Log("遍历collidedTiles tile："+tile+cellPos);
            if (tile != null)
            { 
                // 获取 Tile 的颜色
                Color tileColor = tilemap.GetColor(cellPos);
                // Debug.Log("脚下 Tile 的颜色：" + tileColor);
                ApplyTileEffect(tileColor);
                // 检查 tile 是否为 null
                if (tile != null)
                {
                    collidedTiles.Remove((tile, cellPos));
                }
                // 如果只需要处理一个 Tile，可以使用 break 跳出循环
                break; 
            }
        }
        
        if (Input.GetButtonDown("Jump") && (isGrounded || (jumpCount < (maxjump - 1) && doublejump)))
        {
            // Vector2 gravityDirection = Physics2D.gravity.normalized; 
            // 获取当前重力方向 (考虑 gravityScale)
            Vector2 gravityDirection = Vector2.down * Mathf.Sign(rbody.gravityScale);
            Debug.Log("重力"+rbody.gravityScale+gravityDirection);
            // 根据重力方向调整跳跃力方向
            rbody.velocity = new Vector2(rbody.velocity.x, temporaryjumpForce * -gravityDirection.y); 

            jumpCount++;
        }
        if (isGrounded)
        {
            jumpCount = 0;
        }

    }

    public void ChangeColor(Color newColor)
    {   
        spriteRenderer.color = newColor;
    }

    Vector3Int FindClosestPoint(List<Vector3Int> magentaTilePositions)
    {
        Vector3Int closestPoint = magentaTilePositions[0]; // 初始化为 第一个
        float shortestDistance = Mathf.Infinity; // 初始化为正无穷，确保第一个 Tile 距离会被比较

        foreach (Vector3Int point in magentaTilePositions)
        {
            // 计算角色与当前 Tile 的距离
            float distance = Vector3.Distance(rbody.position, point);

            // 判断距离是否小于最短距离，并且距离大于 1f（排除当前 Tile）
            if (distance < shortestDistance && distance > 1.4f)
            {
                shortestDistance = distance;
                closestPoint = point;
            }
        }

        return closestPoint; // 返回最近的 Tile 坐标，如果没有找到则返回 null
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log(isGrounded);
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Debug.Log(isGrounded);
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // 获取 Tile 的颜色
    Color GetTileColor(TileBase tile)
    {
        // 检查 Tile 是否为 Tile 类型
        if (tile is Tile)
        {
            return ((Tile)tile).color;
        }

        // 如果不是 Tile 类型，则返回默认颜色（例如：白色）
        return Color.white;
    }

    // void ApplyTileEffect(Color tileColor)
    // {
    //     // 黄色加速倍率
    //     float yellowBoostMultiplier = 1.5f; 
    //     // 红色跳跃高度倍率
    //     float redjumpheight = 2f; 
        
        // // red
        // if (tileColor == new Color(1f,0f,0f) && (activePowerUps & PowerUpType.JumpingHeightRatio) == 0)
        // {
        //     Debug.Log("red true");

        //     // 应用红色加速效果
        //     temporaryjumpForce *= redjumpheight; 
        //     activePowerUps |= PowerUpType.JumpingHeightRatio;
        //     if (spriteRenderer.color != tileColor)
        //     {
        //         spriteRenderer.color = tileColor;
        //         Debug.Log("color:"+spriteRenderer.color);
        //     }
        // }

    //     // orange
    //     else if (tileColor == new Color(1f,0.5f,0f) && (activePowerUps & PowerUpType.DoubleJump) == 0)
    //     {
    //         Debug.Log("orange true");
    //         doublejump = true;
    //         // // 应用红色加速效果
    //         // temporaryjumpForce *= redjumpheight; 
    //         activePowerUps |= PowerUpType.DoubleJump;
    //         if (spriteRenderer.color != tileColor)
    //         {
    //             spriteRenderer.color = tileColor;
    //             Debug.Log("color:"+spriteRenderer.color);
    //         }
    //     }

    //     // yellow
    //     else if (tileColor == new Color(1f,1f,0f) && (activePowerUps & PowerUpType.SpeedBoost) == 0)
    //     {
    //         Debug.Log("yellow true");

    //         // 应用红色加速效果
    //         temporaryspeed *= yellowBoostMultiplier; 
    //         activePowerUps |= PowerUpType.SpeedBoost;
    //         if (spriteRenderer.color != tileColor)
    //         {
    //             spriteRenderer.color = tileColor;
    //             Debug.Log("color:"+spriteRenderer.color);
    //         }
    //     }
        
    //     // red end
    //     if (tileColor != new Color(1f,0f,0f) && (activePowerUps & PowerUpType.JumpingHeightRatio) != 0) 
    //     {
    //         Debug.Log("red Leave");
    //         temporaryjumpForce /= redjumpheight;
    //         activePowerUps &= ~PowerUpType.JumpingHeightRatio;
    //     }
    //     // orange end
    //     if (tileColor != new Color(1f,0.5f,0f) && (activePowerUps & PowerUpType.DoubleJump) != 0) 
    //     {
    //         Debug.Log("orange Leave");
    //         doublejump = false;
    //         activePowerUps &= ~PowerUpType.DoubleJump;
    //     }
    //     // yellow end
    //     if (tileColor != new Color(1f,1f,0f) && (activePowerUps & PowerUpType.SpeedBoost) != 0)
    //     {
    //         Debug.Log("yellow Leave");
    //         temporaryspeed /= 1.5f; 
    //         activePowerUps &= ~PowerUpType.SpeedBoost;
    //     } 
    // }
    void ApplyTileEffect(Color tileColor)
    {
        foreach (var effect in powerUpEffects)
        {
            if (effect.Key == tileColor && (activePowerUps & effect.Value.Item1) == 0)
            {
                applyEffects[effect.Key]();
                activePowerUps |= effect.Value.Item1;
                UpdateSpriteColor(tileColor);
            }
            else if (effect.Key != tileColor && (activePowerUps & effect.Value.Item1) != 0)
            {
                removeEffects[effect.Key]();
                activePowerUps &= ~effect.Value.Item1;
            }
        }
    }

    void ApplyRedEffect()
    {
        LogEffectStatus(new Color(1f, 0f, 0f), "Enter");
        temporaryjumpForce *= powerUpEffects[new Color(1f, 0f, 0f)].Item2;
    }

    void RemoveRedEffect()
    {
        LogEffectStatus(new Color(1f, 0f, 0f), "Leave");
        temporaryjumpForce /= powerUpEffects[new Color(1f, 0f, 0f)].Item2;
    }

    void ApplyOrangeEffect()
    {
        LogEffectStatus(new Color(1f, 0.5f, 0f), "Enter");
        doublejump = true;
    }

    void RemoveOrangeEffect()
    {
        LogEffectStatus(new Color(1f, 0.5f, 0f), "Leave");
        doublejump = false;
    }

    void ApplyYellowEffect()
    {
        LogEffectStatus(new Color(1f, 1f, 0f), "Enter");
        temporaryspeed *= powerUpEffects[new Color(1f, 1f, 0f)].Item2;
    }

    void RemoveYellowEffect()
    {
        LogEffectStatus(new Color(1f, 1f, 0f), "Leave");
        temporaryspeed /= powerUpEffects[new Color(1f, 1f, 0f)].Item2;
    }
    void ApplyGreenEffect()
    {
        LogEffectStatus(new Color(0f, 0f, 0f), "Enter");
        // rbody.gravityScale *= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }
    void RemoveGreenEffect()
    {
        LogEffectStatus(new Color(0f, 0f, 0f), "Leave");
        // rbody.gravityScale /= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }

    void ApplyBlueEffect()
    {
        LogEffectStatus(new Color(0f, 0f, 0f), "Enter");
        // rbody.gravityScale *= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }
    void RemoveBlueEffect()
    {
        LogEffectStatus(new Color(0f, 0f, 0f), "Leave");
        // rbody.gravityScale /= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }
    void ApplyIndigoEffect()
    {
        LogEffectStatus(new Color(0.3f, 0f, 0.5f), "Enter");
        // rbody.gravityScale *= powerUpEffects[new Color(0.3f, 0f, 0.5f)].Item2;
    }

    void RemoveIndigoEffect()
    {
        LogEffectStatus(new Color(0.3f, 0f, 0.5f), "Leave");
        // rbody.gravityScale /= powerUpEffects[new Color(0.3f, 0f, 0.5f)].Item2;
    }

    void ApplyVioletEffect()
    {
        LogEffectStatus(new Color(0f, 0f, 0f), "Enter");
        // rbody.gravityScale *= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }

    void RemoveVioletEffect()
    {
        LogEffectStatus(new Color(0f, 0f, 0f), "Leave");
        // rbody.gravityScale /= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }

    void ApplyMagentaEffect()
    {
        LogEffectStatus(new Color(1f, 0f, 1f), "Enter");
        tilemap.CompressBounds();
        BoundsInt area = tilemap.cellBounds;
        print(tilemap.cellBounds);
        TileBase[] tileArray = tilemap.GetTilesBlock(area);
        int tileArrayLength = tileArray.Length; 
        print(tileArrayLength);
        List<TileBase> magentaTiles = new List<TileBase>();
        List<Vector3Int> magentaTilePositions = new List<Vector3Int>();
        // 遍历所有 Tile
        for (int i = 0; i < tileArrayLength; i++ )
        {
            if (tileArray[i] != null) // 确保 Tile 不是空的
            {
                // 获取 Tile 的颜色
                Color tileColor = tilemap.GetColor(area.position + new Vector3Int(i % area.size.x, i / area.size.x, 0));
                
                // 判断颜色
                if (tileColor == new Color(1f, 0f, 1f))
                {
                    magentaTiles.Add(tileArray[i]);

                    magentaTilePositions.Add(area.position + new Vector3Int(i % area.size.x, i / area.size.x, 0));
                    print(area.position + new Vector3Int(i % area.size.x, i / area.size.x, 0));
                }
            }
        }
        print("传送之前的坐标"+rbody.position);
        Vector3Int positioshortestint = FindClosestPoint(magentaTilePositions);
        if (positioshortestint != magentaTilePositions[0])
            print(positioshortestint);
            Vector2 positioshortest = new Vector2(positioshortestint.x + 0.5f, positioshortestint.y + 1.2f);
            rbody.position = positioshortest;
        // for (int index = 0; index < tileArray.Length; index++)
        // {
        //     print("全部cell对象"+tileArray[index]);
        // }

    }

    void RemoveMagentaEffect()
    {
        LogEffectStatus(new Color(1f, 0f, 1f), "Leave");
        rbody.gravityScale /= powerUpEffects[new Color(1f, 0f, 1f)].Item2;
    }

    void ApplyWhiteEffect()     
    {
        LogEffectStatus(new Color(1f, 1f, 1f), "Enter");
        rbody.gravityScale *= powerUpEffects[new Color(1f, 1f, 1f)].Item2;
    }

    void RemoveWhiteEffect()
    {
        LogEffectStatus(new Color(1f, 1f, 1f), "Leave");
        rbody.gravityScale /= powerUpEffects[new Color(1f, 1f, 1f)].Item2;
    }

    void ApplyBlackEffect()
    {
        LogEffectStatus(new Color(0f, 0f, 0f), "Enter");
        rbody.gravityScale *= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }

    void RemoveBlackEffect()
    {
        LogEffectStatus(new Color(0f, 0f, 0f), "Leave");
        rbody.gravityScale /= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }


    void UpdateSpriteColor(Color tileColor)
    {
        if (spriteRenderer.color != tileColor)
        {
            spriteRenderer.color = tileColor;
            // Debug.Log("color:" + spriteRenderer.color);
        }
    }
}

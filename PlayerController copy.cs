
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;

// public class PlayerController : MonoBehaviour
// {
//     private SpriteRenderer spriteRenderer;
//     private Rigidbody2D rbody;
//     public Tilemap tilemap; // Tilemap 
//     private List<(TileBase tile, Vector3Int cellPos)> collidedTiles = new List<(TileBase tile, Vector3Int cellPos)>();
//     public float initialspeed = 10f;
//     public float temporaryspeed = 10f;
//     public float initialjumpForce = 5f;
//     public float temporaryjumpForce = 5f;
//     public int maxjump = 2;
//     private int jumpCount = 0;
//     public Color carriedColor;
//     private bool isGrounded;
//     private bool doublejump = false;
    
//     // 用于射线检测的参数
//     private float groundCheckDistance =0.3f;
//     public LayerMask groundLayer; 

//     private Dictionary<Color,(PowerUpType,float)> powerUpEffects;
//     private Dictionary<Color, System.Action> applyEffects;
//     private Dictionary<Color, System.Action> removeEffects;
//     public enum PowerUpType
//     {
//         None = 0,
//         JumpingHeightRatio = 1 << 0, // 00001 红色
//         DoubleJump = 1 << 1, // 00010 橙色
//         SpeedBoost = 1 << 2, // 00100 黄色
//         Invincible = 1 << 3, // 01000
//     }
//      // 存储当前激活的效果

//     private PowerUpType activePowerUps = PowerUpType.None; 
//     void Start()
//     {
//         rbody = GetComponent<Rigidbody2D>();
//         spriteRenderer = GetComponent<SpriteRenderer>();
//         groundLayer = LayerMask.GetMask("Ground");
//         Debug.Log("" + groundLayer);

//         powerUpEffects = new Dictionary<Color,(PowerUpType,float)>
//         {
//             { new Color(1f, 0f, 0f), (PowerUpType.JumpingHeightRatio, 2f) }, // red
//             { new Color(1f, 0.5f, 0f), (PowerUpType.DoubleJump, 0f) }, // orange
//             { new Color(1f, 1f, 0f), (PowerUpType.SpeedBoost, 1.5f) } // yellow
//         };
//         applyEffects = new Dictionary<Color, System.Action>
//         {
//             { new Color(1f, 0f, 0f), ApplyRedEffect },
//             { new Color(1f, 0.5f, 0f), ApplyOrangeEffect },
//             { new Color(1f, 1f, 0f), ApplyYellowEffect }
//         };

//         removeEffects = new Dictionary<Color, System.Action>
//         {
//             { new Color(1f, 0f, 0f), RemoveRedEffect },
//             { new Color(1f, 0.5f, 0f), RemoveOrangeEffect },
//             { new Color(1f, 1f, 0f), RemoveYellowEffect }
//         };
//     }
//     void LogEffectStatus(Color color, string status)
//     {
//         string colorName = GetColorName(color);
//         Debug.Log($"{colorName} {status}");
//     }

//     string GetColorName(Color color)
//     {
//         if (color == new Color(1f, 0f, 0f))
//             return "Red";
//         else if (color == new Color(1f, 0.5f, 0f))
//             return "Orange";
//         else if (color == new Color(1f, 1f, 0f))
//             return "Yellow";
//         else
//             return "Unknown Color";
//     }

//     void FixedUpdate()
//     {
//         // Debug.Log("1111");
//         // 获取物体的位置
//         Vector2 currentPosition = transform.position;

//         // 定义检测区域的半径（可以根据实际情况调整）
//         float detectionRadius = 1f; 

//         // 使用 Physics2D 检测物体是否与 Tilemap 碰撞
//         Collider2D[] colliders = Physics2D.OverlapCircleAll(currentPosition, detectionRadius);

//         foreach (var collider in colliders)
//         {
//             // 检查碰撞到的物体是否是 Tilemap
//             if (collider.gameObject == tilemap.gameObject)
//             {
//                 // 获取碰撞点
//                 Vector2 hitPoint = collider.ClosestPoint(currentPosition);
//                 // 偏移量
//                 Vector2 direction = (hitPoint - currentPosition).normalized; 
//                 // 计算偏移之后碰撞的对象
//                 Vector2 offsetPosition = hitPoint + direction * groundCheckDistance;
//                 // 计算调整后的碰撞点（假设需要调整以匹配地面检查距离）
//                 // Vector2 adjustedHitPoint = hitPoint - new Vector2(0f, groundCheckDistance);
//                 // Vector3 Point = collider.gameObject.transform.position;
//                 Debug.Log(offsetPosition);
//                 // 获取 Tilemap 中的单元格位置
//                 Vector3Int cellPos = tilemap.WorldToCell(offsetPosition);

//                 // 获取单元格中的 Tile
//                 TileBase tile = tilemap.GetTile(cellPos);
//                 if (tile != null)
//                 {
//                     collidedTiles.Add((tile, cellPos));
//                     Debug.Log("角色碰撞到了 Tile：" + tile.name + "，位置：" + cellPos);
//                 }
//                 // 打印检测信息
//                 Debug.Log("Tile: " + tile);
//                 Debug.Log("Cell Position: " + cellPos);
//             }
//         }
//     }
//     void Update()
//     {
//         // 获取水平移动输入
//         float move = Input.GetAxis("Horizontal");
//         if (move != 0)
//         rbody.velocity = new Vector2(move * temporaryspeed, rbody.velocity.y);
        
//         // 从角色底部中心发射射线
        
//         // 获取水平移动输入
//         // float horizontal = Input.GetAxis("Horizontal");
//         // if (horizontal != 0)
//         // {
//         //     transform.Translate(Vector2.right * horizontal * Time.deltaTime);
//         // }
//         // 检测跳跃输入并且角色在地面上
//         // 获取角色脚下的 Tile
//         // 使用 foreach 循环遍历 collidedTiles 列表
//         foreach ((TileBase tile, Vector3Int cellPos) in collidedTiles)
//         {   
//             Debug.Log("" + tile+cellPos);
//             if (tile != null)
//             {
//                 // 检查当前 Tile 是否为角色脚下的 Tile       
//                 // 获取 Tile 的颜色
//                 Color tileColor = tilemap.GetColor(cellPos);
//                 // Debug.Log("脚下 Tile 的颜色：" + tileColor);
//                 ApplyTileEffect(tileColor);
//                 // 检查 tile 是否为 null
//                 if (tile != null)
//                 {
//                     collidedTiles.Remove((tile, cellPos));
//                 }
                

//                 // 如果只需要处理一个 Tile，可以使用 break 跳出循环
//                 break; 
//             }
//         }
        
//         // (TileBase tile, Vector3Int cellPos) = RayGetTileUnderFoot();
//         //  if (tile != null)
//         // {
//         //     // 获取 Tile 的颜色
//         //     Color tileColor = tilemap.GetColor(cellPos);
//         //     Debug.Log("脚下 Tile 的颜色：" + tileColor);
//         //     ApplyTileEffect(tileColor);
//         // }
//         // private bool IsTileUnderFoot(Vector3Int tilePos)
//         // {
//         //     // 根据您的游戏逻辑实现具体的判断方法
//         //     // 例如，可以判断 Tile 的中心点是否在角色的碰撞体下方
//         //     // ...
//         // }
//         if (Input.GetButtonDown("Jump") && (isGrounded || (jumpCount < (maxjump - 1) && doublejump)))
//         {
//             rbody.velocity = new Vector2(rbody.velocity.x, temporaryjumpForce);
//             jumpCount++;
//             // jumpForce = 20f;
//             // PlayerController ballController = FindObjectOfType<PlayerController>();
//             // ballController.ChangeColor(Color.red);
//         }
//         if (isGrounded)
//         {
//             jumpCount = 0;
//         }

//     }


    
//     // // 检测角色是否接触地面
//     // private void OnCollisionEnter2D(Collision2D collision)
//     // {
//     //     if (collision.gameObject.CompareTag("Ground"))
//     //     {
//     //         isGrounded = true;
//     //     }
//     // }

//     // private void OnCollisionExit2D(Collision2D collision)
//     // {
//     //     if (collision.gameObject.CompareTag("Ground"))
//     //     {
//     //         isGrounded = false;
//     //     }
//     // }

//     public void ChangeColor(Color newColor)
//     {   
//         spriteRenderer.color = newColor;
//     }

//     // 获取角色脚下的 Tile
//     (TileBase tile, Vector3Int cellPos) RayGetTileUnderFoot()
//     {
//         // 获取角色的底部中心点
//         Vector2 raycastOrigin = transform.position + Vector3.down * GetComponent<SpriteRenderer>().bounds.extents.y;

//         // 从角色底部中心向下发射射线
//         RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, groundCheckDistance, groundLayer);
//         // 偏移碰撞点
//         Vector2 adjustedHitPoint = hit.point - new Vector2(0f, groundCheckDistance);
//         // // 从角色中心向下发射射线
//         // RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        
//         Debug.DrawRay(raycastOrigin, Vector2.down * groundCheckDistance, Color.red); 

//         if (hit.collider != null)
//         {
//             // Debug.Log("Hit Point (World): " + hit.point);
//             // Debug.Log("Tilemap Bounds: " + tilemap.cellBounds);
//             // hit.point = (hit.point.x,hit.poi);    
//             // Debug.Log("" + hit.point);
//             // 将碰撞点转换为 Tilemap 坐标
//             Vector3Int cellPosition = tilemap.WorldToCell(adjustedHitPoint);
//             // tilemap.SetColor(new Vector3Int(3, 0, 0), Color.white); 
//             // Debug.Log(tilemap.GetEditorPreviewTile(cellPosition));
//             // Debug.Log("Cell Position (Tilemap): " + cellPosition + "hit point :" + hit.point+"" + adjustedHitPoint);
//             // Debug.Log(tilemap.GetColor(cellPosition));
//             Color randomColor = new Color(Random.value, Random.value, Random.value);
//             // tilemap.SetColor(cellPosition,randomColor);
//             // ApplyTileEffect(randomColor);
//             // Debug.Log("" + cellPosition);
//             // 获取该位置的 Tile
//             // Debug.Log("" + tilemap.GetTile(new Vector3Int(3,-1,0)));
//             return (tilemap.GetTile(cellPosition), cellPosition);
//         }

//         return (null, default); // 如果没有检测到 Tile，则返回 null
//     }

//     (TileBase tile, Vector3Int cellPos) GetTileUnderFoot()
//     {
//         // 获取角色的碰撞体
//         Collider2D collider = GetComponent<Collider2D>();

//         if (collider != null)
//         {
//             Vector2 checkPoint = transform.position + Vector3.down * collider.bounds.extents.y;
//             TileBase tile = tilemap.GetTile(tilemap.WorldToCell(checkPoint));

//             if (tile != null)
//             {
//                 // 获取 Tile 的坐标
//                 Vector3Int cellPosition = tilemap.WorldToCell(checkPoint);
//                 return (tile, cellPosition);
//             }
//         }

//         return (null, default); // 如果没有检测到 Tile，则返回 null
//     }

//     // 用于检查碰撞是否落地面并返回 Tile 信息
//     private (TileBase tile, Vector3Int cellPos) GetCollidedTile(Collision2D collision)
//     {
//         // 检查碰撞到的物体是否是 Tilemap
//         if (collision.gameObject == tilemap.gameObject)
//         {
//             // 获取当前对象的二维世界坐标（中心点）
//             Vector2 centerPoint1 = transform.position;

//             // 获取碰撞对象的二维世界坐标（中心点）
//             Vector2 centerPoint2 = collision.collider.transform.position;
            
//             // 打印中心点
//             Debug.Log("Object 1 Center: " + centerPoint1);
//             Debug.Log("Object 2 Center: " + centerPoint2);
//             Vector2 adjustedHitPoint = collision.GetContact(0).point - new Vector2(0f, groundCheckDistance);
//             Vector3Int cellPosition = tilemap.WorldToCell(adjustedHitPoint);
//             TileBase tile = tilemap.GetTile(cellPosition);

//             return (tile, cellPosition); 
//         }

//         // 如果没有碰撞到 Tilemap，则返回默认值
//         return (null, default); 
//     }

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         // Debug.Log("角色碰撞到了 Tile：");
//         // 调用函数获取碰撞到的 Tile 信息
//         // (TileBase tile, Vector3Int cellPos) = GetCollidedTile(collision);
//         if (collision.gameObject.CompareTag("Ground"))
//         {
//             isGrounded = true;
//         }
//         // 处理碰撞逻辑，例如：
//         // if (tile != null)
//         // {
//         //     collidedTiles.Add((tile, cellPos));
//         //     Debug.Log("角色碰撞到了 Tile：" + tile.name + "，位置：" + cellPos);
//         // }
//     }
//     private void OnCollisionExit2D(Collision2D collision)
//     {
//         if (collision.gameObject.CompareTag("Ground"))
//             {
//                 isGrounded = false;
//             }
//         // if (collision.contacts.Length > 0) 
//         // {
            
//         // (TileBase tile, Vector3Int cellPos) = GetCollidedTile(collision);

        
//         // }
//     }

//     // 获取 Tile 的颜色
//     Color GetTileColor(TileBase tile)
//     {
//         // 检查 Tile 是否为 Tile 类型
//         if (tile is Tile)
//         {
//             return ((Tile)tile).color;
//         }

//         // 如果不是 Tile 类型，则返回默认颜色（例如：白色）
//         return Color.white;
//     }

//     // void ApplyTileEffect(Color tileColor)
//     // {
//     //     // 黄色加速倍率
//     //     float yellowBoostMultiplier = 1.5f; 
//     //     // 红色跳跃高度倍率
//     //     float redjumpheight = 2f; 
        
//         // // red
//         // if (tileColor == new Color(1f,0f,0f) && (activePowerUps & PowerUpType.JumpingHeightRatio) == 0)
//         // {
//         //     Debug.Log("red true");

//         //     // 应用红色加速效果
//         //     temporaryjumpForce *= redjumpheight; 
//         //     activePowerUps |= PowerUpType.JumpingHeightRatio;
//         //     if (spriteRenderer.color != tileColor)
//         //     {
//         //         spriteRenderer.color = tileColor;
//         //         Debug.Log("color:"+spriteRenderer.color);
//         //     }
//         // }

//     //     // orange
//     //     else if (tileColor == new Color(1f,0.5f,0f) && (activePowerUps & PowerUpType.DoubleJump) == 0)
//     //     {
//     //         Debug.Log("orange true");
//     //         doublejump = true;
//     //         // // 应用红色加速效果
//     //         // temporaryjumpForce *= redjumpheight; 
//     //         activePowerUps |= PowerUpType.DoubleJump;
//     //         if (spriteRenderer.color != tileColor)
//     //         {
//     //             spriteRenderer.color = tileColor;
//     //             Debug.Log("color:"+spriteRenderer.color);
//     //         }
//     //     }

//     //     // yellow
//     //     else if (tileColor == new Color(1f,1f,0f) && (activePowerUps & PowerUpType.SpeedBoost) == 0)
//     //     {
//     //         Debug.Log("yellow true");

//     //         // 应用红色加速效果
//     //         temporaryspeed *= yellowBoostMultiplier; 
//     //         activePowerUps |= PowerUpType.SpeedBoost;
//     //         if (spriteRenderer.color != tileColor)
//     //         {
//     //             spriteRenderer.color = tileColor;
//     //             Debug.Log("color:"+spriteRenderer.color);
//     //         }
//     //     }
        
//     //     // red end
//     //     if (tileColor != new Color(1f,0f,0f) && (activePowerUps & PowerUpType.JumpingHeightRatio) != 0) 
//     //     {
//     //         Debug.Log("red Leave");
//     //         temporaryjumpForce /= redjumpheight;
//     //         activePowerUps &= ~PowerUpType.JumpingHeightRatio;
//     //     }
//     //     // orange end
//     //     if (tileColor != new Color(1f,0.5f,0f) && (activePowerUps & PowerUpType.DoubleJump) != 0) 
//     //     {
//     //         Debug.Log("orange Leave");
//     //         doublejump = false;
//     //         activePowerUps &= ~PowerUpType.DoubleJump;
//     //     }
//     //     // yellow end
//     //     if (tileColor != new Color(1f,1f,0f) && (activePowerUps & PowerUpType.SpeedBoost) != 0)
//     //     {
//     //         Debug.Log("yellow Leave");
//     //         temporaryspeed /= 1.5f; 
//     //         activePowerUps &= ~PowerUpType.SpeedBoost;
//     //     } 
//     // }
//     void ApplyTileEffect(Color tileColor)
//     {
//         foreach (var effect in powerUpEffects)
//         {
//             if (effect.Key == tileColor && (activePowerUps & effect.Value.Item1) == 0)
//             {
//                 applyEffects[effect.Key]();
//                 activePowerUps |= effect.Value.Item1;
//                 UpdateSpriteColor(tileColor);
//             }
//             else if (effect.Key != tileColor && (activePowerUps & effect.Value.Item1) != 0)
//             {
//                 removeEffects[effect.Key]();
//                 activePowerUps &= ~effect.Value.Item1;
//             }
//         }
//     }

//     void ApplyRedEffect()
//     {
//         LogEffectStatus(new Color(1f, 0f, 0f), "Enter");
//         temporaryjumpForce *= powerUpEffects[new Color(1f, 0f, 0f)].Item2;
//     }

//     void RemoveRedEffect()
//     {
//         LogEffectStatus(new Color(1f, 0f, 0f), "Leave");
//         temporaryjumpForce /= powerUpEffects[new Color(1f, 0f, 0f)].Item2;
//     }

//     void ApplyOrangeEffect()
//     {
//         LogEffectStatus(new Color(1f, 0.5f, 0f), "Enter");
//         doublejump = true;
//     }

//     void RemoveOrangeEffect()
//     {
//         LogEffectStatus(new Color(1f, 0.5f, 0f), "Leave");
//         doublejump = false;
//     }

//     void ApplyYellowEffect()
//     {
//         LogEffectStatus(new Color(1f, 1f, 0f), "Enter");
//         temporaryspeed *= powerUpEffects[new Color(1f, 1f, 0f)].Item2;
//     }

//     void RemoveYellowEffect()
//     {
//         LogEffectStatus(new Color(1f, 1f, 0f), "Leave");
//         temporaryspeed /= powerUpEffects[new Color(1f, 1f, 0f)].Item2;
//     }

//     void UpdateSpriteColor(Color tileColor)
//     {
//         if (spriteRenderer.color != tileColor)
//         {
//             spriteRenderer.color = tileColor;
//             // Debug.Log("color:" + spriteRenderer.color);
//         }
//     }

// }

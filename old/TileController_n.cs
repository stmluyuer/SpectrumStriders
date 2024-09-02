// using UnityEngine;
// using TileEffects = Game.TileEffects; // 定义别名
// public class TileController : MonoBehaviour
// {
//     public ColorTileData tileData; // 当前方块的颜色数据
//     private SpriteRenderer spriteRenderer;

//     private void ApplyEffect(Collision2D collision)
//     {
//         if (tileData.effect != null && tileData.effect.CanApply(tileData.tileColor))
//         {
//             // 使用完整命名空间调用 Apply 方法
//             TileEffects.ITileEffect tileEffect = tileData.effect; // 使用别名
//             tileEffect.Apply(collision.gameObject);
//         }
//     }
//     void Awake()
//     {
//         spriteRenderer = GetComponent<SpriteRenderer>();
//         // 初始化时应用颜色数据
//         if (tileData != null) 
//         {
//             spriteRenderer.color = tileData.tileColor;
//         }
//     }

//     // 改变方块的颜色和数据
//     public void ChangeColor(Color newColor, ColorTileData newData)
//     {
//         spriteRenderer.color = newColor;
//         tileData = newData;
//     }

//     // 递归地将颜色传播到相邻的相同颜色方块
//     public void SpreadColor(Color targetColor, ColorTileData newData)
//     {
//         // 如果已经是目标颜色，则停止传播
//         if (spriteRenderer.color == targetColor) 
//         {
//             return; 
//         }

//         // 改变当前方块的颜色和数据
//         ChangeColor(targetColor, newData);

//         // 检查四个方向的相邻方块
//         CheckAndSpread(Vector2.up, targetColor, newData);
//         CheckAndSpread(Vector2.down, targetColor, newData);
//         CheckAndSpread(Vector2.left, targetColor, newData);
//         CheckAndSpread(Vector2.right, targetColor, newData);
//     }

//     // 检查指定方向上的方块是否可以传播颜色
//     private void CheckAndSpread(Vector2 direction, Color targetColor, ColorTileData newData)
//     {
//         RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f);
//         if (hit.collider != null && hit.collider.CompareTag("Tile"))
//         {
//             TileController neighbor = hit.collider.GetComponent<TileController>();
//             if (neighbor != null && neighbor.tileData.tileColor == targetColor)
//             {
//                 neighbor.SpreadColor(targetColor, newData);
//             }
//         }
//     }
// }

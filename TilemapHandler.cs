using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapHandler : MonoBehaviour
{
    public Tilemap tilemap; // Tilemap
    public List<(TileBase tile, Vector3Int cellPos)> collidedTiles = new List<(TileBase tile, Vector3Int cellPos)>();
    // private float GravityRatio = -1f;
    public float customGravityScale = 1f; // 重力
    // 用于射线检测的参数
    private float groundCheckDistance =0.3f;
    public LayerMask groundLayer; 

    public void Initialize(Tilemap Tilemap)
    {
        tilemap = Tilemap;
        groundLayer = LayerMask.GetMask("Ground");  
    }
    // 检测碰撞
    public void CheckCollisions()
    {
        // Debug.Log("1111");
        // 获取角色物体的位置
        Vector2 currentPosition = transform.position;
        // 定义检测区域的半径
        float detectionRadius = 0.35f; 
        // 使用 Physics2D 检测物体是否与 Tilemap 碰撞
        Collider2D[] colliders = Physics2D.OverlapCircleAll(currentPosition, detectionRadius);
        foreach (var collider in colliders)
            // 检查碰撞到的物体是否是 Tilemap
            if (((1 << collider.gameObject.layer) & groundLayer) != 0)
            {
                
                // 获取碰撞点
                Vector2 hitPoint = collider.ClosestPoint(currentPosition);
                // 偏移量
                Vector2 direction = (hitPoint - currentPosition).normalized; 
                // 计算偏移之后碰撞的对象
                Vector2 offsetPosition = hitPoint + direction * groundCheckDistance;
                // Debug.Log("碰撞点" + offsetPosition);
                // 获取 Tilemap 中的单元格位置
                Vector3Int cellPos = tilemap.WorldToCell(offsetPosition);
                // collider.sharedMaterial = physicsMaterial;
                // print(collider.sharedMaterial);
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
                    // spriteTile.color = new Color(1f,1f,1f); //修改原有颜色
                    // print("修改了颜色" +" Tile:" + tile +" cellpos坐标："+ cellPos+"cellpos颜色："+tilemap.GetColor(cellPos));
                    // 刷新 Tile 的显示
                    tilemap.RefreshTile(cellPos);
                }
            }
    }
    // public void AddCollidersToTiles()
    // {
    //     // Handle adding colliders to tiles
    // }
}


// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;

// public class TilemapInteraction : MonoBehaviour
// {
//     private SpriteRenderer spriteRenderer;
//     public Tilemap tilemap;
//     private List<(TileBase tile, Vector3Int cellPos)> collidedTiles = new List<(TileBase tile, Vector3Int cellPos)>();
//     private PowerUpManager powerUpManager; // Reference to PowerUpManager

//     void Start()
//     {
//         spriteRenderer = GetComponent<SpriteRenderer>();
//         powerUpManager = GetComponent<PowerUpManager>(); 
//     }

//     void FixedUpdate()
//     {
//         DetectTileCollisions();
//     }

//     void Update()
//     {
//         ProcessTileEffects();
//     }

//     void DetectTileCollisions()
//     {
//         // ... (Logic to detect tile collisions - same as in your original code)
//     }

//     void ProcessTileEffects()
//     {
//         foreach ((TileBase tile, Vector3Int cellPos) in collidedTiles)
//         {
//             if (tile != null)
//             {
//                 Color tileColor = tilemap.GetColor(cellPos);
//                 powerUpManager.ApplyTileEffect(tileColor); 
//                 UpdateSpriteColor(tileColor);
//                 collidedTiles.Remove((tile, cellPos));
//                 break;
//             }
//         }
//     }

//     // ... (RayGetTileUnderFoot, GetTileUnderFoot, GetTileColor methods - same as before)

//     public void ChangeColor(Color newColor)
//     {
//         spriteRenderer.color = newColor;
//     }

//     void UpdateSpriteColor(Color tileColor)
//     {
//         if (spriteRenderer.color != tileColor)
//         {
//             spriteRenderer.color = tileColor;
//         }
//     }
// }

using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{

    public Tilemap tilemap;
    private PlayerMovement playerMovement;
    private PowerUpManager powerUpManager;
    private TilemapHandler tilemapHandler;

    void Start()
    {
        // // 给 player加上3个脚本
        playerMovement = gameObject.AddComponent<PlayerMovement>();
        powerUpManager = gameObject.AddComponent<PowerUpManager>();
        tilemapHandler = gameObject.AddComponent<TilemapHandler>();

        // Initialize components with required data
        
        playerMovement.Initialize(tilemap);
        powerUpManager.Initialize(tilemap);
        
        tilemapHandler.Initialize(tilemap);
    }

    void FixedUpdate()
    {
        tilemapHandler.CheckCollisions();
    }
    void Update()
    {
        playerMovement.UpdateMovement();
        // Call other update methods if needed
    }
}

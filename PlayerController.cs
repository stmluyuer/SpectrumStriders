using Mirror;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : NetworkBehaviour
{
    private PlayerMovement playerMovement;
    private PowerUpManager powerUpManager;
    private TilemapHandler tilemapHandler;
    void Start()
    {
        GameObject tilemapObject = GameObject.FindWithTag("Tilemap");
        Tilemap tilemap = tilemapObject.GetComponent<Tilemap>();
        // // 给 player加上3个脚本
        playerMovement = gameObject.AddComponent<PlayerMovement>();
        powerUpManager = gameObject.AddComponent<PowerUpManager>();
        tilemapHandler = gameObject.AddComponent<TilemapHandler>();

        // Initialize components with required data

        playerMovement.Initialize(tilemap);
        powerUpManager.Initialize(tilemap);
        tilemapHandler.Initialize(tilemap);

        // GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        // NetworkServer.AddPlayerForConnection(conn, player);
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

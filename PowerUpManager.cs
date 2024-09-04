using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PowerUpManager : NetworkBehaviour
{
    public PlayerData playerData;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rbody;
    public Tilemap tilemap;
    public PhysicsMaterial2D physicsMaterial;
    public float temporaryspeed = 10f;
    public bool isOnBlueCube = false;
    public bool isOnMagentaCube = false;
    public float temporaryjumpForce = 6f;
    public bool doublejump;
    public float customGravityScale = 1f; // 重力
    public Vector2 positioshortest;
    public LayerMask groundLayer;
    private TilemapHandler tilemapHandler;
    private List<(GameObject, Vector3Int)> tilemapobject = new List<(GameObject, Vector3Int)>();

    private Dictionary<Color, (PowerUpType, float)> powerUpEffects;
    private Dictionary<Color, System.Action> applyEffects;
    private Dictionary<Color, System.Action> removeEffects;
    public enum PowerUpType
    {
        None = 0,
        JumpingHeightRatio = 1 << 0, // 0000000001 红色
        DoubleJump = 1 << 1, // 0000000010 橙色
        SpeedBoost = 1 << 2, // 0000000100 黄色
        Catapult = 1 << 3, // 0000001000 绿色
        Invinciblea = 1 << 4, // 0000010000 蓝色
        Feather = 1 << 5, // 0000100000 靛色
        Through = 1 << 6, // 0001000000 紫色
        Transfer = 1 << 7, // 0010000000 品红
        GravityInversion = 1 << 8, // 0100000000 白色
        DeGravityInversion = 1 << 9, // 1000000000 黑色
        Invincibleg = 1 << 10, // 100000000000
    }
    // 存储当前激活的效果

    private PowerUpType activePowerUps = PowerUpType.None;

    private Dictionary<Color, string> colorTagMapping = new Dictionary<Color, string>()
    {
        { Color.red, "RedCube" },
        { new Color(1.0f, 0.5f, 0.0f), "OrangeCube" },  // 橙色
        { new Color(1.0f, 1.0f, 0.0f), "YellowCube" },
        { Color.green, "GreenCube" },
        { Color.blue, "BlueCube" },
        { new Color(0.3f, 0.0f, 0.5f), "IndigoCube" },  // 靛色
        { new Color(0.5f, 0.0f, 1f), "PurpleCube" },  // 紫色
        { new Color(1.0f, 0.0f, 1.0f), "MagentaCube" }, // 品红 
        { Color.white, "WhiteCube" },
        { Color.black, "BlackCube" }
    };
    private bool isThroughActive;
    private bool isOnPurpleCube;

    public void Initialize(Tilemap Tilemap)
    {
        tilemap = Tilemap;
        playerData = new PlayerData();
        tilemapHandler = gameObject.GetComponent<TilemapHandler>();
        Vector2 gravity2D = Physics2D.gravity; // 全局重力
        rbody = GetComponent<Rigidbody2D>();
        // customGravityScale = rbody.gravityScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        groundLayer = LayerMask.GetMask("Ground");
        Debug.Log("" + groundLayer.value);
        physicsMaterial = Resources.Load<PhysicsMaterial2D>("Material/BouncyMaterial");
        print(physicsMaterial);
        powerUpEffects = new Dictionary<Color, (PowerUpType, float)>
        {
            { new Color(1f, 0f, 0f), (PowerUpType.JumpingHeightRatio, 1.42f) },
            { new Color(1f, 0.5f, 0f), (PowerUpType.DoubleJump, 0f) },
            { new Color(1f, 1f, 0f), (PowerUpType.SpeedBoost, 1.5f) },
            { new Color(0f, 1f, 0f), (PowerUpType.Catapult, 1f) },
            { new Color(0f, 0f, 1f), (PowerUpType.Invinciblea, 1f) },
            { new Color(0.3f, 0f, 0.5f), (PowerUpType.Feather, 0.3f) },
            { new Color(0.5f, 0f, 1f), (PowerUpType.Through, 1f) },
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
            { new Color(0.5f, 0f, 1f), ApplyPurpleEffect },
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
            { new Color(0.5f, 0f, 1f), RemovePurpleEffect },
            { new Color(1f, 0f, 1f), RemoveMagentaEffect },
            { new Color(1f, 1f, 1f), RemoveWhiteEffect },
            { new Color(0f, 0f, 0f), RemoveBlackEffect }
        };

        AddCollidersToTiles();
    }

    public void ApplyTileEffect(Color tileColor)
    {
        // 检查当前是否在处理“Through”效果
        // 获取角色物体的位置
        Vector2 currentPosition = transform.position;
        // 定义检测区域的半径
        float detectionRadius = 0.35f; 
        Collider2D[] colliders = Physics2D.OverlapCircleAll(currentPosition, detectionRadius);
        foreach (var collider in colliders)
        // 检查碰撞到的物体是否是 Tilemap
            if (collider.gameObject.tag == "PurpleCube" && spriteRenderer.color == new Color(0.5f, 0f, 1f))
            {
                return;
            }

        foreach (var effect in powerUpEffects)
        {
            if (effect.Key == tileColor && (activePowerUps & effect.Value.Item1) == 0)
            {
                applyEffects[effect.Key]();
                activePowerUps |= effect.Value.Item1;
                // print(tileColor);
                UpdateSpriteColor(tileColor);
            }
            else if (effect.Key != tileColor && (activePowerUps & effect.Value.Item1) != 0)
            {
                removeEffects[effect.Key]();
                activePowerUps &= ~effect.Value.Item1;
            }
        }
    }

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

    void LogEffectStatus(Color color, string status)
    {
        string colorName = GetColorName(color);
        // Debug.Log($"{colorName} {status}");
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
        LogEffectStatus(new Color(0f, 1f, 0f), "Enter");
        foreach ((TileBase tile, Vector3Int cellPos) in tilemapHandler.collidedTiles)
        {
            // 获取世界坐标
            Vector3 worldPosition = tilemap.CellToWorld(cellPos);
            // 获取指定位置的Collider
            Collider2D collider = Physics2D.OverlapPoint(worldPosition);

            // if (collider != null)
            // {
            //     // 为该Collider设置物理材质
            //     collider.sharedMaterial = physicsMaterial;
            // }
        }

        // rbody.gravityScale *= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }
    void RemoveGreenEffect()
    {
        LogEffectStatus(new Color(0f, 1f, 0f), "Leave");
        // rbody.gravityScale /= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }

    void ApplyBlueEffect()
    {
        LogEffectStatus(new Color(0f, 0f, 0f), "Enter");
        isOnBlueCube = true;
        // rbody.gravityScale *= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }
    void RemoveBlueEffect()
    {
        LogEffectStatus(new Color(0f, 0f, 0f), "Leave");
        isOnBlueCube = false;
        // rbody.gravityScale /= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }
    void ApplyIndigoEffect()
    {
        LogEffectStatus(new Color(0.3f, 0f, 0.5f), "Enter");
        rbody.gravityScale *= powerUpEffects[new Color(0.3f, 0f, 0.5f)].Item2;
    }

    void RemoveIndigoEffect()
    {
        LogEffectStatus(new Color(0.3f, 0f, 0.5f), "Leave");
        rbody.gravityScale /= powerUpEffects[new Color(0.3f, 0f, 0.5f)].Item2;
    }

    void ApplyPurpleEffect()
    {
        isOnPurpleCube = true;
        LogEffectStatus(new Color(0.5f, 0f, 1f), "Enter");
        foreach (var (gameObject, cellPosition) in tilemapobject)
        {
            if (gameObject.tag == "PurpleCube")
            {
                // gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
                Color cellcolor = tilemap.GetColor(cellPosition);
                cellcolor = new Color(cellcolor.r, cellcolor.g, cellcolor.b, 0.1f);

                // 将新的颜色设置到 Tilemap 的指定位置
                tilemap.SetColor(cellPosition, cellcolor);
            }
        }
        // rbody.gravityScale *= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }

    void RemovePurpleEffect()
    {
        isOnPurpleCube = false;
        LogEffectStatus(new Color(0.5f, 0f, 1f), "Leave");
        List<GameObject> gameObjects = new List<GameObject>();
        LogEffectStatus(new Color(0.5f, 0f, 1f), "Enter");
        foreach (var (gameObject, cellPosition) in tilemapobject)
        {
            if (gameObject.tag == "PurpleCube")
            {
                // gameObject.GetComponent<BoxCollider2D>().isTrigger = false;

            }
        }
        // rbody.gravityScale /= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }

    void ApplyMagentaEffect()
    {
        LogEffectStatus(new Color(1f, 0f, 1f), "Enter");
        isOnMagentaCube = true;
        tilemap.CompressBounds();
        BoundsInt area = tilemap.cellBounds;
        // print(tilemap.cellBounds);
        TileBase[] tileArray = tilemap.GetTilesBlock(area);
        int tileArrayLength = tileArray.Length;
        print(tileArrayLength);
        List<TileBase> magentaTiles = new List<TileBase>();
        List<Vector3Int> magentaTilePositions = new List<Vector3Int>();
        // 遍历所有 Tile
        for (int i = 0; i < tileArrayLength; i++)
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
        print("传送之前的坐标" + rbody.position);
        Vector3Int positioshortestint = FindClosestPoint(magentaTilePositions);
        if (positioshortestint != magentaTilePositions[0])
        print(positioshortestint);
        positioshortest = new Vector2(positioshortestint.x + 0.5f, positioshortestint.y + 1.2f);
        // rbody.position = positioshortest;
    }

    // 找到最近传送点函数
    Vector3Int FindClosestPoint(List<Vector3Int> magentaTilePositions)
    {
        Vector3Int closestPoint = magentaTilePositions[0]; // 初始化为 第一个
        float shortestDistance = Mathf.Infinity; // 确保第一个 Tile 距离会被比较

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

    void RemoveMagentaEffect()
    {
        LogEffectStatus(new Color(1f, 0f, 1f), "Leave");
        isOnMagentaCube = false;
    }

    void ApplyWhiteEffect()
    {
        LogEffectStatus(new Color(1f, 1f, 1f), "Enter");
        rbody.gravityScale = powerUpEffects[new Color(1f, 1f, 1f)].Item2;
    }

    void RemoveWhiteEffect()
    {
        LogEffectStatus(new Color(1f, 1f, 1f), "Leave");
        // rbody.gravityScale /= powerUpEffects[new Color(1f, 1f, 1f)].Item2;
    }

    void ApplyBlackEffect()
    {
        LogEffectStatus(new Color(0f, 0f, 0f), "Enter");
        rbody.gravityScale = powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }

    void RemoveBlackEffect()
    {
        LogEffectStatus(new Color(0f, 0f, 0f), "Leave");
        // rbody.gravityScale /= powerUpEffects[new Color(0f, 0f, 0f)].Item2;
    }

    void UpdateSpriteColor(Color tileColor)
    {
        if (spriteRenderer.color != tileColor)
        {
            spriteRenderer.color = tileColor;
            // Debug.Log("color:" + spriteRenderer.color);
        }
    }

    void AddCollidersToTiles()
    {
        // 获取Tilemap的所有边界
        BoundsInt bounds = tilemap.cellBounds;

        // 遍历Tilemap中的所有位置
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(cellPosition);

                if (tile != null) // 如果Tile不是空白
                {
                    // 创建一个新的GameObject来附加BoxCollider
                    GameObject tileObject = new GameObject("Tile_" + x + "_" + y);
                    tileObject.transform.position = tilemap.CellToWorld(cellPosition) + tilemap.tileAnchor;
                    string colortag = GetColorTag(cellPosition);
                    tileObject.tag = colortag;
                    tileObject.layer = 6;

                    // 添加BoxCollider2D组件
                    BoxCollider2D boxCollider = tileObject.AddComponent<BoxCollider2D>();
                    boxCollider.size = tilemap.cellSize;
                    if (tileObject.tag == "PurpleCube")
                    {
                        tileObject.GetComponent<BoxCollider2D>().isTrigger = true;
                    }
                    // Color tileColor = tilemap.GetColor(cellPosition);
                    // 检查Tile的颜色，如果是绿色则添加PhysicsMaterial2D
                    if (IsTileGreen(cellPosition))
                    {
                        boxCollider.sharedMaterial = physicsMaterial;
                    }
                    // 将新创建的GameObject作为Tilemap的子对象
                    tilemapobject.Add((tileObject, cellPosition));
                    tileObject.transform.parent = tilemap.transform;
                }
            }
        }
        print("tile生成的总数量 :" + tilemapobject.Count);
    }

    bool IsTileGreen(Vector3Int cellPosition)
    {
        // 获取Tile的Sprite
        if (cellPosition != null)
        {
            // 获取Sprite的颜色（假设Sprite的颜色是单一颜色）
            Color color = tilemap.GetColor(cellPosition);
            // 判断颜色是否为绿色（这里假设绿色的RGB值为(0, 1, 0)）
            return color == Color.green;
        }

        return false;
    }

    private string GetColorTag(Vector3Int cellPosition)
    {
        Color tileColor = tilemap.GetColor(cellPosition);

        // 尝试在字典中查找颜色对应的标签
        if (colorTagMapping.TryGetValue(tileColor, out string colorTag))
        {
            return colorTag;
        }
        else
        {
            return "UnknownColor"; // 如果找不到对应的颜色标签，则返回默认标签
        }
    }
}

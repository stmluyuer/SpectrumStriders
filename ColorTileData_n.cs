using UnityEngine;

[CreateAssetMenu(fileName = "New ColorTileData", menuName = "Game/ColorTileData")]
public class ColorTileData : ScriptableObject
{
    public Color tileColor; // 方块的颜色
    public EffectType effectType; // 产生的效果类型
    internal object effect;

    public enum EffectType
    {
        None,
        SpeedBoost,        // 黄色，蓝色
        DoubleJump,       // 橙色
        SlowDown,         //  绿色（弹射可视为负速度）, 靛色（空中降落减半）
        // Heal,           //  暂未涉及
        // Damage,          //  暂未涉及 
        Teleport,         //  品红，部分红色
        PhaseThrough,     //  紫色
        HeightBoost,       //  红色
        AirControlBoost,   //  橙色（空中移速增加）, 靛色（空中保留加速）
    }
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Tile")) // 假设你的方块标签为 "Tile"
    //     {
    //         TileController tileController = collision.gameObject.GetComponent<TileController>();
    //         if (tileController != null)
    //         {
    //             ApplyEffect(tileController.tileData);
    //         }
    //     }
    // }

    private void ApplyEffect(ColorTileData tileData)
    {
        switch (tileData.effectType)
        {
            case ColorTileData.EffectType.SpeedBoost:
                // 应用速度提升效果
                break;
            case ColorTileData.EffectType.DoubleJump:
                // 应用二段跳效果
                break;
            // ... 其他效果 ...
        }
}

}


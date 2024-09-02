// TileEffects.cs
using UnityEngine;

namespace Game.TileEffects
{
    public interface ITileEffect 
    {
        bool CanApply(Color tileColor);
        void Apply(GameObject player); 
    }

    #region Movement Effects

    public class SpeedBoostEffect : ITileEffect
    {
        // 可以设置 SpeedBoostEffect 适用的颜色
        public Color effectColor = Color.green; 

        public bool CanApply(Color tileColor)
        {
            // 判断传入的颜色是否与 effectColor 相同或相似
            return tileColor == effectColor; 
        }

        public void Apply(GameObject player)
        {
            // 在这里添加 SpeedBoostEffect 的具体实现逻辑
            Debug.Log("Speed Boost applied to " + player.name);

            // 例如，获取玩家的移动脚本并增加移动速度
            // PlayerMovement movement = player.GetComponent<PlayerMovement>();
            // if (movement != null)
            // {
            //     movement.speed *= 1.5f; 
            // }
        }
    }

    public class SlowDownEffect : ITileEffect
    {
        // ...
        public void Apply(GameObject player)
        {
            throw new System.NotImplementedException();
        }

        public bool CanApply(Color tileColor)
        {
            throw new System.NotImplementedException();
        }
    }

    #endregion
    
    #region Jump Effects

    public class DoubleJumpEffect : ITileEffect
    {
        // ...
        public void Apply(GameObject player)
        {
            throw new System.NotImplementedException();
        }

        public bool CanApply(Color tileColor)
        {
            throw new System.NotImplementedException();
        }
    }

    #endregion

    // ... 其他效果 ...
}
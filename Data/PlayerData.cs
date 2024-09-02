using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float BaseSpeed = 10f;
    public float Temporaryspeed = 5f;
    public float AccelerationRate = 0.6f; // 每秒加速 60%
    public float MaxSpeedMultiplier = 3f; // 最大速度倍率 300%
    public float TemporaryJumpForce = 6f;
    public int MaxJump = 2;
    public float CoyoteTime = 0.2f; // 土狼时间
    public LayerMask GroundLayer;

    // public PlayerData(int baseSpeed, int temporaryspeed, string accelerationRate)
    // {
    //     BaseSpeed = baseSpeed;
    //     Temporaryspeed = temporaryspeed;
    //     AccelerationRate = accelerationRate;
    // }
}

// using System.Collections.Generic;
// using UnityEngine;

// public class PowerUpManager : MonoBehaviour
// {
//     public enum PowerUpType
//     {
//         None = 0,
//         JumpingHeightRatio = 1 << 0, // 00001 红色
//         DoubleJump = 1 << 1, // 00010 橙色
//         SpeedBoost = 1 << 2, // 00100 黄色
//         Invincible = 1 << 3, // 01000
//     }

//     private PowerUpType activePowerUps = PowerUpType.None;
//     private Dictionary<Color, (PowerUpType, float)> powerUpEffects;
//     private Dictionary<Color, System.Action> applyEffects;
//     private Dictionary<Color, System.Action> removeEffects;

//     private PlayerMovement playerMovement; // Reference to PlayerMovement script

//     void Start()
//     {
//         playerMovement = GetComponent<PlayerMovement>(); 

//         powerUpEffects = new Dictionary<Color, (PowerUpType, float)>
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

//     public void ApplyTileEffect(Color tileColor)
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

//     // ... (Rest of the Apply/Remove effect methods) 

//     void ApplyRedEffect()
//     {
//         LogEffectStatus(new Color(1f, 0f, 0f), "Enter");
//         playerMovement.temporaryjumpForce *= powerUpEffects[new Color(1f, 0f, 0f)].Item2;
//     }

//     void RemoveRedEffect()
//     {
//         LogEffectStatus(new Color(1f, 0f, 0f), "Leave");
//         playerMovement.temporaryjumpForce /= powerUpEffects[new Color(1f, 0f, 0f)].Item2;
//     }

//     void ApplyOrangeEffect()
//     {
//         LogEffectStatus(new Color(1f, 0.5f, 0f), "Enter");
//         playerMovement.doublejump = true;
//     }

//     void RemoveOrangeEffect()
//     {
//         LogEffectStatus(new Color(1f, 0.5f, 0f), "Leave");
//         playerMovement.doublejump = false;
//     }

//     void ApplyYellowEffect()
//     {
//         LogEffectStatus(new Color(1f, 1f, 0f), "Enter");
//         playerMovement.temporaryspeed *= powerUpEffects[new Color(1f, 1f, 0f)].Item2;
//     }

//     void RemoveYellowEffect()
//     {
//         LogEffectStatus(new Color(1f, 1f, 0f), "Leave");
//         playerMovement.temporaryspeed /= powerUpEffects[new Color(1f, 1f, 0f)].Item2;
//     }

//     void UpdateSpriteColor(Color tileColor)
//     {
//         if (playerMovement.spriteRenderer.color != tileColor)
//         {
//             playerMovement.spriteRenderer.color = tileColor;
//             // Debug.Log("color:" + spriteRenderer.color);
//         }
//     }

//     // ... (Other Apply/Remove methods)

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

// }

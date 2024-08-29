// using System.Collections.Generic;
// using Game.TileEffects;
// using UnityEngine;

// public class TileEffectManager : MonoBehaviour
// {
//     public static TileEffectManager Instance { get; private set; }

//     [SerializeField] private List<TileEffect> tileEffects = new(); 

//     private Dictionary<Color, ITileEffect> effectDictionary = new Dictionary<Color, ITileEffect>();

//     void Awake()
//     {
//         // 单例模式
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }

//         // 初始化字典，将颜色与对应的 TileEffect 关联起来
//         InitializeEffectDictionary();
//     }

//     private void InitializeEffectDictionary()
//     {
//         effectDictionary.Clear(); // 清空字典，避免重复添加
//         foreach (var effect in tileEffects)
//         {
//             if (effect != null) 
//             {
//                 // 检查颜色是否已经存在于字典中
//                 if (!effectDictionary.ContainsKey(effect.effectColor))
//                 {
//                     effectDictionary.Add(effect.effectColor, effect);
//                 }
//                 else
//                 {
//                     Debug.LogWarning($"TileEffectManager: Duplicate color detected for effect: {effect.name}. Skipping.");
//                 }
//             }
//         }
//     }

//     public ITileEffect GetEffect(Color color)
//     {
//         if (effectDictionary.TryGetValue(color, out var effect))
//         {
//             return effect;
//         }

//         return null;
//     }
// }

// internal class TileEffect
// {
//     internal Color effectColor;
// }
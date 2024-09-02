using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TagManager : EditorWindow
{
    // 颜色到标签的映射
    private static Dictionary<Color, string> colorTagMapping = new Dictionary<Color, string>()
    {
        { new Color(1f, 0f, 0f), "RedCube" },
        { new Color(1.0f, 0.5f, 0.0f), "OrangeCube" },  // 橙色
        { new Color(1.0f, 1f, 0.0f), "YellowCube" },
        { new Color(0.0f, 1f, 0.0f), "GreenCube" },
        { new Color(0.0f, 0.0f, 1.0f), "BlueCube" },
        { new Color(0.0f, 0.5f, 1.0f), "IndigoCube" },  // 靛色
        { new Color(0.5f, 0.0f, 1f), "PurpleCube" },  // 紫色
        { new Color(1.0f, 0.0f, 1.0f), "MagentaCube" }, // 品红
        { new Color(1.0f, 1.0f, 1.0f), "WhiteCube" },
        { new Color(0.0f, 0.0f, 0.0f), "BlackCube" }
    };

    // 在Unity编辑器中添加一个菜单项
    [MenuItem("Tools/Tag Manager/Generate Tags")]
    public static void GenerateTags()
    {
        foreach (KeyValuePair<Color, string> entry in colorTagMapping)
        {
            AddTag(entry.Value);
        }
    }

    // 检查并添加标签
    private static void AddTag(string tag)
    {
        // 打开标签管理器
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        // 检查标签是否已经存在
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tag)) { found = true; break; }
        }

        // 如果标签不存在，则添加它
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(0);
            newTag.stringValue = tag;
            Debug.Log("Added Tag: " + tag);
        }

        // 应用更改
        tagManager.ApplyModifiedProperties();
    }
}

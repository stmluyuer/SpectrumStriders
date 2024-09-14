using UnityEngine;

public class ScrollingScript : MonoBehaviour
{
    public float scrollSpeed = 5f; 

    void Update()
    {
        // 使背景图片向左移动
        transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime);

        // 当背景图片移动到屏幕左侧边缘时，将其重置到右侧
        if (transform.position.x < -GetComponent<SpriteRenderer>().bounds.size.x)
        {
            transform.position = new Vector3(0, 0, 0);
        }
    }
}

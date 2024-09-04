
using System.Collections.Generic;
using UnityEngine;

public class CustomPrefabPool : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (prefabId == "PlayerPrefab")
        {
            return Instantiate(playerPrefab, position, rotation);
        }
        else
        {
            Debug.LogError("Unknown prefab ID: " + prefabId);
            return null;
        }
    }

    public void Destroy(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}

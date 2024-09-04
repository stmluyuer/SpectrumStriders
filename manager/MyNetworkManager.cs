using Mirror;
using UnityEngine;


public class MyNetworkManager : NetworkManager
{
    
    public GameObject newplayerPrefab;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        
        // 生成玩家对象
        // print("测试生成玩家对象");
        GameObject player = Instantiate(newplayerPrefab,new Vector3(3,3,3), Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Debug.Log("服务器启动");
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        // Debug.Log("客户端已连接到服务器");
    }


    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.Log("客户端已断开连接");
    }
}

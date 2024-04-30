/* 
 * Global.cs
 * @brief: 用于存储全局变量、配置信息的单例。
 * @author: YukinaSora
 * @date: 2023.9.20
 * @version: 0.0.2
 * 
 * --------------------
 * 2023.9.20 v0.0.1 YukinaSora
 * 1.加入受伤时禁用移动的配置项。
 * --------------------
 * 2023.9.27 v0.0.2 YukinaSora
 * 1.加入玩家实例列表与相关操作。
 * 2.继承MonoBehaviour，将Global.cs添加至Main Control。
 * 3.关闭垂直同步、锁定帧率至60。
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// player id与实例的键值对
using PlayerPair = System.Collections.Generic.KeyValuePair<int, PlayerController>;
// 存储player的id与实例的键值对列表
using PlayerList = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, PlayerController>>;




public class Global : MonoBehaviour
{

    public static bool isplayVideo = false;//是否播放结束动画
    private void Awake()
    {
        //垂直同步计数设置为0，才能锁帧，否则锁帧代码无效
        QualitySettings.vSyncCount = 0;
        //设置游戏帧数，否则会出现Time.deltaTime抖动过大导致移动出现明显颤动
        Application.targetFrameRate = 60;
    }

    public static bool isground;
    [Header("玩家设置")]
    // 受伤持续时间，单位：帧，期间禁用移动
    public static int hurtDuration = 30;

    // player实例列表
    public static PlayerList playerList = new();
    // 当前控制玩家的index
    public static int currentPlayerId = 0;

    // 将player实例添加到列表中，返回该实例对应id
    public static int AddPlayer(PlayerController player)
    {
        int id = playerList.Count;
        playerList.Add(new PlayerPair(id, player));
        return id;
    }

    public static void SetCurrentPlayer(int id)
    {
        currentPlayerId = id;
    }

    // 通过id获取player实例
    public static PlayerController CurrentPlayer()
    {
        if (playerList.Count == 0)
            return null;

        return playerList[Global.currentPlayerId].Value;
    }
}

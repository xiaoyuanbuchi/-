/* 
 * Global.cs
 * @brief: ���ڴ洢ȫ�ֱ�����������Ϣ�ĵ�����
 * @author: YukinaSora
 * @date: 2023.9.20
 * @version: 0.0.2
 * 
 * --------------------
 * 2023.9.20 v0.0.1 YukinaSora
 * 1.��������ʱ�����ƶ��������
 * --------------------
 * 2023.9.27 v0.0.2 YukinaSora
 * 1.�������ʵ���б�����ز�����
 * 2.�̳�MonoBehaviour����Global.cs�����Main Control��
 * 3.�رմ�ֱͬ��������֡����60��
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// player id��ʵ���ļ�ֵ��
using PlayerPair = System.Collections.Generic.KeyValuePair<int, PlayerController>;
// �洢player��id��ʵ���ļ�ֵ���б�
using PlayerList = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, PlayerController>>;




public class Global : MonoBehaviour
{

    public static bool isplayVideo = false;//�Ƿ񲥷Ž�������
    private void Awake()
    {
        //��ֱͬ����������Ϊ0��������֡��������֡������Ч
        QualitySettings.vSyncCount = 0;
        //������Ϸ֡������������Time.deltaTime�����������ƶ��������Բ���
        Application.targetFrameRate = 60;
    }

    public static bool isground;
    [Header("�������")]
    // ���˳���ʱ�䣬��λ��֡���ڼ�����ƶ�
    public static int hurtDuration = 30;

    // playerʵ���б�
    public static PlayerList playerList = new();
    // ��ǰ������ҵ�index
    public static int currentPlayerId = 0;

    // ��playerʵ����ӵ��б��У����ظ�ʵ����Ӧid
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

    // ͨ��id��ȡplayerʵ��
    public static PlayerController CurrentPlayer()
    {
        if (playerList.Count == 0)
            return null;

        return playerList[Global.currentPlayerId].Value;
    }
}

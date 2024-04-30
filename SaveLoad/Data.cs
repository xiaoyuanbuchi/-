using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public string sceneToSave;

    public Dictionary<string, SerializeVector3> characterPosDict = new Dictionary<string, SerializeVector3>();//�ֵ�

    public Dictionary<string, float> floatSaveData = new Dictionary<string, float>();

    public void SaveGameScene(GameSceneSO saveScene)
    {
        sceneToSave = JsonUtility.ToJson(saveScene);//��savesacne���һ��string����
        Debug.Log(sceneToSave);

    }

    public GameSceneSO GetSaveScene()//�����л�
    {
        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();//����һ��ʵ��newscene
        JsonUtility.FromJsonOverwrite(sceneToSave, newScene);//��scentosave����newscene
        return newScene;//�����³�������ʵ���Ƿ����˴����ĳ���
    }


    public class SerializeVector3
     {
        public float x, y, z;

        public SerializeVector3(Vector3 pos)
        {
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }


}

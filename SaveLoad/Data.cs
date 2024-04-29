using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public string sceneToSave;

    public Dictionary<string, SerializeVector3> characterPosDict = new Dictionary<string, SerializeVector3>();//字典

    public Dictionary<string, float> floatSaveData = new Dictionary<string, float>();

    public void SaveGameScene(GameSceneSO saveScene)
    {
        sceneToSave = JsonUtility.ToJson(saveScene);//把savesacne变成一个string类型
        Debug.Log(sceneToSave);

    }

    public GameSceneSO GetSaveScene()//反序列化
    {
        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();//创建一个实例newscene
        JsonUtility.FromJsonOverwrite(sceneToSave, newScene);//用scentosave覆盖newscene
        return newScene;//返回新场景，其实就是返回了储存点的场景
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

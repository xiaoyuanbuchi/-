using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using System.IO;

[DefaultExecutionOrder(-100)]//数值越小表示越优先执行，让该脚本在第一时间执行
public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header("事件监听")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;

    private List<ISaveable> saveableList = new List<ISaveable>();//创建一个列表来储存

    private Data saveData;

    private string jsonFolder;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);//确保只有yige

        saveData = new Data();

        jsonFolder = Application.persistentDataPath+"/SAVE DATA/";

        ReadSavedData();
    }
    public void OnEnable()
    {
        saveDataEvent.OnEventRaised += Save;
        loadDataEvent.OnEventRaised += Load;

    }

    public void OnDisable()
    {
        saveDataEvent.OnEventRaised -= Save;
        loadDataEvent.OnEventRaised -= Load;
    }

    private void Update()
    {
        if(Keyboard.current.lKey.wasPressedThisFrame)
        {
            Load();
        }
    }
    public void RegisterSaveData(ISaveable saveable)//通知
    {
        if(!saveableList.Contains(saveable))//是否包含，不包含就add一个
        {
            saveableList.Add(saveable);
        }
    }

    public void UnRegisterSaveData(ISaveable saveable)//注销
    {
        saveableList.Remove(saveable);//从列表中移除掉
    }

    public void Save()
    {
        foreach(var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }

        var resultPath = jsonFolder + "data.sav";

        var jsonData = JsonConvert.SerializeObject(saveData);//把数据变成string存起来
        
        if(!File.Exists(resultPath))//没有这个文件那就创建一个
        {
            Directory.CreateDirectory(jsonFolder);
        }
        File.WriteAllText(resultPath, jsonData);//写入
    }

    public void Load()
    {

        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }

    private void ReadSavedData()//读取
    {
        var resultPath = jsonFolder + "data.sav";
        if (File.Exists(resultPath))
        {
            var stringData = File.ReadAllText(resultPath);
            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);//反序列化
            saveData = jsonData;
        }

    }
}

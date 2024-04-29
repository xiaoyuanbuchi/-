using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using System.IO;

[DefaultExecutionOrder(-100)]//��ֵԽС��ʾԽ����ִ�У��øýű��ڵ�һʱ��ִ��
public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header("�¼�����")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;

    private List<ISaveable> saveableList = new List<ISaveable>();//����һ���б�������

    private Data saveData;

    private string jsonFolder;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);//ȷ��ֻ��yige

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
    public void RegisterSaveData(ISaveable saveable)//֪ͨ
    {
        if(!saveableList.Contains(saveable))//�Ƿ��������������addһ��
        {
            saveableList.Add(saveable);
        }
    }

    public void UnRegisterSaveData(ISaveable saveable)//ע��
    {
        saveableList.Remove(saveable);//���б����Ƴ���
    }

    public void Save()
    {
        foreach(var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }

        var resultPath = jsonFolder + "data.sav";

        var jsonData = JsonConvert.SerializeObject(saveData);//�����ݱ��string������
        
        if(!File.Exists(resultPath))//û������ļ��Ǿʹ���һ��
        {
            Directory.CreateDirectory(jsonFolder);
        }
        File.WriteAllText(resultPath, jsonData);//д��
    }

    public void Load()
    {

        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }

    private void ReadSavedData()//��ȡ
    {
        var resultPath = jsonFolder + "data.sav";
        if (File.Exists(resultPath))
        {
            var stringData = File.ReadAllText(resultPath);
            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);//�����л�
            saveData = jsonData;
        }

    }
}

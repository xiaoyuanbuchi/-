using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable //���泡���ӿ�
{
    DataDifination GetDataID();
    void RegisterSaveData()
    {
        DataManager.instance.RegisterSaveData(this);//ע��
    }
    void UnRegisterSaveData()//ע��
    {
        DataManager.instance.UnRegisterSaveData(this);
    }

    void GetSaveData(Data data);

    void LoadData(Data data);
}

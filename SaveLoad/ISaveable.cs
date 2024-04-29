using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable //保存场景接口
{
    DataDifination GetDataID();
    void RegisterSaveData()
    {
        DataManager.instance.RegisterSaveData(this);//注册
    }
    void UnRegisterSaveData()//注销
    {
        DataManager.instance.UnRegisterSaveData(this);
    }

    void GetSaveData(Data data);

    void LoadData(Data data);
}

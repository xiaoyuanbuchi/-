using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDifination : MonoBehaviour
{
    public PersistentType persistentType;
    //����ÿ��player�����˶�һ�޶���id
    public string ID;

    public void OnValidate()
    {
        if(persistentType==PersistentType.ReadWrite)
        {
            if (ID == string.Empty)
                ID = System.Guid.NewGuid().ToString();
        }
        else
        {
            ID = string.Empty;
        }
      
    }
}

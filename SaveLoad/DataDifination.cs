using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDifination : MonoBehaviour
{
    public PersistentType persistentType;
    //创建每个player，敌人独一无二的id
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

/* 
 * Dialog.cs
 * @brief: 用于控制交互文本。
 * @author: YukinaSora
 * @date: 2023.10.23
 * @version: 0.0.1
 * 
 * --------------------
 * 2023.10.23 v0.0.1 YukinaSora
 * 1.添加对剧本的读取方法。
 * 2.完成对DialogUnit的状态控制。
 * 3.完成对Interact操作的响应。
 */

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;


public class Dialog : MonoBehaviour
{
    private Dictionary<string, GameObject> Units = new();
    private DialogUnit UnitLast = null;
    private List<DialogNode> Data = null;
    private Vector3[] Positions = {new(), new()};

    private string BasePath = "/Resources/Dialogs/";
    private string DialogPath = "__ObjectName__/__ObjectDialog_0__.json";

    private int CurrentDialogIndex = 0; // Dialog文本指针。
    public bool ReadDialogSucceed = false; // 正确读取交互文本后置为true，否则拒绝显示文本。
    public bool Finished = false; // 交互文本显示完成状态。

    public bool AnimationFinished = false; // 是否正在播放文字效果。

    public InteractableObject.InteractCallback Callback = null;

    public void SetCallback(InteractableObject.InteractCallback callback)
    {
        Callback = callback;
    }

    // 定位文本框。
    // positions[0]为玩家位置，之后为NPC0, NPC1...位置。
    public void SetPositions(Vector3[] positions)
    {
        Positions = positions;
    }

    public void Load(string scriptPath)
    {
        // 当文件夹名或文件名为空时使用默认路径。
        bool pathEmpty = scriptPath.Length == 0;
        bool floderEmpty = scriptPath.IndexOf("/") == 0;
        if (!pathEmpty && !floderEmpty)
            DialogPath = scriptPath;

        ReadDialogSucceed = LoadData();
        if (!ReadDialogSucceed)
            return;
        
        CurrentDialogIndex = 0;
    }

    public void Next()
    {
        if (!ReadDialogSucceed)
            return;

        // 如果正在播放动画，则直接显示全部文本。
        if (AnimationFinished)
        {
            UnitLast.ShowAll();
            return;
        }

        if (CurrentDialogIndex >= Data.Count)
        {
            Finished = true;
            return;
        }

        UnitLast?.Hide();

        DialogNode data = Data[CurrentDialogIndex];
        DialogUnit unit = Units[data.Name].GetComponent<DialogUnit>();
        Debug.Log("Dialog: Next dialog: " + data.Name + " " + data.Text);
        unit.SetText(data.Text);
        AnimationFinished = true;
        UnitLast = unit;

        CurrentDialogIndex++;
    }


    // File manage
    private bool LoadData()
    {
        string path = Application.dataPath + BasePath + DialogPath;
        string data = ReadFile(path);
        if (data.Length == 0)
        {
            Debug.LogError("Dialog: Load dialog failed in path: " + path);
            return false;
        }

        //Debug.Log(data);
        Data = JsonUtility.FromJson<DialogNodeJson>(data).DialogList;

        Debug.Log("Dialog: Load dialog succeed: " + path);

        // 将json中出现的所有object添加到Units键值对中。
        List<string> objects = new();
        for (int i = 0; i < Data.Count; i++)
        {
            string name = "DialogCanvas_" + i.ToString();
            GameObject unit = GameObject.Find(name);

            if (objects.IndexOf(Data[i].Name) != -1)
                continue;

            objects.Add(Data[i].Name);
            Units.Add(Data[i].Name, unit);

            //Debug.Log("Dialog: Add new object: " + name + ", " + unit);
        }

        // 定位
        int j = 1;
        foreach (var unit in Units)
        {
            DialogUnit _unit = unit.Value.GetComponent<DialogUnit>();

            // Position[0]为玩家位置，之后为NPC位置。
            if ("$Player" == unit.Key)
                _unit.SetPosition(Positions[0]);
            else
                _unit.SetPosition(Positions[j++]);
            _unit.Hide();
        }
        return true;
    }

    private string ReadFile(string path)
    {
        Debug.Log("Dialog: Load dialog: " + path);
        string data;
        using (StreamReader sr = File.OpenText(path))
        {
            data = sr.ReadToEnd();
            sr.Close();
        }
        return data;
    }
}


[System.Serializable]
public class DialogNode
{
    public string Name;
    public string Text;
}

// JsonUtility不能直接返回数组，只能返回对象，故创建此对象模板。
[System.Serializable]
public class DialogNodeJson
{
    public List<DialogNode> DialogList;
}
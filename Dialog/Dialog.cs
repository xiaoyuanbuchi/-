/* 
 * Dialog.cs
 * @brief: ���ڿ��ƽ����ı���
 * @author: YukinaSora
 * @date: 2023.10.23
 * @version: 0.0.1
 * 
 * --------------------
 * 2023.10.23 v0.0.1 YukinaSora
 * 1.��ӶԾ籾�Ķ�ȡ������
 * 2.��ɶ�DialogUnit��״̬���ơ�
 * 3.��ɶ�Interact��������Ӧ��
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

    private int CurrentDialogIndex = 0; // Dialog�ı�ָ�롣
    public bool ReadDialogSucceed = false; // ��ȷ��ȡ�����ı�����Ϊtrue������ܾ���ʾ�ı���
    public bool Finished = false; // �����ı���ʾ���״̬��

    public bool AnimationFinished = false; // �Ƿ����ڲ�������Ч����

    public InteractableObject.InteractCallback Callback = null;

    public void SetCallback(InteractableObject.InteractCallback callback)
    {
        Callback = callback;
    }

    // ��λ�ı���
    // positions[0]Ϊ���λ�ã�֮��ΪNPC0, NPC1...λ�á�
    public void SetPositions(Vector3[] positions)
    {
        Positions = positions;
    }

    public void Load(string scriptPath)
    {
        // ���ļ��������ļ���Ϊ��ʱʹ��Ĭ��·����
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

        // ������ڲ��Ŷ�������ֱ����ʾȫ���ı���
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

        // ��json�г��ֵ�����object��ӵ�Units��ֵ���С�
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

        // ��λ
        int j = 1;
        foreach (var unit in Units)
        {
            DialogUnit _unit = unit.Value.GetComponent<DialogUnit>();

            // Position[0]Ϊ���λ�ã�֮��ΪNPCλ�á�
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

// JsonUtility����ֱ�ӷ������飬ֻ�ܷ��ض��󣬹ʴ����˶���ģ�塣
[System.Serializable]
public class DialogNodeJson
{
    public List<DialogNode> DialogList;
}
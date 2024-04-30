/* 
 * InteractableObject.cs
 * @brief: ���ڿ��Ƶ�ǰ����Ľ���״̬��
 * @author: YukinaSora
 * @date: 2023.10.23
 * @version: 0.0.1
 * 
 * --------------------
 * 2023.10.13 v0.0.1 YukinaSora
 * 1.����������Ա�ṹ��
 */

using UnityEngine;

public class InteractableObject : MonoBehaviour
{

    [Header("��������")]
    public bool Interactable = true;

    // �����Ի��ľ籾���ƣ������ڷ��ʾ籾��·�� Assets/Resources/Dialogs/$ScriptName/��
    public string ScriptName = "";

    // �����Ի�����Ӧ���������ڶ�ȡ�籾 Assets/Resources/Dialogs/$scriptName/$ScriptAction.json
    public string ScriptAction = "";

    public string ScriptPath()
    {
        return ScriptName + "/" + ScriptAction + ".json";
    }

    // һ�ν����Ի���Ϻ���еĻص������������Ķ���籾�����һ�仰����á�
    public delegate void InteractCallback();
    public InteractCallback Callback = () => { };
}

/* 
 * InteractableObject.cs
 * @brief: 用于控制当前对象的交互状态。
 * @author: YukinaSora
 * @date: 2023.10.23
 * @version: 0.0.1
 * 
 * --------------------
 * 2023.10.13 v0.0.1 YukinaSora
 * 1.建立基本成员结构。
 */

using UnityEngine;

public class InteractableObject : MonoBehaviour
{

    [Header("交互设置")]
    public bool Interactable = true;

    // 交互对话的剧本名称，是用于访问剧本的路径 Assets/Resources/Dialogs/$ScriptName/。
    public string ScriptName = "";

    // 交互对话的相应操作，用于读取剧本 Assets/Resources/Dialogs/$scriptName/$ScriptAction.json
    public string ScriptAction = "";

    public string ScriptPath()
    {
        return ScriptName + "/" + ScriptAction + ".json";
    }

    // 一次交互对话完毕后进行的回调操作，将在阅读完剧本的最后一句话后调用。
    public delegate void InteractCallback();
    public InteractCallback Callback = () => { };
}

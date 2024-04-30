/* 
 * InteractCheck.cs
 * @brief: 用于检测交互物体的控件。
 * @author: YukinaSora
 * @date: 2023.10.16
 * @version: 0.0.1
 * 
 * --------------------
 * 2023.10.16 v0.0.1 YukinaSora
 * 1.建立基本射线检测操作。
 */

using UnityEngine;

public class InteractCheck
{
    public Collider2D Target = null;
    private float CheckRange = 3.0f;

    public bool Check(Vector2 position, Vector2 direction)
    {
        bool returnFlag = position == null || direction == null;
        if (returnFlag)
        {
            Debug.Log("InteractCheck: position or direction undefined.");
            return false;
        }

        // 可通过f检测到Bounds或Player，待进一步处理相关碰撞箱
        // raycast = Physics2D.Raycast(position, direction, checkRange);

        //Ray ray = new Ray(position, direction);
        int layerMask = 1 << 9; // 仅检测Layer 9(InteractableObject)上的物体
        RaycastHit2D raycast = Physics2D.Raycast(position, direction, CheckRange, layerMask);
        Target = raycast.collider;

        // 该对象无交互模块
        InteractableObject interactableObject = Target?.GetComponent<InteractableObject>();
        if (interactableObject == null)
        {
            Target = null;
            return false;
        }

        if (Target != null)
        {
            //Debug.Log("InteractCheck: " + target.name + ", position: " + target.transform.position
            //        + ", interactable: " + target.GetComponent<InteractableObject>().interactable);

            // Debug.DrawLine(position, collider.transform.position, Color.yellow); // 显示射线
            return interactableObject.Interactable;
        }

        return false;
    }
}

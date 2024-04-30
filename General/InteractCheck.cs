/* 
 * InteractCheck.cs
 * @brief: ���ڼ�⽻������Ŀؼ���
 * @author: YukinaSora
 * @date: 2023.10.16
 * @version: 0.0.1
 * 
 * --------------------
 * 2023.10.16 v0.0.1 YukinaSora
 * 1.�����������߼�������
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

        // ��ͨ��f��⵽Bounds��Player������һ�����������ײ��
        // raycast = Physics2D.Raycast(position, direction, checkRange);

        //Ray ray = new Ray(position, direction);
        int layerMask = 1 << 9; // �����Layer 9(InteractableObject)�ϵ�����
        RaycastHit2D raycast = Physics2D.Raycast(position, direction, CheckRange, layerMask);
        Target = raycast.collider;

        // �ö����޽���ģ��
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

            // Debug.DrawLine(position, collider.transform.position, Color.yellow); // ��ʾ����
            return interactableObject.Interactable;
        }

        return false;
    }
}

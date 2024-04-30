using UnityEngine;

public class TestNPC : MonoBehaviour
{
    InteractableObject interactableObject;

    private void Awake()
    {
        interactableObject = GetComponent<InteractableObject>();
        interactableObject.Callback = InteractCallback;
    }

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ��д�ص�����
    public void InteractCallback()
    {
        interactableObject.ScriptAction = "AnyDialogAfterCallback";
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{


    private bool isOKeyPressed = false;
    [Header("UI���")]
    public Text textLabel;
    public Image faceImage;

    [Header("�ı��ļ�")]
    public TextAsset textFile;//�ɹ�text���ļ����Ի��ļ���
    public int index;
    public float textSpeed;


    [Header("ͷ��")]
    public Sprite face01, face02;
    bool textFinished;//�ж���һ�������Ƿ��Ѿ���ʾ����
    List<string> textList = new List<string>();
   
    void Awake()
    {
        GetTextFromFile(textFile);//����
        
    }

    private void OnEnable()
    {
        //textLabel.text = textList[index];
        //index++;
        //  StartCoroutine(SetTextUI());
        textFinished = true;
    }
    // Update is called once per frame
    void Update()
    {

        if (Keyboard.current.oKey.isPressed && index == textList.Count)
        {
            gameObject.SetActive(false);


            index = 0;
            isOKeyPressed = true;

            return;
        }

        if (Keyboard.current.oKey.isPressed && !isOKeyPressed && textFinished)
        {
            //textLabel.text = textList[index];//������o����˳����ʾtext����
            //index++;
            StartCoroutine(SetTextUI());
            isOKeyPressed = true;

        }
        else if (!Keyboard.current.oKey.isPressed)
        {
            // �� "o" ���ɿ�ʱ��������Ӧ״̬
            isOKeyPressed = false;
        }

    }

    void GetTextFromFile(TextAsset file)
    {
        textList.Clear();//����б��е���ʷ��¼����Ȼÿ�ζ�ȡ�ļ���Խ��Խ��
        index = 0;//��ų�ʼ��


        var lineDate=textFile.text.Split('\n');//��txt�ı��ָ��ÿһ��
        
        foreach(var line in lineDate)
        {
            textList.Add(line);//��ÿһ�ж�ȡ��textlist�б�����
        }
    }

    IEnumerator SetTextUI()
    {
        textFinished = false;
        textLabel.text = "";

        switch(textList[index].Trim().ToString())
        {
            case "A":
                faceImage.sprite = face01;
                index++;
                break;
            case "B":
                faceImage.sprite = face02;
                index++;
                break;

        }
        for(int i = 0; i < textList[index].Length;i++)
        {
            textLabel.text += textList[index][i];

            yield return new WaitForSeconds(textSpeed);
        }
        textFinished = true;
        index++;
    }
}

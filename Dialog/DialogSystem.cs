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
    [Header("UI组件")]
    public Text textLabel;
    public Image faceImage;

    [Header("文本文件")]
    public TextAsset textFile;//可挂text等文件（对话文件）
    public int index;
    public float textSpeed;


    [Header("头像")]
    public Sprite face01, face02;
    bool textFinished;//判断这一行文字是否已经显示完了
    List<string> textList = new List<string>();
   
    void Awake()
    {
        GetTextFromFile(textFile);//调用
        
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
            //textLabel.text = textList[index];//当按下o键，顺序显示text内容
            //index++;
            StartCoroutine(SetTextUI());
            isOKeyPressed = true;

        }
        else if (!Keyboard.current.oKey.isPressed)
        {
            // 当 "o" 键松开时，重置响应状态
            isOKeyPressed = false;
        }

    }

    void GetTextFromFile(TextAsset file)
    {
        textList.Clear();//清空列表中的历史记录，不然每次读取文件会越来越大
        index = 0;//编号初始化


        var lineDate=textFile.text.Split('\n');//把txt文本分割成每一行
        
        foreach(var line in lineDate)
        {
            textList.Add(line);//把每一行读取到textlist列表里面
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

/* 
 * DialogUnit.cs
 * @brief: ������ʾ�ı��Ŀؼ���
 * @author: YukinaSora
 * @date: 2023.10.23
 * @version: 0.0.2
 * 
 * --------------------
 * 2023.10.16 v0.0.1 YukinaSora
 * 1.������ʼ���ı���λ�á���С�Ȳ�����
 * 2.�����ӡ����ʾЧ����
 * --------------------
 * 2023.10.23 v0.0.2 YukinaSora
 * 1.���ShowAll()�����������ж϶�����һ������ʾ�����ı���
 * 2.���Hide(), Show()������������ʾ�������ı���
 */

using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using static Unity.Burst.Intrinsics.X86.Avx;

public class DialogUnit : MonoBehaviour
{
    private Canvas canvas;
    private Transform background;
    private TMP_Text TMP;
    private Transform textarea;
    private string text;

    private float transparent = .7f;        // ����͸����
    private float margin = .2f;             // ���ұ߾�
    private float maxWidth = 10f - .2f * 2; // �����
    private Vector2 position;               // ê��

    // �ı������췽��
    private ExtendDirection extendDirection = ExtendDirection.Up;        

    public float width;
    public float height;
    public float textInterval = .1f;        // ��ӡ��Ч�����
    public float fadeInterval = .05f;       // ����Ч��ˢ�¼��
    public float fadeSpeed = .05f;          // ����Ч���ٶ�


    // �ı������췽��
    public enum ExtendDirection
    {
        Down = -1,  // ��������
        Center = 0, // ����
        Up = 1      // ��������
    }

    void Awake()
    {
        canvas = gameObject.GetComponent<Canvas>();
        background = transform.Find("Background");
        TMP = transform.Find("Textarea").GetComponent<TMP_Text>();
        textarea = transform.Find("Textarea");

        var sprite = background.GetComponent<SpriteRenderer>();
        var color = sprite.color;
        sprite.color = new Color(color.r, color.b, color.g, transparent); // ���ñ���͸����

        // ǿ���ڵ�ǰ֡��Ⱦ��������ܻ���ʵ����ı�
        Canvas.ForceUpdateCanvases();
        TMP.ForceMeshUpdate();

        Resize();
    }

    // Start is called before the first frame update
    void Start()
    {
        // SetText("��ã�����__HeroName__����ʲô�ܰﵽ�����");
    }

    public void SetPosition(Vector2 position)
    {
        this.position = position;
        Resize();
    }

    private IEnumerator FadeEffect(bool _in = true)
    {
        var sprite = background.GetComponent<SpriteRenderer>();
        var color = sprite.color;
        float timer = 0;
        sprite.color = new Color(color.r, color.b, color.g, (_in ? 0 : transparent));
        while (color.a < (_in ? transparent : 0))
        {
            timer += Time.deltaTime;

            if (timer >= fadeInterval)
            {
                color.a += fadeSpeed * (_in ? 1 : -1);
                sprite.color = color;
                timer -= fadeInterval;
            }

            yield return null;
        }
    }

    // ��ӡ��Ч��
    private IEnumerator PlayPrinterEffect(string str)
    {
        // ���������ַ�
        //text.maxVisibleCharacters = 0;
        //text.ForceMeshUpdate();
        TMP.text = "<space=.5>";
        Resize();
        Show();

        float timer = 0;
        int i = 1;
        TMP.text = string.Format("<nobr>{0}</nobr>", str[..i]);
        do
        {
            timer += Time.deltaTime;
            if (timer >= textInterval)
            {
                timer -= textInterval;
                i++;

                // ���ÿո���
                TMP.text = string.Format("<nobr>{0}</nobr>", str[..i].Replace(" ", "<space=.5>"));
                //Debug.Log(string.Format("<nobr>{0}</nobr>", str[..i]));
                Resize();
            }

            yield return null;
        }
        while (str[..i] != str);

        AnimationFinished();
    }

    public void SetText(string str)
    {
        // str = string.Format("<nobr>{0}</nobr>", str); // ���÷ִʻ���
        // text.SetText(str);
        // StartCoroutine(FadeEffect());
        text = str;
        StartCoroutine("PlayPrinterEffect", str);

        // Debug.Log("Height: " + height + ", Width: " + width);
        // Resize();
    }

    public void ShowAll()
    {
        StopCoroutine("PlayPrinterEffect");
        AnimationFinished();
        TMP.text = string.Format("<nobr>{0}</nobr>", text.Replace(" ", "<space=.5>"));
        Resize();
    }

    // ���¿�ߣ���Ӧ�����Ի����С
    private void Resize()
    {
        RectTransform transform = textarea.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(maxWidth, 1);

        width = Mathf.Min(TMP.preferredWidth, maxWidth); // ����Ӧ��ȣ��������������
        height = TMP.preferredHeight;

        transform.sizeDelta = new Vector2(width, 1);

        Vector2 sizeCanvas     = new Vector2(maxWidth + margin * 2, height + margin * 2);
        Vector2 sizeBackground = new Vector2(width    + margin * 2, height + margin * 2); // �ճ����ұ߾�

        //Debug.Log("set size: " + sizeCanvas);
        canvas.GetComponent<RectTransform>().sizeDelta = sizeCanvas;
        background.localScale = sizeBackground;

        float y = (int)extendDirection * height / 2;
        this.transform.position = position + new Vector2(0, y);
    }

    public void SetMaxWidth(float maxWidth)
    {
        this.maxWidth = maxWidth - 2 * margin;
        Resize();
    }

    public void SetExtendDirection(ExtendDirection direction)
    {
        this.extendDirection = direction;
        Resize();
    }

    public void Exit()
    {
        StartCoroutine(FadeEffect(false));
    }

    public void Hide()
    {
        canvas.GetComponent<RectTransform>().localScale = Vector3.zero;
    }

    public void Show()
    {
        canvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    private void AnimationFinished()
    {
        GameObject.Find("Dialog").GetComponent<Dialog>().AnimationFinished = false;
    }
}

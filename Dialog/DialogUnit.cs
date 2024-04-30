/* 
 * DialogUnit.cs
 * @brief: 用于显示文本的控件。
 * @author: YukinaSora
 * @date: 2023.10.23
 * @version: 0.0.2
 * 
 * --------------------
 * 2023.10.16 v0.0.1 YukinaSora
 * 1.建立初始化文本、位置、大小等操作。
 * 2.加入打印机显示效果。
 * --------------------
 * 2023.10.23 v0.0.2 YukinaSora
 * 1.添加ShowAll()方法，用于中断动画、一次性显示所有文本。
 * 2.添加Hide(), Show()方法，用于显示、隐藏文本。
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

    private float transparent = .7f;        // 背景透明度
    private float margin = .2f;             // 左右边距
    private float maxWidth = 10f - .2f * 2; // 最大宽度
    private Vector2 position;               // 锚点

    // 文本框延伸方向
    private ExtendDirection extendDirection = ExtendDirection.Up;        

    public float width;
    public float height;
    public float textInterval = .1f;        // 打印机效果间隔
    public float fadeInterval = .05f;       // 淡入效果刷新间隔
    public float fadeSpeed = .05f;          // 淡入效果速度


    // 文本框延伸方向
    public enum ExtendDirection
    {
        Down = -1,  // 向下延伸
        Center = 0, // 居中
        Up = 1      // 向上延伸
    }

    void Awake()
    {
        canvas = gameObject.GetComponent<Canvas>();
        background = transform.Find("Background");
        TMP = transform.Find("Textarea").GetComponent<TMP_Text>();
        textarea = transform.Find("Textarea");

        var sprite = background.GetComponent<SpriteRenderer>();
        var color = sprite.color;
        sprite.color = new Color(color.r, color.b, color.g, transparent); // 设置背景透明度

        // 强制在当前帧渲染，否则可能会访问到空文本
        Canvas.ForceUpdateCanvases();
        TMP.ForceMeshUpdate();

        Resize();
    }

    // Start is called before the first frame update
    void Start()
    {
        // SetText("你好，我是__HeroName__，有什么能帮到你的吗？");
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

    // 打印机效果
    private IEnumerator PlayPrinterEffect(string str)
    {
        // 隐藏所有字符
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

                // 禁用空格换行
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
        // str = string.Format("<nobr>{0}</nobr>", str); // 禁用分词换行
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

    // 更新宽高，并应用至对话框大小
    private void Resize()
    {
        RectTransform transform = textarea.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(maxWidth, 1);

        width = Mathf.Min(TMP.preferredWidth, maxWidth); // 自适应宽度，但不超过最大宽度
        height = TMP.preferredHeight;

        transform.sizeDelta = new Vector2(width, 1);

        Vector2 sizeCanvas     = new Vector2(maxWidth + margin * 2, height + margin * 2);
        Vector2 sizeBackground = new Vector2(width    + margin * 2, height + margin * 2); // 空出左右边距

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

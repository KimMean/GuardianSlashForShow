using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(RectTransform))]
public class CustomSizeFitter : MonoBehaviour
{
    private RectTransform m_Rect;
    private RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }

    [SerializeField] private RectOffset padding;

    [SerializeField] private Text targetText;
    [SerializeField] private string content;

    private void OnEnable()
    {
        UpdateSize();
    }

    public void SetContent(string str)
    {
        content = str;
        targetText.text = content.Replace("\\n", "\n");
        UpdateSize();  // ���� ����� ������ ũ�⸦ ������Ʈ
    }
    

    private void UpdateSize()
    {
        // ���̾ƿ� ���� ���� (Text ������Ʈ�� ũ�⸦ ����)
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        // Text�� preferredWidth�� preferredHeight�� ����Ͽ� ũ�� ���
        float width = targetText.preferredWidth + padding.left + padding.right;
        float height = targetText.preferredHeight + padding.top + padding.bottom;

        //Debug.Log($"Width : {targetText.preferredWidth}, padding left : {padding.left}, Padding Right : {padding.right}");
        //Debug.Log($"Height : {targetText.preferredHeight}, padding left : {padding.top}, Padding Right : {padding.bottom}");
        // RectTransform ũ�� ������Ʈ
        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
    }

    private void OnValidate()
    {
        targetText.text = content.Replace("\\n", "\n");
        UpdateSize();  // ���� ����� ������ ũ�⸦ ������Ʈ
        //Debug.Log("OnValidate");
    }

}


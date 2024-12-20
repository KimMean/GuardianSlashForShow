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
        UpdateSize();  // 값이 변경될 때마다 크기를 업데이트
    }
    

    private void UpdateSize()
    {
        // 레이아웃 강제 갱신 (Text 컴포넌트의 크기를 재계산)
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        // Text의 preferredWidth와 preferredHeight를 사용하여 크기 계산
        float width = targetText.preferredWidth + padding.left + padding.right;
        float height = targetText.preferredHeight + padding.top + padding.bottom;

        //Debug.Log($"Width : {targetText.preferredWidth}, padding left : {padding.left}, Padding Right : {padding.right}");
        //Debug.Log($"Height : {targetText.preferredHeight}, padding left : {padding.top}, Padding Right : {padding.bottom}");
        // RectTransform 크기 업데이트
        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
    }

    private void OnValidate()
    {
        targetText.text = content.Replace("\\n", "\n");
        UpdateSize();  // 값이 변경될 때마다 크기를 업데이트
        //Debug.Log("OnValidate");
    }

}


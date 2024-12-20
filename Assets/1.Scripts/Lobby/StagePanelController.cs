using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

using Image = UnityEngine.UI.Image;

public class StagePanelController : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] Transform Content;

    [SerializeField] GameObject StageBlockPrefab;
    ScrollRect _ScrollRect;

    GameObject[] Stages;

    int TargetStage = 1;
    int PrevTargetStage = 1;
    float InterpolationSpeed = 10.0f;
    float Padding = 250.0f;
    float Offset = 10.0f;

    bool IsDrag = false;
    bool IsInterpolation = false;

    private void Awake()
    {
        Stages = new GameObject[GameManager.Instance.GetMaxStageCount()];
        _ScrollRect = GetComponent<ScrollRect>();
        //Debug.Log(Screen.dpi);
        //Content.GetComponent<HorizontalLayoutGroup>().padding.left = Screen.safeArea.width / 2 - 150;
        float halfWidth = FindObjectOfType<Canvas>().GetComponent<RectTransform>().rect.width / 2;
        int blockCenter = (int)halfWidth - 100;
        Content.GetComponent<HorizontalLayoutGroup>().padding.left = blockCenter;
        Content.GetComponent<HorizontalLayoutGroup>().padding.right = blockCenter;

    }

    // Start is called before the first frame update
    void Start()
    {
        int maxStage = GameManager.Instance.GetMaxStageCount();
        int clearStage = GameManager.Instance.GetClearStage();
        for (int i = 0; i < maxStage; i++)
        {
            Stages[i] = Instantiate(StageBlockPrefab, Content);
            Stages[i].gameObject.name = "Stage" + i.ToString();
            Stages[i].GetComponent<Image>().sprite = BlockDataManager.Instance.GetStageBlockSprite(i);
            Stages[i].transform.GetChild(1).GetComponent<Text>().text = "Stage" + (i + 1).ToString("00");

            if (i <= clearStage)
                Stages[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        //Debug.Log(Content.GetComponent<RectTransform>().sizeDelta);
    }


    /// <summary>
    /// �������� ����� �巡���� �Ŀ� ��Ȯ�� ���������� ����ŵ�ϴ�.
    /// </summary>
    void Update()
    {
        // �ӵ��� �� �پ����� ��
        // 
        //if(scrollRect.velocity.x != 0 && Mathf.Abs(scrollRect.velocity.x) < 100)
        // �巡�װ� ������ �̲��������� �̵��� ��
        if(!IsDrag && IsInterpolation)
        {
            if(Mathf.Abs(_ScrollRect.velocity.x) < 300.0f )
            {
                float localPos = Content.localPosition.x;
                float targetPos = TargetStage * -Padding + Padding;

                if (Mathf.Abs(localPos - targetPos) < Offset)
                {
                    Content.localPosition = new Vector2(targetPos, 0);
                    _ScrollRect.velocity = Vector2.zero;
                    IsInterpolation = false;
                }
                else
                {
                    float xPos = Mathf.Lerp(localPos, targetPos, Time.deltaTime * InterpolationSpeed);
                    Content.localPosition = new Vector2(xPos, 0);
                }
            }
        }
    }

    /// <summary>
    /// ���� ���õ� Ȯ���� ���� ���������� �����մϴ�.
    /// </summary>
    public void OnScrollViewValueChange()
    {
        // Ÿ���� ���������� ����
        TargetStage = (int)Mathf.Round(Mathf.Abs(Content.localPosition.x) / Padding) + 1;
        if (TargetStage > 80) TargetStage = 80;

        if(TargetStage != PrevTargetStage)
        {
            PrevTargetStage = TargetStage;
            SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.StageBelt);
        }
    }

    public int GetTargetStage()
    {
        return TargetStage;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDrag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDrag = false;
        IsInterpolation = true;
    }
}

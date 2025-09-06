using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;                   // 고스트 아이템 프리팹
    [SerializeField] private GameObject ghostItem;  // 고스트 아이템
    [SerializeField] private Canvas canvas;         // 캔버스

    public void Awake()
    {
        // 캔버스 할당
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("begin");

        // 고스트 아이템 생성
        ghostItem = Instantiate(itemPrefab, canvas.transform);

        // 고스트와 원본의 sprite 같게 하기
        ghostItem.GetComponent<Image>().sprite = itemPrefab.GetComponent<Image>().sprite;

        // 고스트와 원본의 사이즈 같게 하기
        RectTransform itemRect = GetComponent<RectTransform>();
        RectTransform ghostRect = ghostItem.GetComponent<RectTransform>();

        ghostRect.sizeDelta = itemRect.sizeDelta;
        ghostRect.localScale = new Vector3(2, 2, 2);
        ghostRect.anchoredPosition = itemRect.anchoredPosition;
        ghostRect.pivot = itemRect.pivot;

        // 고스트의 위치를 마우스와 같게끔 하기
        ghostItem.transform.position = eventData.position;

        // 고스트가 Drag 되도록 rayCast 타겟 해제
        ghostItem.GetComponent<CanvasGroup>().blocksRaycasts = false;

        // 원본 아이템이 드래그중임을 표시하기 위해 투명하게
        ghostItem.GetComponent<CanvasGroup>().alpha = 0.4f;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostItem != null)
        {
            ghostItem.transform.position = eventData.position;
        }

        Debug.Log("ing");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ghostItem != null)
        {
            Destroy(ghostItem);
        }

        Debug.Log("end");
    }
}


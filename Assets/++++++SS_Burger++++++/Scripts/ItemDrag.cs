using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;                   // ��Ʈ ������ ������
    [SerializeField] private GameObject ghostItem;  // ��Ʈ ������
    [SerializeField] private Canvas canvas;         // ĵ����

    public void Awake()
    {
        // ĵ���� �Ҵ�
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("begin");

        // ��Ʈ ������ ����
        ghostItem = Instantiate(itemPrefab, canvas.transform);

        // ��Ʈ�� ������ sprite ���� �ϱ�
        ghostItem.GetComponent<Image>().sprite = itemPrefab.GetComponent<Image>().sprite;

        // ��Ʈ�� ������ ������ ���� �ϱ�
        RectTransform itemRect = GetComponent<RectTransform>();
        RectTransform ghostRect = ghostItem.GetComponent<RectTransform>();

        ghostRect.sizeDelta = itemRect.sizeDelta;
        ghostRect.localScale = new Vector3(2, 2, 2);
        ghostRect.anchoredPosition = itemRect.anchoredPosition;
        ghostRect.pivot = itemRect.pivot;

        // ��Ʈ�� ��ġ�� ���콺�� ���Բ� �ϱ�
        ghostItem.transform.position = eventData.position;

        // ��Ʈ�� Drag �ǵ��� rayCast Ÿ�� ����
        ghostItem.GetComponent<CanvasGroup>().blocksRaycasts = false;

        // ���� �������� �巡�������� ǥ���ϱ� ���� �����ϰ�
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


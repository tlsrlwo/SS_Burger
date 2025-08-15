using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class StationHighlighter : MonoBehaviour
{
    [Header("������ �����̼�")]
    [SerializeField] private Transform highlighted;
    [SerializeField] private Transform selected;

    [Header("�ܰ��� ������")]
    [SerializeField] private Color _outlineColor = Color.green;
    [SerializeField] private float _outlineWidth = 7.0f;

    private Transform lastHighlight;
    private RaycastHit hit;

    private void Update()
    {
        // UI ���� ���� �� ����
        if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            ClearHighlight();
            return;
        }

        var cam = Camera.main;
        if (cam == null) return;                    // ���� ����

        // ����
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // �⺻ : �̹� ������ �ĺ� �������� ����
        highlighted = null;

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            var hitObject = hit.transform;

            // �����̼��̰� selected �̶� �ٸ� ���� 
            if (hitObject.CompareTag("Station") && hitObject != selected)
            {
                highlighted = hitObject;
            }
        }

        // ����� �ٲ�������� ���
        if (lastHighlight != highlighted)
        {
            // ���� �� ����
            if (lastHighlight != null && lastHighlight.TryGetComponent<Outline>(out var prev))
            {
                prev.enabled = false;
            }

            // �� ��� �ѱ�
            if (highlighted != null)
            {
                var outline = highlighted.GetComponent<Outline>();
                if(outline == null) outline = highlighted.gameObject.AddComponent<Outline>();

                // �� / �β� ���� ���� -> �״��� �ѱ� (�� �ܰ��� ����)
                outline.OutlineColor = _outlineColor;
                outline.OutlineWidth = _outlineWidth;
                outline.enabled = true;
            }

            lastHighlight = highlighted;
        }

        // ����
        if (Input.GetMouseButtonDown(1))
        {
            if(highlighted)                 // ���콺 �����µ� highlighted �� null �� �ƴ�
            {
                if(selected && selected.TryGetComponent<Outline>(out var oldSelection))     // �̹� selected �� ������ 
                {
                    oldSelection.enabled = false;                                           // �� selected �� �ܰ����� ���ְ�
                }

                selected = hit.transform;                                                   // ���Ӱ� ������ ������Ʈ��  selected �� ����
                var selectedStationOutline = selected.GetComponent<Outline>() ?? selected.gameObject.AddComponent<Outline>();

                selectedStationOutline.OutlineColor = _outlineColor;
                selectedStationOutline.OutlineWidth = _outlineWidth;
                selectedStationOutline.enabled = true;

                // lastHighlight �� �ߺ��� �� ������
                lastHighlight = null;
            }
            else                                                                            // highlited �� ����
            {
                if(selected && selected.TryGetComponent<Outline>(out var s))                // selected �� ���� ��(�����̼��� �ƴ� ���� Ŭ����) 
                {
                    s.enabled = false;                                                      // �ܰ����� ����
                }

                selected = null;                                                            // selected �� ���� (null)
            }
        }
    }

    private void ClearHighlight()
    {
        // ������ ���õȰ� ������
        if (lastHighlight != null && lastHighlight.TryGetComponent<Outline>(out var prevHighlight))
        {
            prevHighlight.enabled = false;  
        }

        // ����, ���� �� ���� (null)
        highlighted = null;
        lastHighlight= null;
    }

}

#region ("���� �ڵ�")
/*
  [SerializeField] private Transform highlighted;
    [SerializeField] private Transform selected;
    [SerializeField] private RaycastHit raycastHit;


    void Update()
    {
        // ���̶���Ʈ
        if (highlighted != null)
        {
            highlighted.gameObject.GetComponent<Outline>().enabled = false;
            highlighted = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlighted = raycastHit.transform;

            if (highlighted.CompareTag("Station") && highlighted != selected)           // raycastHit �� ��ȯ �� �� ���õ��ִ� ���°� �ƴϸ� ���̶���Ʈ��
            {
                var outline = highlighted.GetComponent<Outline>();

                if (outline == null)
                {
                    outline = highlighted.gameObject.AddComponent<Outline>();
                }

                // �Ӽ� ���� ����
                outline.OutlineColor = Color.magenta;
                outline.OutlineWidth = 7.0f;

                // �� ���� �ѱ�
                outline.enabled = true;

                #region
                //// �̰Ŷ� �������� ���� var�κ�
                //if (highlighted.gameObject.GetComponent<Outline>() != null)             // ���̶���Ʈ�� ������Ʈ�� Outline �� ������
                //{
                //    highlighted.gameObject.GetComponent<Outline>().enabled = true;
                //    highlighted.gameObject.GetComponent<Outline>().OutlineColor = Color.magenta;
                //    highlighted.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
                //}
                //else
                //{
                //    Outline outline = highlighted.gameObject.AddComponent<Outline>();
                //    outline.enabled = true;
                //    highlighted.gameObject.GetComponent<Outline>().OutlineColor = Color.magenta;
                //    highlighted.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
                //}
                #endregion
            }
            else
            {
                highlighted = null;
            }
        }

        // ����
        if (Input.GetMouseButtonDown(1))
        {
            if (highlighted)                                 // ���̶���Ʈ �� �Ϳ� ���콺�� ��Ŭ���ϸ�
            {
                if (selected != null)
                {
                    selected.gameObject.GetComponent<Outline>().enabled = false;
                }
                selected = raycastHit.transform;
                selected.gameObject.GetComponent<Outline>().enabled = true;
                highlighted = null;
            }
            else                                            // ���̶���Ʈ �� �� ���� ����, Station �̿��� ���� ������
            {
                if (selected)                               // selected ������Ʈ�� �����ϸ�
                {
                    selected.gameObject.GetComponent<Outline>().enabled = false;        // selected�� Outline �� enabled = false ��Ŵ
                    selected = null;                                                    // selected �� null��
                }
            }
        }
    }
 */
#endregion
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class StationHighlighter : MonoBehaviour
{
    [Header("선택한 스테이션")]
    [SerializeField] private Transform highlighted;
    [SerializeField] private Transform selected;

    [Header("외곽선 설정값")]
    [SerializeField] private Color _outlineColor = Color.green;
    [SerializeField] private float _outlineWidth = 7.0f;

    private Transform lastHighlight;
    private RaycastHit hit;

    private void Update()
    {
        // UI 위면 해제 후 종료
        if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            ClearHighlight();
            return;
        }

        var cam = Camera.main;
        if (cam == null) return;                    // 버그 방지

        // 레이
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // 기본 : 이번 프레임 후보 없음으로 시작
        highlighted = null;

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            var hitObject = hit.transform;

            // 스테이션이고 selected 이랑 다를 때만 
            if (hitObject.CompareTag("Station") && hitObject != selected)
            {
                highlighted = hitObject;
            }
        }

        // 대상이 바뀌었을때만 토글
        if (lastHighlight != highlighted)
        {
            // 이전 것 끄기
            if (lastHighlight != null && lastHighlight.TryGetComponent<Outline>(out var prev))
            {
                prev.enabled = false;
            }

            // 새 대상 켜기
            if (highlighted != null)
            {
                var outline = highlighted.GetComponent<Outline>();
                if(outline == null) outline = highlighted.gameObject.AddComponent<Outline>();

                // 색 / 두께 먼저 지정 -> 그다음 켜기 (흰 외곽선 방지)
                outline.OutlineColor = _outlineColor;
                outline.OutlineWidth = _outlineWidth;
                outline.enabled = true;
            }

            lastHighlight = highlighted;
        }

        // 선택
        if (Input.GetMouseButtonDown(1))
        {
            if(highlighted)                 // 마우스 눌렀는데 highlighted 가 null 이 아님
            {
                if(selected && selected.TryGetComponent<Outline>(out var oldSelection))     // 이미 selected 가 있으면 
                {
                    oldSelection.enabled = false;                                           // 그 selected 는 외곽선을 꺼주고
                }

                selected = hit.transform;                                                   // 새롭게 선택한 오브젝트를  selected 로 지정
                var selectedStationOutline = selected.GetComponent<Outline>() ?? selected.gameObject.AddComponent<Outline>();

                selectedStationOutline.OutlineColor = _outlineColor;
                selectedStationOutline.OutlineWidth = _outlineWidth;
                selectedStationOutline.enabled = true;

                // lastHighlight 가 중복될 수 있으니
                lastHighlight = null;
            }
            else                                                                            // highlited 가 없고
            {
                if(selected && selected.TryGetComponent<Outline>(out var s))                // selected 가 있을 때(스테이션이 아닌 곳을 클릭함) 
                {
                    s.enabled = false;                                                      // 외곽선을 꺼줌
                }

                selected = null;                                                            // selected 를 없앰 (null)
            }
        }
    }

    private void ClearHighlight()
    {
        // 이전에 선택된게 있으면
        if (lastHighlight != null && lastHighlight.TryGetComponent<Outline>(out var prevHighlight))
        {
            prevHighlight.enabled = false;  
        }

        // 이전, 현재 다 제거 (null)
        highlighted = null;
        lastHighlight= null;
    }

}

#region ("기존 코드")
/*
  [SerializeField] private Transform highlighted;
    [SerializeField] private Transform selected;
    [SerializeField] private RaycastHit raycastHit;


    void Update()
    {
        // 하이라이트
        if (highlighted != null)
        {
            highlighted.gameObject.GetComponent<Outline>().enabled = false;
            highlighted = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlighted = raycastHit.transform;

            if (highlighted.CompareTag("Station") && highlighted != selected)           // raycastHit 에 반환 된 게 선택되있는 상태가 아니면 하이라이트됨
            {
                var outline = highlighted.GetComponent<Outline>();

                if (outline == null)
                {
                    outline = highlighted.gameObject.AddComponent<Outline>();
                }

                // 속성 먼저 지정
                outline.OutlineColor = Color.magenta;
                outline.OutlineWidth = 7.0f;

                // 그 다음 켜기
                outline.enabled = true;

                #region
                //// 이거랑 같은거임 위에 var부분
                //if (highlighted.gameObject.GetComponent<Outline>() != null)             // 하이라이트된 오브젝트에 Outline 이 있으면
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

        // 선택
        if (Input.GetMouseButtonDown(1))
        {
            if (highlighted)                                 // 하이라이트 된 것에 마우스를 우클릭하면
            {
                if (selected != null)
                {
                    selected.gameObject.GetComponent<Outline>().enabled = false;
                }
                selected = raycastHit.transform;
                selected.gameObject.GetComponent<Outline>().enabled = true;
                highlighted = null;
            }
            else                                            // 하이라이트 된 걸 선택 안함, Station 이외의 것을 선택함
            {
                if (selected)                               // selected 오브젝트가 존재하면
                {
                    selected.gameObject.GetComponent<Outline>().enabled = false;        // selected의 Outline 을 enabled = false 시킴
                    selected = null;                                                    // selected 는 null로
                }
            }
        }
    }
 */
#endregion
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemDrop : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image droppableArea;               // 조리 가능 구역 표시 Ui image 컴포넌트
    [SerializeField] private GameObject cookingFoodPrefab;      // Instantiate 할 prefab

    [SerializeField] private Transform[] spawnPoints;
    

    private int spawnIndex = 0;
    public int SpawnIndex => spawnIndex;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedItem = eventData.pointerDrag;

        if (draggedItem != null && draggedItem.CompareTag("Patty") && spawnIndex < spawnPoints.Length)
        {
            // 디버그
            Debug.Log("Patty is in the area");

            // 아이템 해당 위치에 생성
            Instantiate(cookingFoodPrefab, spawnPoints[spawnIndex].position, cookingFoodPrefab.transform.rotation);

            // 인덱스 값 증가
            spawnIndex++;
        }
        else if (spawnIndex >= spawnPoints.Length)
        {
            Debug.Log("더 이상 구울 수 없다");
            // 더 이상 구울 수 없다는 UI 표시
        }
       
    }
}

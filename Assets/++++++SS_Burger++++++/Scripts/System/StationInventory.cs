using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "newStationInventory", menuName = "Game/StationInventory")]
public class StationInventory : ScriptableObject
{
    [System.Serializable]
    public class ItemSlot
    {
        public string itemId;                   // 아이템의 이름
        public int quantity;                    // 개수 
    }

    [SerializeField] private List<ItemSlot> items = new List<ItemSlot>();

    // 런타임 전용 딕셔너리
    [SerializeField]
    private readonly Dictionary<string, int> map = new Dictionary<string, int>();

    // Ui 에서 구독 가능한 변경 알림
    public event Action OnChanged;

    // 초기화 , 동기화


    // 리스트 -> 맵 (에디터 설정값을 런타임 맵으로 반영)
    public void InitFromList()
    {
        map.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            var slot = items[i];
            if (slot != null && !string.IsNullOrEmpty(slot.itemId))
            {
                map[slot.itemId] = Mathf.Max(0, slot.quantity);
            }
        }
        SyncListFromMap();   // 정리 겸 0 이하 제거
        OnChanged?.Invoke();
    }

    // 맵 -> 리스트 ( 익스펙터 확인용 동기화 )
    public void SyncListFromMap()
    {
        items.Clear();
        foreach (var kv in map)
        {
            if (kv.Value > 0)
            {
                items.Add(new ItemSlot { itemId = kv.Key, quantity = kv.Value });
            }
        }
    }

    // 조회, 조작
    public int GetQuantity(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return 0;
        return map.TryGetValue(itemId, out var qty) ? qty : 0;
    }

    // 양수면 추가 (+), 음수면 제거 (-) 처럼 사용 가능
    public void Add(string itemId, int amount)
    {
        if (string.IsNullOrEmpty(itemId) || amount == 0) return;

        int cur = GetQuantity(itemId);
        int next = cur + amount;

        if (next <= 0)
        {
            if (map.ContainsKey(itemId)) map.Remove(itemId);
        }
        else
        {
            map[itemId] = next;
        }

        SyncListFromMap();
        OnChanged?.Invoke();
    }

    public bool Remove(string itemId, int amount)
    {
        if (string.IsNullOrEmpty(itemId) || amount <= 0) return false;

        int cur = GetQuantity(itemId);
        if (cur < amount) return false;

        int next = cur - amount;
        if (next <= 0) map.Remove(itemId);
        else map[itemId] = next;

        SyncListFromMap();
        OnChanged?.Invoke();

        return true;
    }

    // 종료 시 전체 초기화
    public void ResetAll()
    {
        map.Clear();
        items.Clear();
        OnChanged?.Invoke();
    }

#if UNITY_EDITOR
    // 에디터에서 값 수정 시 리스트-맵 정합성 유지(선택적)
    private void OnValidate() // ?? 이건 무슨 함수지
    {
        // 플레이 중 변경은 런타임 로직이 처리하므로, 에디터 상태에서만 정리
        if(!Application.isPlaying)
        {
            InitFromList();
        }
    }
#endif

    // 저장 / 로드





}

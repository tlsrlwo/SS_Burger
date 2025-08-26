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
        public string itemId;                   // �������� �̸�
        public int quantity;                    // ���� 
    }

    [SerializeField] private List<ItemSlot> items = new List<ItemSlot>();

    // ��Ÿ�� ���� ��ųʸ�
    [SerializeField]
    private readonly Dictionary<string, int> map = new Dictionary<string, int>();

    // Ui ���� ���� ������ ���� �˸�
    public event Action OnChanged;

    // �ʱ�ȭ , ����ȭ


    // ����Ʈ -> �� (������ �������� ��Ÿ�� ������ �ݿ�)
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
        SyncListFromMap();   // ���� �� 0 ���� ����
        OnChanged?.Invoke();
    }

    // �� -> ����Ʈ ( �ͽ����� Ȯ�ο� ����ȭ )
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

    // ��ȸ, ����
    public int GetQuantity(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return 0;
        return map.TryGetValue(itemId, out var qty) ? qty : 0;
    }

    // ����� �߰� (+), ������ ���� (-) ó�� ��� ����
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

    // ���� �� ��ü �ʱ�ȭ
    public void ResetAll()
    {
        map.Clear();
        items.Clear();
        OnChanged?.Invoke();
    }

#if UNITY_EDITOR
    // �����Ϳ��� �� ���� �� ����Ʈ-�� ���ռ� ����(������)
    private void OnValidate() // ?? �̰� ���� �Լ���
    {
        // �÷��� �� ������ ��Ÿ�� ������ ó���ϹǷ�, ������ ���¿����� ����
        if(!Application.isPlaying)
        {
            InitFromList();
        }
    }
#endif

    // ���� / �ε�





}

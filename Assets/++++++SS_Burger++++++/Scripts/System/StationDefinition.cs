using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StationType
{
    Grill,
    Frier,
    Beverage,
    Bun,
    Chicken,
    Custard,
    Expo
}

[CreateAssetMenu(fileName = "newStationInventory", menuName = "Game/StationDefinition")]
public class StationDefinition : ScriptableObject
{
    [Header("ID / �з�")]
    [SerializeField] private string stationId = "StationID";        // �����̼��� Id ���� ����
    [SerializeField] private StationType stationType = StationType.Grill;

    [Header("ǥ�ÿ� �̸�")]
    [SerializeField] private string displayName = "StationName";    // �����̼��� �̸��� ����

    [Header("���� �ð� �⺻��(��)")]    
    [SerializeField] private float baseCookTimeSeconds = 45f;       // ���׷��̵� �� ���� �ʱⰪ

    // ��/��� ����, �������� � ���⿡
    [SerializeField] private string inputItemId = "input item name";            // ���� �ϱ� �� ������ ������
    [SerializeField] private string outputItemId = "output item name";          // ���� �Ϸ� �� ������ ������

    // �ܺ� ���� ������Ƽ
    public string StationId => string.IsNullOrEmpty(stationId) ? name : stationId;
    public StationType StationType => stationType;
    public string DisplayName => displayName;
    public float BaseCookTimeSeconds => baseCookTimeSeconds;
    public string InputItemId => inputItemId;
    public string OutputItemId => outputItemId;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newStationInventory", menuName = "Game/StationDefinition")]
public class StationDefinition : ScriptableObject
{
    [Header("ID/ǥ��")]
    [SerializeField] private string stationId = "grill";   // ��: "grill", "fryer", "beverage"
    [SerializeField] private string displayName = "Grill";

    [Header("�⺻ ���� �ð�(��) - ���׷��̵� �� ������")]
    [Min(0f)]
    [SerializeField] private float baseCookTimeSeconds = 45f;

    // �ʿ� �� ��/��� ����, �������� � ���⿡
    // [SerializeField] private string inputItemId = "patty_raw";
    // [SerializeField] private string outputItemId = "patty_cooked";

    public string StationId => stationId;
    public string DisplayName => displayName;
    public float BaseCookTimeSeconds => baseCookTimeSeconds;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newStationInventory", menuName = "Game/StationDefinition")]
public class StationDefinition : ScriptableObject
{
    [Header("ID/표기")]
    [SerializeField] private string stationId = "grill";   // 예: "grill", "fryer", "beverage"
    [SerializeField] private string displayName = "Grill";

    [Header("기본 조리 시간(초) - 업그레이드 전 원본값")]
    [Min(0f)]
    [SerializeField] private float baseCookTimeSeconds = 45f;

    // 필요 시 입/출력 정의, 실패조건 등도 여기에
    // [SerializeField] private string inputItemId = "patty_raw";
    // [SerializeField] private string outputItemId = "patty_cooked";

    public string StationId => stationId;
    public string DisplayName => displayName;
    public float BaseCookTimeSeconds => baseCookTimeSeconds;

}

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
    [Header("ID / 분류")]
    [SerializeField] private string stationId = "StationID";        // 스테이션의 Id 값을 지정
    [SerializeField] private StationType stationType = StationType.Grill;

    [Header("표시용 이름")]
    [SerializeField] private string displayName = "StationName";    // 스테이션의 이름을 지정

    [Header("조리 시간 기본값(초)")]    
    [SerializeField] private float baseCookTimeSeconds = 45f;       // 업그레이드 전 가장 초기값

    // 입/출력 정의, 실패조건 등도 여기에
    [SerializeField] private string inputItemId = "input item name";            // 조리 하기 전 상태의 아이템
    [SerializeField] private string outputItemId = "output item name";          // 조리 완료 후 상태의 아이템

    // 외부 참조 프로퍼티
    public string StationId => string.IsNullOrEmpty(stationId) ? name : stationId;
    public StationType StationType => stationType;
    public string DisplayName => displayName;
    public float BaseCookTimeSeconds => baseCookTimeSeconds;
    public string InputItemId => inputItemId;
    public string OutputItemId => outputItemId;

}

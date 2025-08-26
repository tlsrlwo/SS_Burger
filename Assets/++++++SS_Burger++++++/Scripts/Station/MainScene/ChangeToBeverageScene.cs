using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class ChangeToBeverageScene : StationBase
{
    [Header("베버리지")]
    [SerializeField] private string stationName = "Beverage";        // 디버그 식별용

    [SerializeField] private float gizmoRadius = 0.5f;
    protected override void Awake()
    {
        base.Awake();
        // 필요 시 그릴 전용 초기화 추가 가능(예열상태 등)
    }
    public override void OnPlayerArrive(GameObject player, ArrivalDetector.ArrivalEvents events)
    {
        if (!IsAvailable)
        {
            // 
            Debug.LogWarning($"' {StationID} ' 은(는) 현재 사용 불가 상태입니다. 도착 처리 무시.");
            return;
        }

        if (ApproachPoint == null)
        {
            Debug.LogWarning($"' {StationID} ' 에 ApproachPoint 가 지정되지 않았습니다");
        }

        var playerName = player != null ? player.name : "Unknown";

        Debug.Log($"[BeverageStation] ' {playerName} ' 이(가) ' {stationName} '(StationID = {StationID}' 에 도착함." +
            $"AP={(ApproachPoint != null ? ApproachPoint.name : "null")}, Pos = {events.Position}, t = {events.Time:0.00}");

        // Todo 
        // 씬 전환 트리거

    }

    public override void OnPlayerLeave(GameObject player)
    {
        var playerName = player != null ? player.name : "Unknown";

        Debug.Log($"[BeverageStation] Player '{playerName}' 가 Beverage '{stationName}' (StationID='{StationID}') 에서 이탈함. ");

        // 필요 시 정리 로직 (상호작용 해제, UI 닫기 등)
    }

    private void OnDrawGizmos()
    {

        if (ApproachPoint != null)
        {
            Gizmos.DrawWireSphere(ApproachPoint.position, gizmoRadius);
            Gizmos.DrawLine(transform.position, ApproachPoint.position);
        }
    }
}

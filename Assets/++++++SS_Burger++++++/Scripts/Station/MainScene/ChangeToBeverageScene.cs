using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class ChangeToBeverageScene : StationBase
{
    [Header("��������")]
    [SerializeField] private string stationName = "Beverage";        // ����� �ĺ���

    [SerializeField] private float gizmoRadius = 0.5f;
    protected override void Awake()
    {
        base.Awake();
        // �ʿ� �� �׸� ���� �ʱ�ȭ �߰� ����(�������� ��)
    }
    public override void OnPlayerArrive(GameObject player, ArrivalDetector.ArrivalEvents events)
    {
        if (!IsAvailable)
        {
            // 
            Debug.LogWarning($"' {StationID} ' ��(��) ���� ��� �Ұ� �����Դϴ�. ���� ó�� ����.");
            return;
        }

        if (ApproachPoint == null)
        {
            Debug.LogWarning($"' {StationID} ' �� ApproachPoint �� �������� �ʾҽ��ϴ�");
        }

        var playerName = player != null ? player.name : "Unknown";

        Debug.Log($"[BeverageStation] ' {playerName} ' ��(��) ' {stationName} '(StationID = {StationID}' �� ������." +
            $"AP={(ApproachPoint != null ? ApproachPoint.name : "null")}, Pos = {events.Position}, t = {events.Time:0.00}");

        // Todo 
        // �� ��ȯ Ʈ����

    }

    public override void OnPlayerLeave(GameObject player)
    {
        var playerName = player != null ? player.name : "Unknown";

        Debug.Log($"[BeverageStation] Player '{playerName}' �� Beverage '{stationName}' (StationID='{StationID}') ���� ��Ż��. ");

        // �ʿ� �� ���� ���� (��ȣ�ۿ� ����, UI �ݱ� ��)
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

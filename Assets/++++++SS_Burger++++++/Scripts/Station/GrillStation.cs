using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GrillStation : StationBase
{
    [Header("�׸�")]
    [SerializeField] private string grillName = "Grill";        // ����� �ĺ���

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

        Debug.Log($"[GrillStation] ' {playerName} ' ��(��) ' {grillName} '(StationID = {StationID}' �� ������." + 
            $"AP={(ApproachPoint != null ? ApproachPoint.name : "null")}, Pos = {events.Position}, t = {events.Time:0.00}");
        
        // Todo 
        // �� ��ȯ Ʈ����
        
    }

    public override void OnPlayerLeave(GameObject player)
    {
        var playerName = player != null ? player.name : "Unknown";

        Debug.Log($"[GrillStation] Player '{playerName}' �� Grill '{grillName}' (StationID='{StationID}') ���� ��Ż��. ");

        // �ʿ� �� ���� ���� (��ȣ�ۿ� ����, UI �ݱ� ��)
    }

    private void OnDrawGizmos()
    {

        if(ApproachPoint != null)
        {
            Gizmos.DrawWireSphere(ApproachPoint.position, 0.15f);
            Gizmos.DrawLine(transform.position, ApproachPoint.position);
        }
    }

}

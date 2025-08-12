using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStation
{
    public string StationID { get; }                   // �����̼�, �� �� �ĺ���
    
    public Transform Root { get; }                    // �����̼��� ��Ʈ ��ġ

    public Transform ApproachPoint { get; }            // �÷��̾��� ���� ��ġ

    public bool IsAvailable { get; }                   // ���� ��� ���� ����

    public bool CanPlayerUse(GameObject player);       // �÷��̾� ��� ���� �˻�

    public void OnPlayerArrive(GameObject player, ArrivalDetector.ArrivalEvents events);   // �÷��̾� ���� ó��

    public void OnPlayerLeave(GameObject player);      // �÷��̾� ��Ż ó��
}



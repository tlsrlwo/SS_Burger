using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStation
{
    public string StationID { get; }                   // 스테이션, 씬 등 식별자
    
    public Transform Root { get; }                    // 스테이션의 루트 위치

    public Transform ApproachPoint { get; }            // 플레이어의 접근 위치

    public bool IsAvailable { get; }                   // 현재 사용 가능 여부

    public bool CanPlayerUse(GameObject player);       // 플레이어 사용 가능 검사

    public void OnPlayerArrive(GameObject player, ArrivalDetector.ArrivalEvents events);   // 플레이어 도착 처리

    public void OnPlayerLeave(GameObject player);      // 플레이어 이탈 처리
}



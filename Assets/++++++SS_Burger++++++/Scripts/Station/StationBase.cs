using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StationBase : MonoBehaviour, IStation
{
    [Header("스테이션 공용")]
    [SerializeField] private string stationId = "Station";
    [SerializeField] private Transform approachPoint;
    [SerializeField] private bool startAvailable = false;

    protected bool available;

    protected virtual void Awake()      
    {
        available = startAvailable;
    }

   
    #region ("IStation 항목")
    public string StationID => stationId;
    public Transform Root => transform;     // 이거는 왜 transform 이라는 게 선언안되어있는데 이렇게 사용하는거지
    public Transform ApproachPoint => approachPoint;
    public virtual bool IsAvailable => available;

    public virtual bool CanPlayerUse(GameObject player)
    {
        return IsAvailable && player != null;
    }

    public abstract void OnPlayerArrive(GameObject player, ArrivalDetector.ArrivalEvents events);
    
    public virtual void OnPlayerLeave(GameObject player)
    {
        // 필요 시 정리 로직
    }

    #endregion
   
    // 외부에서 잠금/해제할 수 있도록 공개
    public void SetAvailable(bool value) => available = value; 

}

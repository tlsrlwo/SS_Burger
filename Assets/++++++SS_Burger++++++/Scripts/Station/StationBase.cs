using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StationBase : MonoBehaviour, IStation
{
    [Header("�����̼� ����")]
    [SerializeField] private string stationId = "Station";
    [SerializeField] private Transform approachPoint;
    [SerializeField] private bool startAvailable = false;

    protected bool available;

    protected virtual void Awake()      
    {
        available = startAvailable;
    }

   
    #region ("IStation �׸�")
    public string StationID => stationId;
    public Transform Root => transform;     // �̰Ŵ� �� transform �̶�� �� ����ȵǾ��ִµ� �̷��� ����ϴ°���
    public Transform ApproachPoint => approachPoint;
    public virtual bool IsAvailable => available;

    public virtual bool CanPlayerUse(GameObject player)
    {
        return IsAvailable && player != null;
    }

    public abstract void OnPlayerArrive(GameObject player, ArrivalDetector.ArrivalEvents events);
    
    public virtual void OnPlayerLeave(GameObject player)
    {
        // �ʿ� �� ���� ����
    }

    #endregion
   
    // �ܺο��� ���/������ �� �ֵ��� ����
    public void SetAvailable(bool value) => available = value; 

}

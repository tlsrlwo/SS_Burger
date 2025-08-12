using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ArrivalDetector : MonoBehaviour
{
    // 도착 이벤트 정보 구조체
    public readonly struct ArrivalEvents
    {
        public readonly object      Context;            // Arm() 호출 시 넘길 문맥 (스테이션이름, 씬이름)
        public readonly Transform   ApproachPoint;      // 접근 지점
        public readonly Vector3     Position;           // 도착 지점의 실제 위치
        public readonly float       Time;               // 도착 시간

        public ArrivalEvents(object context, Transform approachPoint, Vector3 position, float time)
        {
            Context = context;
            ApproachPoint = approachPoint;
            Position = position;
            Time = time;
        }
    }

    public event Action<ArrivalEvents> OnArrived;

    [Header("도착 정보")]
    [SerializeField] private float arriveTolerance = 0.15f;         // stoppingDistance 가 0일 때를 대비한 보정값
    [SerializeField] private bool useApproachPointCheck = false;    // ApproachPoint와의 거리까지 확인할지 여부
    [SerializeField] private float approachPointTolerance = 0.40f;  // ApproachPoint 와의 허용 거리

    private NavMeshAgent _agent;

    // Arm 시 세팅되는 상태
    private bool _armed;                                            // 도착 감지 활성화 여부
    private Transform _approachPoint;                               //
    private object _context;                                        //

    public bool IsArmed => _armed;
    public object Context => _context;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        if(_agent == null)
        {
            Debug.LogError("[ArrivalDetector] NavMeshAgent 가 필요합니다", this);     // Debug.LogError 는 디버그 오류 시 정지
        }
    }

    void Update()
    {
        if (!_armed || _agent == null) return;              // armed 가 아니거나 navMeshAgent 가 존재하지 않을시
        if (!_agent.isOnNavMesh) return;                    // navMeshSurface 에 있지 않을 시 (오류 방지)
        if (_agent.pathPending) return;                     // 플레이어가 길을 계산하고 있을 때 반환


        // _armed 해제를 위해 3번 확인
        // 거리 기준
        bool nearTarget = _agent.remainingDistance <= Mathf.Max(_agent.stoppingDistance, arriveTolerance); //StoppingDistance 보정
        if (!nearTarget) return;

        // 실제 정지 여부
        bool actuallyStopped = (!_agent.hasPath || _agent.velocity.sqrMagnitude <= 0.0001f);
        if(!actuallyStopped) return;

        // ApproachPointCheck 정밀확인
        // 이따 해보자고

        // 한 번만 실행되도록 이해제 후 이벤트 호출 ?
        _armed = false;

        var payload = new ArrivalEvents(_context, _approachPoint, transform.position, Time.time);

        OnArrived?.Invoke(payload);
    }

    // 도착 감지 활성화
    public void Arm(Transform approachPoint = null, object context = null, bool? overrideUseApproachCheck = null)
    {
        _approachPoint = approachPoint;
        _context = context;
        if(overrideUseApproachCheck.HasValue)
        {
            useApproachPointCheck = overrideUseApproachCheck.Value;
        }

        _armed = true;
    }

    // 도착 감지 비활성화
    public void Disarm()
    {
        _armed = false;
        _approachPoint = null;          // 다음 요청으로 문맥 누수 방지
        _context = null;
    }
}

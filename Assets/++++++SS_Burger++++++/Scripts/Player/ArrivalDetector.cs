using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ArrivalDetector : MonoBehaviour
{
    // ���� �̺�Ʈ ���� ����ü
    public readonly struct ArrivalEvents
    {
        public readonly object      Context;            // Arm() ȣ�� �� �ѱ� ���� (�����̼��̸�, ���̸�)
        public readonly Transform   ApproachPoint;      // ���� ����
        public readonly Vector3     Position;           // ���� ������ ���� ��ġ
        public readonly float       Time;               // ���� �ð�

        public ArrivalEvents(object context, Transform approachPoint, Vector3 position, float time)
        {
            Context = context;
            ApproachPoint = approachPoint;
            Position = position;
            Time = time;
        }
    }

    public event Action<ArrivalEvents> OnArrived;

    [Header("���� ����")]
    [SerializeField] private float arriveTolerance = 0.15f;         // stoppingDistance �� 0�� ���� ����� ������
    [SerializeField] private bool useApproachPointCheck = false;    // ApproachPoint���� �Ÿ����� Ȯ������ ����
    [SerializeField] private float approachPointTolerance = 0.40f;  // ApproachPoint ���� ��� �Ÿ�

    private NavMeshAgent _agent;

    // Arm �� ���õǴ� ����
    private bool _armed;                                            // ���� ���� Ȱ��ȭ ����
    private Transform _approachPoint;                               //
    private object _context;                                        //

    public bool IsArmed => _armed;
    public object Context => _context;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        if(_agent == null)
        {
            Debug.LogError("[ArrivalDetector] NavMeshAgent �� �ʿ��մϴ�", this);     // Debug.LogError �� ����� ���� �� ����
        }
    }

    void Update()
    {
        if (!_armed || _agent == null) return;              // armed �� �ƴϰų� navMeshAgent �� �������� ������
        if (!_agent.isOnNavMesh) return;                    // navMeshSurface �� ���� ���� �� (���� ����)
        if (_agent.pathPending) return;                     // �÷��̾ ���� ����ϰ� ���� �� ��ȯ


        // _armed ������ ���� 3�� Ȯ��
        // �Ÿ� ����
        bool nearTarget = _agent.remainingDistance <= Mathf.Max(_agent.stoppingDistance, arriveTolerance); //StoppingDistance ����
        if (!nearTarget) return;

        // ���� ���� ����
        bool actuallyStopped = (!_agent.hasPath || _agent.velocity.sqrMagnitude <= 0.0001f);
        if(!actuallyStopped) return;

        // ApproachPointCheck ����Ȯ��
        // �̵� �غ��ڰ�

        // �� ���� ����ǵ��� ������ �� �̺�Ʈ ȣ�� ?
        _armed = false;

        var payload = new ArrivalEvents(_context, _approachPoint, transform.position, Time.time);

        OnArrived?.Invoke(payload);
    }

    // ���� ���� Ȱ��ȭ
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

    // ���� ���� ��Ȱ��ȭ
    public void Disarm()
    {
        _armed = false;
        _approachPoint = null;          // ���� ��û���� ���� ���� ����
        _context = null;
    }
}

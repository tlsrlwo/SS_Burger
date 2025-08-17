using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ArrivalDetector))]
public class PlayerMove : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private float rayDistance = 1000f;
    [SerializeField] private LayerMask raycastMask = ~0;        // ��ü
    [SerializeField] private bool ignoreUi = true;

    [Header("NavMesh ����")]
    [SerializeField] private float sampleMaxDistance = 1.0f;    // Ap �� ���� ���� �� ���� �ݰ�
    [SerializeField] private int sampleAreaMask = NavMesh.AllAreas;

    [Header("���� �����̼�")]
    [SerializeField]private IStation currentStation;

    private NavMeshAgent agent;
    private Animator anim;
    private ArrivalDetector arrivalDetector;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");         // �޸� ��뷪�� �����ϱ� ���� SpeedHash ���� ����

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        TryGetComponent(out anim);                                                  // animator �� ��� �ϴ� ��Ȱ�� ����ǰԲ�
        arrivalDetector = GetComponent<ArrivalDetector>();

        if(arrivalDetector != null)
        {
            arrivalDetector.OnArrived += HandleArrived;
        }
    }

    private void OnDestroy()
    {
        if(arrivalDetector != null)
        {
            arrivalDetector.OnArrived -= HandleArrived;
        }
    }


    private void Update()
    {
        HandleClickMove();

        if(anim != null)
        {
            anim.SetFloat(SpeedHash, agent != null ? agent.velocity.magnitude : 0f);            // NavMeshAgent �� ������ agent �� �ӵ�, ������ 0
        }
    }

    // ���콺 Ŭ���� ���� �̵�
    private void HandleClickMove()
    {
        // ���� ����
        if (!Input.GetMouseButtonDown(1)) return;                                               // ���콺 Ŭ�� ������ ��ȯ
        if (ignoreUi && EventSystem.current.IsPointerOverGameObject()) return;                  // Ui�� ����Ǿ��ְų�, ���콺�� Ui ���� ������ ��ȯ
        if (Camera.main == null || agent == null) return;                                       // ī�޶� ����, navMesh �� ������ ��ȯ

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, raycastMask)) return;

        // �����̼� Ŭ�� ���� �ڽ� �޽õ� Ŀ�� : �θ𿡼� IStation Ž����)
        IStation station = null;

        if(hit.transform != null)
        {
            station = hit.transform.GetComponentInParent<IStation>();                           // IStation �� ��ӹ޴� ������Ʈ�� ��ȯ���༭ GetComponentInParent<IStation>
        }

        if(station != null)
        {
            MoveToStation(station);
            return;
        }


        // �Ϲ� ���� �̵�       
        ClearCurrentStation();                                              // ���� ��ȣ�ۿ� ����
        MoveToPoint(hit.point);
        arrivalDetector.Disarm();                                           // ���� üũ ���ʿ�
    }

    // ������ �����̼� �������� �̵�(ApproachPoint)
    private void MoveToStation(IStation station)
    {
        if (station == null || agent == null) return;

        // ��� ���� ���� Ȯ��
        if (!station.CanPlayerUse(gameObject))
        {
            // �ʿ� �� �ȳ� UI/���� ó�� ���-------------
            ClearCurrentStation();
            return;
        }

        currentStation = station;

        // ApproachPoint�� ������ approachPoint �� ����, ������ �����̼��� root.position ���� �̵�
        Vector3 target = station.ApproachPoint != null
            ? station.ApproachPoint.position : station.Root.position;           // position �̶� transform �� ����???

        // NavMesh ����, navMesh ���� �ƴ� ���� ���� �� ��ó navMeshArea �� ��ȯ
        if( NavMesh.SamplePosition(target, out NavMeshHit navHit, sampleMaxDistance, sampleAreaMask))
        {
            target = navHit.position;
        }

        MoveToPoint(target);

        // ���� ���� Arm (Context�� station ��ü�� ����)
        arrivalDetector.Arm(station.ApproachPoint, station, true);                  // ApproachPoint ???? Arm �Լ� �� ������
    }

    // �����̼��� �ƴ� �������� �̵�
    private void MoveToPoint(Vector3 worldPoint)
    {
        if (agent == null) return;
        agent.isStopped = false;
        agent.SetDestination(worldPoint);
    }

    // �����̼ǿ� ����
    private void HandleArrived(ArrivalDetector.ArrivalEvents e)
    {
        // Context �� �Ǿ�� �����̼�
        if (e.Context is IStation station && currentStation == station)
        {
            station.OnPlayerArrive(gameObject, e);

            // ��ȣ�ۿ� ���� ���߰� �ʹٸ� Ȱ��ȭ
            // agent.isStopped = true;
        }
    }

    // ���� �����̼ǰ��� ��ũ ���� + ��Ż �˸�
    private void ClearCurrentStation()
    {
        if (currentStation != null)
        {
            currentStation.OnPlayerLeave(gameObject);
            currentStation = null;
        }
    }

}

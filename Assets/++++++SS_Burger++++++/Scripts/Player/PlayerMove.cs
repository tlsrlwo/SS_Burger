using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("�÷��̾� ȸ�� ����")]
    [SerializeField] private bool rotateOnArrive = true;        
    [SerializeField] private bool smoothRotate = true;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float angleSnap = 0.1f;
    //[SerializeField] private bool lockFacingDuringInteraction = true;

    private Coroutine rotateCoroutine;
    
    private IStation currentStation;

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
        // ���콺 Ŭ�� ������ ��ȯ
        if (!Input.GetMouseButtonDown(1)) return; 

        // Ui�� ����Ǿ��ְų�, ���콺�� Ui ���� ������ ��ȯ
        if (ignoreUi && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        // ī�޶� ����, navMesh �� ������ ��ȯ
        if (Camera.main == null || agent == null) return;


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, raycastMask)) return;

        // �����̼� Ŭ�� ����, �ڽ� �޽õ� Ŀ�� : �θ𿡼� IStation Ž����)
        IStation clickedStation = null;

        if(hit.transform != null)
        {
            clickedStation = hit.transform.GetComponentInParent<IStation>();                           // IStation �� ��ӹ޴� ������Ʈ�� ��ȯ���༭ GetComponentInParent<IStation>
        }

        if(clickedStation != null)
        {
            // ���� ���õ� �����̼��� �ְ�, ���� ���� �����̼��� �ƴ� �� (���� �����̼� �ִ� ���¿��� �ٸ� �����̼����� �̵�(�����̼ǿ��� �����̼����� �̵� �� OnPlayerLeave() �� �ȶߴ� ���� ����))
            if (currentStation != null && currentStation != clickedStation)          
            {
                // ���� �����̼ǿ����� ��Ż ����
                currentStation.OnPlayerLeave(gameObject);

                // ���� ���� ���� ����
                arrivalDetector.Disarm();
                currentStation = null;
            }

            MoveToStation(clickedStation);
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

        // �÷��̾��� �����̼� ��� ���� ���� Ȯ��
        if (!station.CanPlayerUse(gameObject))
        {
            // ���� �÷��̾ �����̼��� ����� �� ���ٴ� Ui ǥ��
            Debug.Log("���� �ش� �����̼��� �̿� �� �� �����ϴ�");

            // ClearCurrentStation ���ٴ� ���� �÷��̾� ���� ����/�������� �� �ڿ�������
            // ClearCurrentStation();
            return;
        }
        
        // ���� �����̼��� �ٽ� Ŭ���ص� ������ ���� (�ٽ� Arm() ��)
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
        if (arrivalDetector != null)
        {
            arrivalDetector.Arm(station.ApproachPoint, station, true);
        }
    }

    // �����̼��� �ƴ� �������� �̵�
    private void MoveToPoint(Vector3 worldPoint)
    {
        if (agent == null) return;

        // �����̼ǿ� �����ؼ� ȸ���ϰ� ������ ����
        StopRotationCoroutine();
        EnableAutoRotation();

        agent.isStopped = false;
        agent.SetDestination(worldPoint);
    }

    // �����̼ǿ� ����
    private void HandleArrived(ArrivalDetector.ArrivalEvents events)
    {
        // Context �� �Ǿ�� �����̼�
        if (events.Context is IStation station && currentStation == station)
        {
            // �����̼ǿ� �÷��̾ �������� �˸�
            station.OnPlayerArrive(gameObject, events);

            // �÷��̾� �� ���� ����
            AlignPlayerToStation(station, events);

            // ��ȣ�ۿ� ���� ���߰� �ʹٸ� Ȱ��ȭ
            // agent.isStopped = true;
        }
    }

    #region �÷��̾� �����̼� ���� �� ȸ������
    private void AlignPlayerToStation(IStation station, ArrivalDetector.ArrivalEvents events)
    {
        // ���� ����
        if (!rotateOnArrive || station == null) return;

        DisableAutoRotation();
        Quaternion targetRot;
        
        // ������ ApproachPoint �� ����
        if(station.ApproachPoint != null)
        {
            targetRot = station.ApproachPoint.rotation;
        }
        // ������ ApproachPoint �� ����
        else
        {
            Vector3 dir = station.Root.position - transform.position;
            dir.y = 0;
            if (dir.sqrMagnitude < 0.0001) return;
            targetRot = Quaternion.LookRotation(dir.normalized);            // ���Ⱚ�� 1�� ���ܵ� ���Ⱚ�� �������� ȸ���� ����

        }

        StopRotationCoroutine();
        // �ε巴�� ���� �ƴϸ� �׳� �ٷ� ������ �ٲ���
        if (smoothRotate)
        {            
            rotateCoroutine = StartCoroutine(CoSmoothRotate(targetRot));
        }
        else
        {
            transform.rotation = targetRot;
        }
    }

    // �ε巴�� �÷��̾ �����̼��� �ٶ󺸴� ������ ������ �κ�
    private IEnumerator CoSmoothRotate(Quaternion targetRot)
    {
        // ��ǥ�� ���� ȸ���� ���̰� 0.1�� ���ϰ� �� ������ �ݺ�
        while (Quaternion.Angle(transform.rotation, targetRot) > angleSnap)
        {
            // ���� ȸ�� �� ��ǥ ȸ�� ���̸� �ε巴�� ����
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * rotateSpeed   // rotateSpeed�� ���� ����, ���� Ű��� ������ ȸ��
            );
            yield return null; // �� ������ �� �� �ٽ� �ݺ�
        }

        // �������� �� ���� ����
        transform.rotation = targetRot;
        rotateCoroutine = null;
    }
      

    private void EnableAutoRotation()
    {
        if (agent != null) agent.updateRotation = true;
    }
    private void DisableAutoRotation()
    {
        if (agent != null) agent.updateRotation = false;
    }

    private void StopRotationCoroutine()
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
            rotateCoroutine = null;
        }
    }
    #endregion

    // ���� �����̼ǰ��� ��ũ ���� + ��Ż �˸�
    private void ClearCurrentStation()
    {
        if (currentStation != null)
        {
            currentStation.OnPlayerLeave(gameObject);
            currentStation = null;
        }

        // ��ȣ�ۿ� ���� -> ȸ�� ���� ����
        StopRotationCoroutine();
        EnableAutoRotation();
    }

}

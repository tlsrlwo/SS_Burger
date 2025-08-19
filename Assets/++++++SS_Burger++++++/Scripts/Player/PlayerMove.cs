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
    [SerializeField] private LayerMask raycastMask = ~0;        // 전체
    [SerializeField] private bool ignoreUi = true;

    [Header("NavMesh 보정")]
    [SerializeField] private float sampleMaxDistance = 1.0f;    // Ap 가 내비 밖일 때 스냅 반경
    [SerializeField] private int sampleAreaMask = NavMesh.AllAreas;

    [Header("현재 스테이션")]
    private IStation currentStation;

    private NavMeshAgent agent;
    private Animator anim;
    private ArrivalDetector arrivalDetector;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");         // 메모리 사용랴을 절약하기 위해 SpeedHash 변수 생성

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        TryGetComponent(out anim);                                                  // animator 가 없어도 일단 원활히 실행되게끔
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
            anim.SetFloat(SpeedHash, agent != null ? agent.velocity.magnitude : 0f);            // NavMeshAgent 가 있으면 agent 의 속도, 없으면 0
        }
    }

    // 마우스 클릭에 따른 이동
    private void HandleClickMove()
    {
        // 오류 방지        
        // 마우스 클릭 없으면 반환
        if (!Input.GetMouseButtonDown(1)) return; 

        // Ui가 실행되어있거나, 마우스가 Ui 위에 있으면 반환
        if (ignoreUi && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        // 카메라가 없고, navMesh 가 없으면 반환
        if (Camera.main == null || agent == null) return;


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, raycastMask)) return;

        // 스테이션 클릭 여부, 자식 메시도 커버 : 부모에서 IStation 탐색함)
        IStation clickedStation = null;

        if(hit.transform != null)
        {
            clickedStation = hit.transform.GetComponentInParent<IStation>();                           // IStation 을 상속받는 컴포넌트를 반환해줘서 GetComponentInParent<IStation>
        }

        if(clickedStation != null)
        {
            // 현재 선택된 스테이션이 있고, 지금 누른 스테이션이 아닐 시 (기존 스테이션 있는 상태에서 다른 스테이션으로 이동(스테이션에서 스테이션으로 이동 시 OnPlayerLeave() 가 안뜨는 현상 수정))
            if (currentStation != null && currentStation != clickedStation)          
            {
                // 이전 스테이션에서의 이탈 선언
                currentStation.OnPlayerLeave(gameObject);

                // 이전 도착 감지 해제
                arrivalDetector.Disarm();
                currentStation = null;
            }

            MoveToStation(clickedStation);
            return;
        }

        // 일반 지점 이동       
        ClearCurrentStation();                                              // 기존 상호작용 해제
        MoveToPoint(hit.point);
        arrivalDetector.Disarm();                                           // 도착 체크 불필요
    }

    // 지정된 스테이션 목적지로 이동(ApproachPoint)
    private void MoveToStation(IStation station)
    {
        if (station == null || agent == null) return;

        // 플레이어의 스테이션 사용 가능 여부 확인
        if (!station.CanPlayerUse(gameObject))
        {
            // 현재 플레이어가 스테이션을 사용할 수 없다는 Ui 표시
            Debug.Log("현재 해당 스테이션은 이용 할 수 없습니다");

            // ClearCurrentStation 보다는 현재 플레이어 상태 유지/무동작이 더 자연스러움
            // ClearCurrentStation();
            return;
        }
        
        // 같은 스테이션을 다시 클릭해도 목적지 갱신 (다시 Arm() 됨)
        currentStation = station;

        // ApproachPoint가 있으면 approachPoint 로 가고, 없으면 스테이션의 root.position 으로 이동
        Vector3 target = station.ApproachPoint != null
            ? station.ApproachPoint.position : station.Root.position;           // position 이랑 transform 의 차이???

        // NavMesh 보정, navMesh 위가 아닌 점을 선택 시 근처 navMeshArea 로 반환
        if( NavMesh.SamplePosition(target, out NavMeshHit navHit, sampleMaxDistance, sampleAreaMask))
        {
            target = navHit.position;
        }

        MoveToPoint(target);

        // 도착 감지 Arm (Context에 station 자체를 넣음)
        if (arrivalDetector != null)
        {
            arrivalDetector.Arm(station.ApproachPoint, station, true);
        }
    }

    // 스테이션이 아닌 곳으로의 이동
    private void MoveToPoint(Vector3 worldPoint)
    {
        if (agent == null) return;
        agent.isStopped = false;
        agent.SetDestination(worldPoint);
    }

    // 스테이션에 도착
    private void HandleArrived(ArrivalDetector.ArrivalEvents e)
    {
        // Context 로 실어둔 스테이션
        if (e.Context is IStation station && currentStation == station)
        {
            station.OnPlayerArrive(gameObject, e);

            // 상호작용 동안 멈추고 싶다면 활성화
            // agent.isStopped = true;
        }
    }

    // 현재 스테이션과의 링크 해제 + 이탈 알림
    private void ClearCurrentStation()
    {
        if (currentStation != null)
        {
            currentStation.OnPlayerLeave(gameObject);
            currentStation = null;
        }
    }

}

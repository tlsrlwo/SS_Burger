using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class QuarterViewCamera : MonoBehaviour
{
    // 카메라 이동, 회전 변수
    public float cameraRotationSpeed;
    public float cameraMoveSpeed;

    [SerializeField]private float limitRange = 3f;
    public float minX, maxX, minZ, maxZ;   
    
    // 컴포넌트
    public Camera mainCamera;
    public Transform cameraPivot;
    public Cinemachine.CinemachineVirtualCamera quarterViewCamera;

    private void Start()
    {
        // 카메라 이동범위 제한
        Vector3 cameraStartPos = cameraPivot.position;              // 이미 transform 으로 선언되어서 .position 만 적어도 됨

        minX = cameraStartPos.x - limitRange;
        maxX = cameraStartPos.x + limitRange;
        minZ = cameraStartPos.z - limitRange;
        maxZ = cameraStartPos.z + limitRange;
    }

    private void Update()
    {
        CameraRotation();
        CameraMove();       
    }

    private void CameraRotation()
    {
        float rotateDirection = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotateDirection = 1f;
            Vector3 originalRot = quarterViewCamera.transform.localRotation.eulerAngles;

            quarterViewCamera.transform.localRotation = Quaternion.Euler(originalRot + (Vector3.up * rotateDirection * cameraRotationSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateDirection = -1f;
            Vector3 originalRot = quarterViewCamera.transform.localRotation.eulerAngles;

            quarterViewCamera.transform.localRotation = Quaternion.Euler(originalRot + (Vector3.up * rotateDirection * cameraRotationSpeed * Time.deltaTime));
        }
    }

    private void CameraMove()
    {
        // 카메라 앞뒤좌우 이동
        Vector3 cameraForward = mainCamera.transform.forward;       // cameraPivot 이 mainCamera의 앞뒤좌우 를 기준으로 움직이도록 변수 생성
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0;                                        // 카메라가 아래를 바라보고 있어도 이동 방향이 “수평(지면)만” 따르도록 만들기 위해서
        cameraRight.y = 0;

        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += cameraForward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= cameraForward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += cameraRight;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= cameraRight;
        }

        if (moveDirection == Vector3.zero) return;

        // 카메라 이동
        Vector3 nextPosition = cameraPivot.position + moveDirection.normalized * cameraMoveSpeed * Time.deltaTime;

        if(nextPosition.x <= maxX && nextPosition.x >= minX && nextPosition.z <= maxZ && nextPosition.z >= minZ)
        {
            cameraPivot.position = nextPosition;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // 사각형 범위 시각화 (파란 선)
        Gizmos.color = Color.cyan;

        Vector3 p1 = new Vector3(minX, cameraPivot.position.y, minZ);
        Vector3 p2 = new Vector3(maxX, cameraPivot.position.y, minZ);
        Vector3 p3 = new Vector3(maxX, cameraPivot.position.y, maxZ);
        Vector3 p4 = new Vector3(minX, cameraPivot.position.y, maxZ);

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class QuarterViewCamera : MonoBehaviour
{
    // ī�޶� �̵�, ȸ�� ����
    public float cameraRotationSpeed;
    public float cameraMoveSpeed;

    [SerializeField]private float limitRange = 3f;
    public float minX, maxX, minZ, maxZ;   
    
    // ������Ʈ
    public Camera mainCamera;
    public Transform cameraPivot;
    public Cinemachine.CinemachineVirtualCamera quarterViewCamera;

    private void Start()
    {
        // ī�޶� �̵����� ����
        Vector3 cameraStartPos = cameraPivot.position;              // �̹� transform ���� ����Ǿ .position �� ��� ��

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
        // ī�޶� �յ��¿� �̵�
        Vector3 cameraForward = mainCamera.transform.forward;       // cameraPivot �� mainCamera�� �յ��¿� �� �������� �����̵��� ���� ����
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0;                                        // ī�޶� �Ʒ��� �ٶ󺸰� �־ �̵� ������ ������(����)���� �������� ����� ���ؼ�
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

        // ī�޶� �̵�
        Vector3 nextPosition = cameraPivot.position + moveDirection.normalized * cameraMoveSpeed * Time.deltaTime;

        if(nextPosition.x <= maxX && nextPosition.x >= minX && nextPosition.z <= maxZ && nextPosition.z >= minZ)
        {
            cameraPivot.position = nextPosition;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // �簢�� ���� �ð�ȭ (�Ķ� ��)
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

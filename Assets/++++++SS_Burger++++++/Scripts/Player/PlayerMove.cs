using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMove : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    private Animator anim;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();    
    }

    private void Update()
    {
        MoveByMousePos();
        // 애니메이션
        float speed = navMeshAgent.velocity.magnitude;
        anim.SetFloat("Speed", speed);
    }

    private void MoveByMousePos()
    {
        // 마우스 클릭 한 부분으로 이동        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                navMeshAgent.SetDestination(hit.point);
            }
        }        
    } 
}

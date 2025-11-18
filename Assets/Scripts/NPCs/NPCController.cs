using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCController : MonoBehaviour
{
    public enum NPCState
    {
        Idle,
        Wander,
        Talk
    }

    [Header("NPC Settings")]
    [SerializeField] private string npcId;
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderInterval = 5f;
    [SerializeField] private float talkDuration = 3f;

    [Header("Components")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;

    private NPCState currentState = NPCState.Idle;
    private Vector3 startPosition;
    private bool isTalking = false;

    private void Start()
    {
        startPosition = transform.position;

        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        StartCoroutine(WanderRoutine());
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (animator != null && navMeshAgent != null)
        {
            float speed = navMeshAgent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (currentState == NPCState.Idle && !isTalking)
            {
                SetState(NPCState.Wander);
                Vector3 randomDestination = GetRandomWanderPoint();
                navMeshAgent.SetDestination(randomDestination);
            }

            yield return new WaitForSeconds(wanderInterval);
        }
    }

    private Vector3 GetRandomWanderPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += startPosition;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return startPosition;
    }

    public void StartTalking()
    {
        if (isTalking) return;

        isTalking = true;
        SetState(NPCState.Talk);

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
        }

        StartCoroutine(EndTalkingAfterDelay());
    }

    private IEnumerator EndTalkingAfterDelay()
    {
        yield return new WaitForSeconds(talkDuration);

        isTalking = false;
        SetState(NPCState.Idle);

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false;
        }
    }

    private void SetState(NPCState newState)
    {
        currentState = newState;

        if (animator != null)
        {
            animator.SetBool("IsTalking", currentState == NPCState.Talk);
        }
    }

    public string GetNpcId()
    {
        return npcId;
    }

    public bool IsTalking()
    {
        return isTalking;
    }
}

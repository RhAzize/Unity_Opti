using UnityEngine;
using UnityEngine.AI;

public class MoveToClickPoint : MonoBehaviour
{

    private NavMeshAgent _navAgent;
    private Animator _animator;
    private RaycastHit _hit;
    private Vector3 _destination;
    void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        if (_navAgent == null || _animator == null)
        {
            Debug.LogError("composant non trouvé!");
        }
    }
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit, 100))
            {
                _destination = _hit.point;
                _navAgent.destination = _destination;
                _navAgent.isStopped = false;
            }
        }

        //on vérifie si le player est arrivé à destination
        if ((transform.position - _navAgent.destination).sqrMagnitude < 0.01f)
        {
            _navAgent.isStopped = true;
        }

        //on met à jour l'animation en fonction de la vitesse de l'agent
        //après avoir vérifier que le component est bien là pour éviter la nullreference
        bool isMoving = _navAgent.velocity.sqrMagnitude > 0.01f;
        _animator.SetBool("running", isMoving);

    }
}
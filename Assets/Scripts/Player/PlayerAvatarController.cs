using System;
using CameraLogic;
using GardenLogic;
using UnityEngine;
using UnityEngine.AI;

namespace Player
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerAvatarController : MonoBehaviour, IAnimationStateReader
    {
        private static readonly int GatherHash = Animator.StringToHash("Gather");
        private static readonly int MoveHash = Animator.StringToHash("MoveVelocity");

        private readonly int _idleStateHash = Animator.StringToHash("Idle");
        private readonly int _gatheringStateHash = Animator.StringToHash("Gathering");
        private readonly int _runningStateHash = Animator.StringToHash("Running");

        [SerializeField] private Animator _avatarAnimator;
        [SerializeField] private NavMeshAgent _navMeshAgent;

        public AnimatorState State { get;  private set; }

        private Action _callBack;
        private PlantSpot _target;
        private Transform _defaultPosTransform;
        private CameraController _cameraController;
        private bool IsAbleToMove = true;

        private void Start()
        {
            var startPoint = GameObject.FindGameObjectWithTag("Player");
            if (startPoint != null)
            {
                transform.SetParent(startPoint.transform);
                transform.localPosition = Vector3.zero;
            }

            State = AnimatorState.Idle;
            _defaultPosTransform = transform.parent;
            GetComponent<CapsuleCollider>().isTrigger = true;
        }
        public void AttachCameraController(CameraController cameraController) =>
            _cameraController = cameraController;
        
        public void MoveTo(PlantSpot target, Action callBack = null)
        {
            if (!IsAbleToMove)
                return;

            _target = target;
            _callBack = callBack;

            _navMeshAgent.SetDestination(_target.transform.position);
            _cameraController.MoveCamera(_target.transform, MathF.Floor(Vector3.Distance(transform.position, target.transform.position) / 2));
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out PlantSpot plantSpot) && IsAbleToMove)
            {
                if (_target != null && plantSpot.gameObject.GetInstanceID() == _target.gameObject.GetInstanceID())
                {
                    IsAbleToMove = false;
                    RotateAvatar(_target.transform);
                    _avatarAnimator.SetTrigger(GatherHash);
                    _navMeshAgent.isStopped = true;
                }
            }
        }

        private void RotateAvatar(Transform currentTarget = null)
        {
            if (currentTarget == null) // if target not setted, then rotate to default idle rotation -90
            {
                var defaultRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
                transform.rotation = defaultRotation;
            }
            else
            {
                Vector3 direction = currentTarget.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
        }
        private void Update()
        {
            _avatarAnimator.SetFloat(MoveHash, _navMeshAgent.velocity.magnitude, 0.1f, Time.deltaTime);
            if (_navMeshAgent.velocity.magnitude < 0.2f)
                RotateAvatar();
        }

        public void EnteredState(int stateHash)
        {
            State = StateFor(stateHash);

            if (State == AnimatorState.Interact)
                IsAbleToMove = false;
        }

        public void ExitedState(int stateHash)
        {
            if (StateFor(stateHash) == AnimatorState.Interact)
            {
                _callBack?.Invoke();
                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(_defaultPosTransform.position);
                _cameraController.MoveCamera();
                _target = null;
                IsAbleToMove = true;

            }
        }
        private AnimatorState StateFor(int stateHash)
        {
            AnimatorState state;
            if (stateHash == _idleStateHash)
            {
                state = AnimatorState.Idle;
            }
            else if (stateHash == _gatheringStateHash)
            {
                state = AnimatorState.Interact;
            }
            else if (stateHash == _runningStateHash)
            {
                state = AnimatorState.Walking;
            }
            else
            {
                state = AnimatorState.Unknown;
            }

            return state;
        }
    }
}
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace Assets.Gamelogic.Player
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class ThirdPersonPlayerControls : MonoBehaviour
    {
        [Require] private Position.Writer PositionWriter;
        [Require] private PlayerMovement.Writer PlayerMovementWriter;
        [Require] private PlayerRotation.Writer PlayerRotationWriter;

        public Vector3 CurrentCanonicalPosition { get { return new Vector3((float)PositionWriter.Data.coords.x, 0, (float)PositionWriter.Data.coords.z); } }
        private Transform camera;
        private float cameraDistance;
        private float cameraYaw;
        private float cameraPitch;
        private Vector3 targetVelocity;
        private bool playerIsGrounded;

        [SerializeField] private Collider playerCollider;
        [SerializeField] private Rigidbody playerRigidbody;
        [SerializeField] private Animator playerAnimator;

        private void Awake()
        {
            cameraDistance = SimulationSettings.ThirdPersonCameraDefaultDistance;
            cameraPitch = SimulationSettings.ThirdPersonCameraDefaultPitch;
        }

        private void OnEnable()
        {
            camera = Camera.main.transform;
            camera.parent = transform;

            playerRigidbody.MovePosition(PositionWriter.Data.coords.ToUnityVector());
            playerRigidbody.MoveRotation(Quaternion.Euler(0f, PlayerRotationWriter.Data.yaw, 0f));
        }

        private void Update()
        {
            UpdateDesiredMovementDirection();
            UpdatePlayerToCameraRotation();
        }

        private void FixedUpdate()
        {
            UpdatePlayerControls();
            MovePlayer();
            UpdateAnimation();
        }

        private void LateUpdate()
        {
            MoveCamera();
            UpdateCameraRotation();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void UpdateAnimation()
        {
            var playerMovement = transform.InverseTransformDirection(Vector3.ClampMagnitude(playerRigidbody.velocity, 1));
            var playerTurn = Mathf.Atan2(playerMovement.x, playerMovement.z);
            playerTurn = Input.GetAxis("Horizontal").Equals(0) ? 0 : playerTurn;
            var playerMotion = playerMovement.magnitude;
            playerAnimator.SetFloat("Forward", playerMotion);
            playerAnimator.SetFloat("Turn", Mathf.Clamp(playerTurn, -0.8f, 0.8f));
        }

        private void UpdateDesiredMovementDirection()
        {
            Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 movementDirection = (camera.transform.rotation * inputDirection).FlattenVector().normalized;
            targetVelocity = movementDirection * SimulationSettings.PlayerMovementSpeed;
        }

        private void UpdatePlayerToCameraRotation()
        {
            var distanceChange = cameraDistance - Input.GetAxis("Mouse ScrollWheel") * SimulationSettings.ThirdPersonCameraDistanceSensitivity;
            cameraDistance = Mathf.Clamp(distanceChange, SimulationSettings.ThirdPersonCameraMinDistance, SimulationSettings.ThirdPersonCameraMaxDistance);
            if (Input.GetMouseButton(SimulationSettings.ThirdPersonRotateCameraMouseButton))
            {
                cameraYaw = (cameraYaw + Input.GetAxis("Mouse X") * SimulationSettings.ThirdPersonCameraSensitivity) % 360f;
                cameraPitch = Mathf.Clamp(cameraPitch - Input.GetAxis("Mouse Y") * SimulationSettings.ThirdPersonCameraSensitivity, SimulationSettings.ThirdPersonCameraMinPitch, SimulationSettings.ThirdPersonCameraMaxPitch);
            }
        }

        private void MoveCamera()
        {
            camera.position = transform.position + Quaternion.Euler(new Vector3(cameraPitch, cameraYaw, 0)) * Vector3.back * cameraDistance;
        }

        private void UpdateCameraRotation()
        {
            camera.LookAt(transform.position);
        }

        private void UpdatePlayerControls()
        {
            var newTargetPosition = playerRigidbody.position;
            if (ShouldUpdatePlayerTargetPosition(newTargetPosition))
            {
                PositionWriter.Send(new Position.Update().SetCoords(new Coordinates(newTargetPosition.x, 0, newTargetPosition.z)));
                PlayerMovementWriter.Send(new PlayerMovement.Update().AddMovementUpdate(new MovementUpdate(newTargetPosition.ToSpatialVector3d(), Time.time)));
                PlayerRotationWriter.Send(new PlayerRotation.Update().SetYaw(transform.eulerAngles.y).AddRotationUpdate(new RotationUpdate(transform.eulerAngles.y, Time.time)));
            }
        }

        private bool ShouldUpdatePlayerTargetPosition(Vector3 newTargetPosition)
        {
            return !MathUtils.CompareEqualityEpsilon(newTargetPosition, CurrentCanonicalPosition);
        }

        private void MovePlayer()
        {
            var currentVelocity = playerRigidbody.velocity;
            var velocityChange = targetVelocity - currentVelocity;
            if (ShouldMoveLocalPlayer(velocityChange))
            {
                transform.LookAt(playerRigidbody.position + targetVelocity);
                playerRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            }
        }

        private bool ShouldMoveLocalPlayer(Vector3 velocityChange)
        {
            return velocityChange.sqrMagnitude > Mathf.Epsilon;
        }
    }
}
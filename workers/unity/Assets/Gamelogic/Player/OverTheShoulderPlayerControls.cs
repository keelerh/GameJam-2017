using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Player;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace Assets.Gamelogic.Player
{
    public class OverTheShoulderPlayerControls : MonoBehaviour
    {
        [Require] private Position.Writer PositionWriter;
        [Require] private PlayerMovement.Writer PlayerMovementWriter;
        [Require] private PlayerRotation.Writer PlayerRotationWriter;

        public Vector3 CurrentCanonicalPosition { get { return new Vector3((float)PositionWriter.Data.coords.x, 0, (float)PositionWriter.Data.coords.z); } }
        private Transform camera;
        private GameObject cameraRoot;
        private float yaw;
        private float pitch;
        private Vector3 targetVelocity;
        private bool playerIsGrounded;

        [SerializeField] private Collider playerCollider;
        [SerializeField] private Rigidbody playerRigidbody;
        [SerializeField] private GameObject playerModel;
        [SerializeField] private Animator playerAnimator;

        private void OnEnable()
        {
            camera = Camera.main.transform;
            
            cameraRoot = new GameObject("CameraRoot");
            cameraRoot.transform.parent = transform;
            cameraRoot.transform.localPosition = SimulationSettings.ThirdPersonCameraRootOffset;
            camera.parent = cameraRoot.transform;
            camera.localPosition = SimulationSettings.ThirdPersonCameraOffset;

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            UpdateDesiredMovementDirection();
            SetCameraTransform();
        }

        private void FixedUpdate()
        {
            MovePlayer();
            UpdateAnimation();
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

        private void LateUpdate()
        {
            UpdatePlayerControls();
        }

        private void UpdateDesiredMovementDirection()
        {
            Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Mathf.Clamp01(Input.GetAxis("Vertical")));
            Vector3 movementDirection = (playerRigidbody.rotation * inputDirection).FlattenVector().normalized;
            targetVelocity = movementDirection * SimulationSettings.PlayerMovementSpeed;
        }

        private void UpdatePlayerControls()
        {
            var newTargetPosition = playerRigidbody.position;
            if (ShouldUpdatePlayerTargetPosition(newTargetPosition))
            {
                PositionWriter.Send(new Position.Update().SetCoords(new Coordinates(newTargetPosition.x, 0, newTargetPosition.z)));
                PlayerMovementWriter.Send(new PlayerMovement.Update().AddMovementUpdate(new MovementUpdate(newTargetPosition.ToSpatialVector3d(), Time.time)));
            }
        }

        private bool ShouldUpdatePlayerTargetPosition(Vector3 newTargetPosition)
        {
            return !MathUtils.CompareEqualityEpsilon(newTargetPosition, CurrentCanonicalPosition, 0.1f);
        }

        private void MovePlayer()
        {
            var currentVelocity = playerRigidbody.velocity.FlattenVector();
            var velocityChange = targetVelocity - currentVelocity;
            if (ShouldMoveLocalPlayer(velocityChange))
            {
                playerRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            }
        }

        private bool ShouldMoveLocalPlayer(Vector3 velocityChange)
        {
            return velocityChange.sqrMagnitude > Mathf.Epsilon;
        }

        private void SetCameraTransform()
        {
            yaw = (yaw + Input.GetAxis("Mouse X") * SimulationSettings.FirstPersonCameraSensitivity) % 360f;
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * SimulationSettings.FirstPersonCameraSensitivity,
                -SimulationSettings.FirstPersonCameraMaxPitch, -SimulationSettings.FirstPersonCameraMinPitch);
            cameraRoot.transform.localRotation = Quaternion.Euler(new Vector3(pitch, 0, 0));
            transform.rotation = (Quaternion.Euler(new Vector3(0, yaw, 0)));
            PlayerRotationWriter.Send(new PlayerRotation.Update().SetYaw(yaw).AddRotationUpdate(new RotationUpdate(yaw, Time.time)));
        }
    }
}

using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Player;
using Improbable.Unity.Visualizer;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using System;

namespace Assets.Gamelogic.Player
{
    public class OtherPlayerMovement : MonoBehaviour
    {
        [Require] private Position.Reader PositionReader;
        [Require] private PlayerMovement.Reader PlayerMovementReader;
        [Require] private PlayerRotation.Reader PlayerRotationReader;

        private InterpolationData<Vector3> positionInterpolationRoot;
        private Queue<InterpolationData<Vector3>> positionUpdates = new Queue<InterpolationData<Vector3>>();
        private float timeDifferentialToPositionSender;

        private InterpolationData<float> rotationInterpolationRoot;
        private Queue<InterpolationData<float>> rotationUpdates = new Queue<InterpolationData<float>>();
        private float timeDifferentialToRotationSender;

        [SerializeField] private Rigidbody playerRigidbody;
        [SerializeField] private Collider playerCollider;
        [SerializeField] private Animator playerAnimator;

        private void OnEnable()
        {
            transform.position = PositionReader.Data.coords.ToUnityVector();
            transform.rotation = Quaternion.Euler(new Vector3(0, PlayerRotationReader.Data.yaw, 0));

            PlayerMovementReader.MovementUpdateTriggered.Add(SaveMovementUpdate);
            PlayerRotationReader.RotationUpdateTriggered.Add(SaveRotationUpdate);
            DisableAuthoritativeRigidbodyBehaviour();
        }

        private void OnDisable()
        {
            PlayerMovementReader.MovementUpdateTriggered.Remove(SaveMovementUpdate);
            PlayerRotationReader.RotationUpdateTriggered.Remove(SaveRotationUpdate);
            EnableAuthoritativeRigidbodyBehaviour();
        }

        private void SaveMovementUpdate(MovementUpdate movementUpdate)
        {
            SaveInterpolationUpdate(ref positionInterpolationRoot, ref positionUpdates, ref timeDifferentialToPositionSender, new InterpolationData<Vector3>(movementUpdate.position.ToUnityVector(), movementUpdate.timestamp));
        }

        private void SaveRotationUpdate(RotationUpdate rotationUpdate)
        {
            SaveInterpolationUpdate(ref rotationInterpolationRoot, ref rotationUpdates, ref timeDifferentialToRotationSender, new InterpolationData<float>(rotationUpdate.yaw, rotationUpdate.timestamp));
        }

        private void SaveInterpolationUpdate<T>(ref InterpolationData<T> interpolationRoot, ref Queue<InterpolationData<T>> updateQueue, ref float timeDifferentialToSender, InterpolationData<T> newInterpolationData)
        {
            timeDifferentialToSender = Time.time - newInterpolationData.timestamp;
            if (interpolationRoot == null)
            {
                interpolationRoot = newInterpolationData;
                return;
            }
            updateQueue.Enqueue(newInterpolationData);
        }

        private void FixedUpdate()
        {
            DisableIfAuthoritative();

            var currentTime = Time.time;
            InterpolatePositionData(currentTime);
            InterpolateRotationData(currentTime);
            UpdateAnimation();
        }

        private void DisableIfAuthoritative()
        {
            if (PositionReader.HasAuthority)
            {
                enabled = false;
            }
        }

        private void InterpolatePositionData(float currentTime)
        {
            var currentInterpolationTime = currentTime - timeDifferentialToPositionSender - SimulationSettings.OtherPlayerUpdateDelay;
            if (ReadyToInterpolate(ref positionInterpolationRoot, ref positionUpdates, currentInterpolationTime))
            {
                var interpolationTarget = positionUpdates.Peek();
                var lerpRate = (currentInterpolationTime - positionInterpolationRoot.timestamp) / (interpolationTarget.timestamp - positionInterpolationRoot.timestamp);
                playerRigidbody.MovePosition(Vector3.Lerp(positionInterpolationRoot.data, interpolationTarget.data, lerpRate));
            }
        }
        private void InterpolateRotationData(float currentTime)
        {
            var currentInterpolationTime = currentTime - timeDifferentialToRotationSender - SimulationSettings.OtherPlayerUpdateDelay;
            if (ReadyToInterpolate(ref rotationInterpolationRoot, ref rotationUpdates, currentInterpolationTime))
            {
                var interpolationTarget = rotationUpdates.Peek();
                var lerpRate = (currentInterpolationTime - rotationInterpolationRoot.timestamp) / (interpolationTarget.timestamp - rotationInterpolationRoot.timestamp);
                playerRigidbody.MoveRotation(Quaternion.Slerp(Quaternion.AngleAxis(rotationInterpolationRoot.data, Vector3.up), Quaternion.AngleAxis(interpolationTarget.data, Vector3.up), lerpRate));
            }
        }

        private bool ReadyToInterpolate<T>(ref InterpolationData<T> interpolationRoot,
            ref Queue<InterpolationData<T>> updateQueue, float currentInterpolationTime)
        {
            TrimOldUpdates(ref interpolationRoot, ref updateQueue, currentInterpolationTime);
            if (ShouldWaitForMoreUpdates<T>(interpolationRoot, updateQueue, currentInterpolationTime))
            {
                return false;
            }
            return true;
        }

        private void UpdateAnimation()
        {
            Debug.LogWarning("UpdateAnimation");
            if (ShouldUpdateAnimation(playerRigidbody.velocity.magnitude))
            {
                Debug.LogWarning("ShouldUpdateAnimation");
                var playerMovement = transform.InverseTransformDirection(Vector3.ClampMagnitude(playerRigidbody.velocity, 1));
                var playerTurn = Mathf.Atan2(playerMovement.x, playerMovement.z);
                var playerMotion = playerMovement.magnitude;
                playerAnimator.SetFloat("Forward", playerMotion);
                playerAnimator.SetFloat("Turn", Mathf.Clamp(playerTurn, -0.8f, 0.8f));
            }
        }

        private bool ShouldUpdateAnimation(float velocityMagnitude)
        {
            return velocityMagnitude < SimulationSettings.PlayerMovementSpeed && !(velocityMagnitude > 0 && velocityMagnitude < 1) && playerAnimator.gameObject.activeSelf;
        }

        private void TrimOldUpdates<T>(ref InterpolationData<T> interpolationRoot,
            ref Queue<InterpolationData<T>> updateQueue, float currentInterpolationTime)
        {
            while (updateQueue.Count > 0)
            {
                for (var updateNum = 0; updateNum < updateQueue.Count; updateNum++)
                {
                    if (updateQueue.Peek().timestamp < currentInterpolationTime)
                    {
                        interpolationRoot = updateQueue.Dequeue();
                        continue;
                    }
                    return;
                }
            }
        }

        private bool ShouldWaitForMoreUpdates<T>(InterpolationData<T> interpolationRoot, Queue<InterpolationData<T>> updateQueue, float currentInterpolationTime)
        {
            return updateQueue.Count < 1 || currentInterpolationTime < interpolationRoot.timestamp;
        }

        private void DisableAuthoritativeRigidbodyBehaviour()
        {
            playerRigidbody.useGravity = false;
            playerRigidbody.isKinematic = true;
        }

        private void EnableAuthoritativeRigidbodyBehaviour()
        {
            playerRigidbody.useGravity = true;
            playerRigidbody.isKinematic = false;
        }

        class InterpolationData<T>
        {
            public readonly T data;
            public readonly float timestamp;

            public InterpolationData(T data, float timestamp)
            {
                this.data = data;
                this.timestamp = timestamp;
            }
        }
    }
}

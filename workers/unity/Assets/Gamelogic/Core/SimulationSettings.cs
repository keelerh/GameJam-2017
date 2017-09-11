using UnityEngine;

namespace Assets.Gamelogic.Core
{
    public static class SimulationSettings
    {
        public static readonly string PlayerPrefabName = "Player";
        public static readonly string PlayerCreatorPrefabName = "PlayerCreator";
        public static readonly string CubePrefabName = "Cube";

        public static readonly float HeartbeatCheckIntervalSecs = 3;
        public static readonly uint TotalHeartbeatsBeforeTimeout = 3;
        public static readonly float HeartbeatSendingIntervalSecs = 3;

        public static readonly int TargetClientFramerate = 60;
        public static readonly int TargetServerFramerate = 60;
        public static readonly int FixedFramerate = 20;

        public static readonly float PlayerCreatorQueryRetrySecs = 4;
        public static readonly float PlayerEntityCreationRetrySecs = 4;

        public static readonly string DefaultSnapshotPath = Application.dataPath + "/../../../snapshots/default.snapshot";

        public static readonly Quaternion InitialThirdPersonCameraRotation = Quaternion.Euler(40, 0, 0);
        public static readonly string MouseScrollWheel = "Mouse ScrollWheel";

        public static readonly Vector3 FirstPersonCameraOffset = new Vector3(0, 1.4f, 0);
        public static readonly Vector3 ThirdPersonCameraOffset = new Vector3(0.4f, 0f, -2f);
        public static readonly Vector3 ThirdPersonCameraRootOffset = new Vector3(0f, 1.3f, -0f);

        public static readonly float FirstPersonCameraSensitivity = 1f;
        public static readonly float FirstPersonCameraMaxPitch = 70f;
        public static readonly float FirstPersonCameraMinPitch = -80f;

        public static readonly float PlayerMovementSpeed = 4f;
        public static readonly float PlayerPositionUpdateMinSqrDistance = 0.001f;
        public static readonly float ReasonableMaxTransformUpdateSeparation = 0.5f;
        public static readonly float PlayerPositionUpdateMaxSqrDistance = PlayerMovementSpeed * PlayerMovementSpeed;
        public static readonly float AngleQuantisationFactor = 2f;

        public static readonly float OtherPlayerUpdateDelay = 0.5f;

        // Third Person Controls
        public static int ThirdPersonRotateCameraMouseButton = 1;
        public static float ThirdPersonCameraSensitivity = 4;
        public static float ThirdPersonCameraDefaultDistance = 10;
        public static float ThirdPersonCameraMinDistance = 5;
        public static float ThirdPersonCameraMaxDistance = 22;
        public static float ThirdPersonCameraDistanceSensitivity = 5;
        public static float ThirdPersonCameraMaxPitch = 70;
        public static float ThirdPersonCameraMinPitch = 20;
        public static float ThirdPersonCameraDefaultPitch = 25;
    }
}

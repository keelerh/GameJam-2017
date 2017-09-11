using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity.Core.Acls;
using Improbable.Unity.Entity;
using Improbable.Worker;
using UnityEngine;

namespace Assets.Gamelogic.EntityTemplates
{
    public class EntityTemplateFactory : MonoBehaviour
    {
        public static Entity CreatePlayerCreatorTemplate()
        {
            var template = EntityBuilder.Begin()
                .AddPositionComponent(Vector3.zero, CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(SimulationSettings.PlayerCreatorPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new PlayerCreation.Data(), CommonRequirementSets.PhysicsOnly)
                .Build();

            return template;
        }

        public static Entity CreatePlayerTemplate(string clientId)
        {
            var template = EntityBuilder.Begin()
                .AddPositionComponent(Vector3.zero, CommonRequirementSets.SpecificClientOnly(clientId))
                .AddMetadataComponent(SimulationSettings.PlayerPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new ClientAuthorityCheck.Data(), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddComponent(new ClientConnection.Data(SimulationSettings.TotalHeartbeatsBeforeTimeout), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new PlayerRotation.Data(yaw: 0), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddComponent(new PlayerMovement.Data(), CommonRequirementSets.SpecificClientOnly(clientId))
                .Build();

            return template;
        }

        public static Entity CreateCubeTemplate()
        {
            var template = EntityBuilder.Begin()
                .AddPositionComponent(new Vector3(0,1,5), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(SimulationSettings.CubePrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .Build();

            return template;
        }
    }
}

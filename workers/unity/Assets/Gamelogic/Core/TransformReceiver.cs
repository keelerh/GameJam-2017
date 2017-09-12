using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    public class TransformReceiver : MonoBehaviour
    {
        [Require] private Position.Reader PositionReader;

        void OnEnable()
        {
            transform.position = PositionReader.Data.coords.ToVector3();

            PositionReader.ComponentUpdated.Add(OnComponentUpdated);
        }

        void OnDisable()
        {
            PositionReader.ComponentUpdated.Remove(OnComponentUpdated);
        }

        void OnComponentUpdated(Position.Update update)
        {
            if (!PositionReader.HasAuthority)
            {
                if (update.coords.HasValue)
                {
                    transform.position = update.coords.Value.ToVector3();
                }
            }
        }
    }
}
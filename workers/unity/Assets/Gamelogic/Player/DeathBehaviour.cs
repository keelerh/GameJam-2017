using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
 
namespace Assets.Gamelogic.Player
{
    // Add this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class DeathBehaviour : MonoBehaviour
    {
        // Inject access to the entity's Health component
        [Require] private Health.Reader HealthReader;
 
        public Animation DeathAnimation;
 
        private bool alreadyDead = false;
 
        private void OnEnable()
        {
            alreadyDead = false;
            InitializeDeathAnimation();
            // Register callback for when components change
            HealthReader.CurrentHealthUpdated.Add(OnCurrentHealthUpdated);
        }
 
        private void OnDisable()
        {
            // Deregister callback for when components change
            HealthReader.CurrentHealthUpdated.Remove(OnCurrentHealthUpdated);
        }
 
        private void InitializeDeathAnimation()
        {

            if (HealthReader.Data.currentHealth <= 0)
            {
                foreach (AnimationState state in DeathAnimation)
                {
                    // Jump to end of the animation
                    state.normalizedTime = 1;
                }
                VisualiseDeath();
                alreadyDead = true;
            }
        }
 
        // Callback for whenever the CurrentHealth property of the Health component is updated
        private void OnCurrentHealthUpdated(int currentHealth)
        {
            if (!alreadyDead && currentHealth <= 0)
            {
                VisualiseDeath();
                alreadyDead = true;
            }
        }
 
        private void VisualiseDeath()
        {
            DeathAnimation.Play();
        }
    }
}
using UnityEngine;
using SoulsLikeIsh.Input;

namespace SoulsLikeIsh.World.Encounters
{
    [RequireComponent(typeof(SphereCollider))]
    public class EncounterChest : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private GameObject lockedVisual;
        [SerializeField] private GameObject unlockedVisual;

        public bool IsUnlocked { get; private set; }
        public bool IsClaimed { get; private set; }

        private bool _playerInRange;

        private void Awake() => SetVisual(false);

        private void OnEnable() => inputReader.OnInteract += TryClaim;
        private void OnDisable() => inputReader.OnInteract -= TryClaim;

        public void Unlock()
        {
            if (IsUnlocked) return;
            IsUnlocked = true;
            SetVisual(true);
        }

        private void SetVisual(bool unlocked)
        {
            if (lockedVisual != null) lockedVisual.SetActive(!unlocked);
            if (unlockedVisual != null) unlockedVisual.SetActive(unlocked);
        }

        private void TryClaim()
        {
            if (!IsUnlocked || IsClaimed || !_playerInRange) return;
            IsClaimed = true;
            // TODO: hook into loot/inventory once it exists.
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) _playerInRange = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) _playerInRange = false;
        }
    }
}
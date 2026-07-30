using UnityEngine;
using SoulsLikeIsh.Character.Player;

namespace SoulsLikeIsh.UI
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private StatBar healthBar;
        [SerializeField] private StatBar staminaBar;

        private void Awake()
        {
            if (player == null)
                player = FindFirstObjectByType<PlayerController>();
        }

        private void Update()
        {
            healthBar.SetValue(player.Health.Current, player.Health.Max);
            staminaBar.SetValue(player.Stamina.Current, player.Stamina.Max);
        }
    }
}
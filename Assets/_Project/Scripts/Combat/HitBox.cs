using System.Collections.Generic;
using UnityEngine;

namespace SoulsLikeIsh.Combat
{
    [RequireComponent(typeof(Collider))]
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private GameObject owner;

        private Collider _collider;
        private readonly HashSet<IDamageable> _hitTargets = new();
        private int _damage;

        public GameObject Owner => owner;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _collider.enabled = false;
            if (owner == null) owner = transform.root.gameObject;

            if (gameObject.TryGetComponent(out Rigidbody rb))
            {
                if (rb == null)
                {
                    rb = gameObject.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
                else
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }
        }

        public void SetDamage(int damage) => _damage = damage;

        public void EnableHitbox()
        {
            _hitTargets.Clear();
            _collider.enabled = true;
        }

        public void DisableHitbox() => _collider.enabled = false;

        private void OnTriggerEnter(Collider other)
        {
            var damageable = other.GetComponentInParent<IDamageable>();
            if (damageable == null || _hitTargets.Contains(damageable)) return;

            var hurtbox = other.GetComponent<Hurtbox>();
            if (hurtbox != null && hurtbox.OwnerRoot == owner) return;

            _hitTargets.Add(damageable);
            damageable.TakeDamage(new DamageInfo(_damage, owner));
        }
    }
}
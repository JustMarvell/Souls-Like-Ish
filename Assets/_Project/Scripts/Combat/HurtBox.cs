using UnityEngine;

namespace SoulsLikeIsh.Combat
{
    [RequireComponent(typeof(Collider))]
    public class Hurtbox : MonoBehaviour
    {
        [SerializeField] private GameObject ownerRoot;

        public GameObject OwnerRoot => ownerRoot != null ? ownerRoot : transform.root.gameObject;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            if (ownerRoot == null) ownerRoot = transform.root.gameObject;
        }
    }
}
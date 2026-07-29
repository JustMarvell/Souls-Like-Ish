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
            try
            {
                GetComponent<Collider>().isTrigger = true;
            }
            catch
            {
                print ("Something wrong");
            }

            if (ownerRoot == null) ownerRoot = transform.root.gameObject;
        }
    }
}
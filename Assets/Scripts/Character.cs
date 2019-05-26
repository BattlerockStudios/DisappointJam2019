using UnityEngine;

namespace Battlerock
{
    [RequireComponent(typeof(AudioSource))]
    public class Character : MonoBehaviour
    {
        #region Public Variables

        public Stats stats;

        #endregion

        #region Private Variables

        protected Rigidbody _rigidbody;

        #endregion
    }

    [System.Serializable]
    public class Stats
    {
        public float speed;
        public float rotateSpeed;
    }
}
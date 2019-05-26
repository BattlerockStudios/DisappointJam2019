using UnityEngine;
namespace Battlerock
{
    [RequireComponent(typeof(SphereCollider))]
    public class LookAtTarget : MonoBehaviour
    {
        // speed is the rate at which the object will rotate
        private float m_speed = 100.0f;

        public bool flipRotation = false;

        [SerializeField]
        private bool m_useMouse = false;

        [SerializeField]
        private Vector2 m_clampRotation = new Vector2(-45.0f, 45.0f);

        private Quaternion m_originalRotation;

        public LayerMask focusLayers;

        [SerializeField]
        private Transform m_focusPoint;

        private void Start()
        {
            var sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.radius = 5.0f;
            sphereCollider.isTrigger = true;



            m_originalRotation = transform.localRotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (UtilityExtensions.IsInLayerMask(layer: other.gameObject.layer, layermask: focusLayers))
            {
                m_focusPoint = other.transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (UtilityExtensions.IsInLayerMask(layer: other.gameObject.layer, layermask: focusLayers))
            {
                m_focusPoint = null;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (UtilityExtensions.IsInLayerMask(layer: other.gameObject.layer, layermask: focusLayers) && m_focusPoint == null)
            {
                m_focusPoint = other.transform;
            }
        }

        void LateUpdate()
        {
            if (m_useMouse == true)
            {
                // Generate a plane that intersects the transform's position with an upwards normal.
                Plane playerPlane = new Plane(Vector3.up, transform.position);

                // Generate a ray from the cursor position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Determine the point where the cursor ray intersects the plane.
                // This will be the point that the object must look towards to be looking at the mouse.
                // Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
                //   then find the point along that ray that meets that distance.  This will be the point
                //   to look at.
                float hitdist = 0.0f;
                // If the ray is parallel to the plane, Raycast will return false.
                if (playerPlane.Raycast(ray, out hitdist))
                {
                    // Get the point along the ray that hits the calculated distance.
                    Vector3 targetPoint = ray.GetPoint(hitdist);

                    //Determine the target rotation.  This is the rotation if the transform looks at the target point.
                    Vector3 lookRotation = (transform.position - targetPoint);
                    if (flipRotation == true)
                    {
                        lookRotation = -(transform.position - targetPoint);
                    }

                    Quaternion targetRotation = Quaternion.LookRotation(lookRotation, Vector3.up);

                    //Smoothly rotate towards the target point.
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_speed * Time.deltaTime);

                }
            }
            else
            {              

                if (m_focusPoint == null)
                {
                    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, m_originalRotation, Time.deltaTime * m_speed);
                    return;
                }
                LookAtSmoothly(m_focusPoint, Vector3.up);

                var y = UtilityExtensions.ClampAngle(transform.localEulerAngles.y, m_clampRotation.x, m_clampRotation.y, 0);

                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
            }
        }

        private void LookAtSmoothly(Transform target, Vector3 worldUp)
        {
            Quaternion direction = Quaternion.LookRotation(target.position - transform.position, worldUp);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, direction, Time.deltaTime * m_speed);
        }


    }
}
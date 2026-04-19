using UnityEngine;
using UnityEngine.UI;

namespace MercenaryWars.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private Vector3 offset = new Vector3(0f, 0.8f, 0f);

        private Transform target;

        public void Setup(Transform followTarget, float initialFill = 1f)
        {
            target = followTarget;
            Debug.Log("HealthBar Setup called on: " + followTarget.name);
            UpdateFill(initialFill);
        }

        public void UpdateFill(float fraction)
        {
            if (fillImage == null)
            {
                Debug.LogError("HealthBar: Fill Image is NULL!");
                return;
            }
            fillImage.fillAmount = Mathf.Clamp01(fraction);
            Debug.Log("HealthBar fill set to: " + fraction);
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
            transform.position = target.position + offset;
            transform.rotation = Quaternion.identity;
        }
    }
}
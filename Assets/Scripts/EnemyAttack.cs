using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float range;
    [SerializeField] float radius;

    public void Attack()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(
            origin,
            radius,
            direction,
            range,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject == this.transform.root.gameObject)
            {
                continue;
            }

            if (hitObject.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage);
            }
        }
    }
}
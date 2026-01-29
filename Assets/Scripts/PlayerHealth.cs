using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int health = 100;

    //[Header("Events")]
    //[SerializeField] IntEvent OnHealthChange;

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log($"-{damageAmount} health.");  

        if (health < 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died");
    }
}

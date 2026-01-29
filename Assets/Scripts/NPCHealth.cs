using UnityEngine;

public class NPCHealth : MonoBehaviour
{
    //[SerializeField] string name;
    [SerializeField] int health = 100;

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"-{amount} health ({"NPC"}).");

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

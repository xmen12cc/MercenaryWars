using UnityEngine;
using MercenaryWars.Managers;
using MercenaryWars.Player;

namespace MercenaryWars.Environment
{
    public class LevelGoal : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"LevelGoal OnTriggerEnter2D triggered by: {other.gameObject.name}");
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player triggered LevelGoal. Loading next level.");
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.LoadNextLevel();
                }
                else
                {
                    Debug.LogError("GameManager.Instance is null! Cannot load next level.");
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"LevelGoal OnCollisionEnter2D triggered by: {collision.gameObject.name}");
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player collided with LevelGoal. Loading next level.");
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.LoadNextLevel();
                }
                else
                {
                    Debug.LogError("GameManager.Instance is null! Cannot load next level.");
                }
            }
        }
    }
}

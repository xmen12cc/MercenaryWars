using UnityEngine;
using UnityEngine.SceneManagement;

namespace MercenaryWars.UI
{
    /// <summary>
    /// Control main menu UI buttons and panels.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Header("UI Panels")]
        [Tooltip("Panel showing game instructions.")]
        [SerializeField] private GameObject instructionsPanel;

        private void Start()
        {
            // Ensure instructions panel hidden on start
            if (instructionsPanel != null)
            {
                instructionsPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Start game. Load Level 1.
        /// </summary>
        public void StartGame()
        {
            // Use GameManager if it exists, otherwise load scene directly
            if (Managers.GameManager.Instance != null)
            {
                Managers.GameManager.Instance.LoadLevel1();
            }
            else
            {
                SceneManager.LoadScene("Level1");
            }
        }

        /// <summary>
        /// Toggle instructions panel on or off.
        /// </summary>
        public void ShowInstructions()
        {
            if (instructionsPanel != null)
            {
                bool isActive = instructionsPanel.activeSelf;
                instructionsPanel.SetActive(!isActive);
            }
            else
            {
                Debug.LogWarning("Instructions panel missing. Assign in Inspector.");
            }
        }

        /// <summary>
        /// Quit game application.
        /// </summary>
        public void ExitGame()
        {
            Debug.Log("Quit game.");
            Application.Quit();
            
#if UNITY_EDITOR
            // Stop play mode in editor
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}

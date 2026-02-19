using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject winPanel;
    public GameObject losePanel;
    private bool gameEnded = false;

    void Awake() => Instance = this;

    void Update()
    {
        if (gameEnded) return;
        TeamLeader[] leaders = Object.FindObjectsByType<TeamLeader>(FindObjectsSortMode.None);
        if (leaders.Length == 1 && leaders[0].teamType == MinionUnit.Team.Player)
        {
            TriggerWin();
        }
    }

    public void TriggerWin()
    {
        gameEnded = true;
        winPanel.SetActive(true);
    }

    public void TriggerGameOver()
    {
        gameEnded = true;
        losePanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
using UnityEngine;

public class PlayAgainScreenHandler : MonoBehaviour {

    public Animator gameOverAnimator;
    public TMPro.TextMeshProUGUI gameOverText;

    public void OnPlayAgainButtonClicked() {
        gameOverAnimator.SetTrigger("PlayAgain");
        GameController.instance.StartGame();
    }

    public void GameOver() {
        gameOverText.text = "GAME OVER";
        gameOverAnimator.SetTrigger("GameOver");
    }

    public void GameWon() {
        gameOverText.text = "YOU WON";
        gameOverAnimator.SetTrigger("GameOver");
    }

}
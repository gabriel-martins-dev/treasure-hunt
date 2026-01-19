using TMPro;
using TreasureHunt.Presentation;
using UnityEngine;

public class ResultHUDView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;

    GameViewModel viewModel;

    const string WinMessage = "You Found the Treasure! You Win!";
    const string LossMessage = "Game Over! Out of Attempts!";

    public void Bind(GameViewModel viewModel)
    {
        this.viewModel = viewModel;
        this.viewModel.GameStarted += HandleGameStarted;
        this.viewModel.GameFinished += HandleGameFinished;
    }

    void HandleGameFinished(bool victory)
    {
        this.resultText.gameObject.SetActive(true);
        this.resultText.text = victory ? WinMessage : LossMessage;
    }

    void HandleGameStarted()
    {
        this.resultText.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (this.viewModel != null)
        {
            this.viewModel.GameStarted -= HandleGameStarted;
            this.viewModel.GameFinished -= HandleGameFinished;
        }
    }
}

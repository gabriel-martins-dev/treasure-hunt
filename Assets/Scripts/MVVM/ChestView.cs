namespace TreasureHunt.View
{
    using UnityEngine;
    using UnityEngine.UI;
    using TreasureHunt.Presentation;
    using TMPro;

    public class ChestView : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private Button actionButton;

        private ChestViewModel viewModel;

        public void Bind(ChestViewModel viewModel)
        {
            this.viewModel = viewModel;

            this.viewModel.StateChanged += OnStateChanged;
            this.actionButton.onClick.AddListener(HandleClick);

            // starting visual state
            this.OnStateChanged(this.viewModel.State);
            this.transform.SetSiblingIndex(this.viewModel.Index);
        }

        private void HandleClick()
        {
            this.viewModel.OnClicked();
        }

        private void OnStateChanged(ChestState state)
        {
            this.stateText.text = state.ToString();

            switch (state)
            {
                case ChestState.Idle:
                    this.background.color = Color.gray;
                    this.actionButton.interactable = true;
                    break;
                case ChestState.Opening:
                    this.background.color = Color.yellow;
                    this.actionButton.interactable = true; // Still interactable to allow interruption
                    break;
                case ChestState.Opened:
                    this.background.color = Color.white;
                    this.actionButton.interactable = false;
                    break;
            }
        }

        private void OnDestroy()
        {
            if (this.viewModel != null)
            {
                this.viewModel.StateChanged -= OnStateChanged;
            } 
            this.actionButton.onClick.RemoveAllListeners();
        }
    }
}
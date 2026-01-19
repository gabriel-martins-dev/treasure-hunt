namespace TreasureHunt.View
{
    using UnityEngine;
    using UnityEngine.UI;
    using TreasureHunt.Presentation;
    using TMPro;

    public class ChestView : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private Button actionButton;
        [SerializeField] private Image background;
        [SerializeField] private Image chest;
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite emptySprite;
        [SerializeField] private Sprite treasureSprite;

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
            switch (state)
            {
                case ChestState.Idle:
                    this.background.enabled = false;
                    this.actionButton.interactable = true;
                    this.chest.sprite = this.idleSprite;
                    break;
                case ChestState.Opening:
                    this.background.enabled = true;
                    this.background.color = Color.yellow;
                    this.actionButton.interactable = true; // Still interactable to allow interruption
                    break;
                case ChestState.Opened:
                    this.background.enabled = false;
                    this.actionButton.interactable = false;
                    this.chest.sprite = this.viewModel.IsWinner ? this.treasureSprite : this.emptySprite;

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
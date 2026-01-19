namespace TreasureHunt.View
{
    using System;
    using System.Collections.Generic;
    using TreasureHunt.Presentation;
    using UnityEngine;
    using UnityEngine.UI;

    public class ChestView : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private Button actionButton;
        [SerializeField] private Image background;
        [SerializeField] private Image chest;
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite emptySprite;
        [SerializeField] private Sprite treasureSprite;

        ChestViewModel viewModel;
        Dictionary<ChestState, Action> stateHandlers;

        public void Bind(ChestViewModel viewModel)
        {
            this.viewModel = viewModel;

            this.stateHandlers = new Dictionary<ChestState, Action>
            {
                { ChestState.Idle, this.HandleIdleState },
                { ChestState.Opening, this.HandleOpeningState },
                { ChestState.Opened, this.HandleOpenedState },
            };

            this.viewModel.StateChanged += OnStateChanged;
            this.actionButton.onClick.AddListener(HandleClick);

            // starting visual state
            this.OnStateChanged(this.viewModel.State);
            this.transform.SetSiblingIndex(this.viewModel.Index);
        }

        void HandleClick()
        {
            this.viewModel.OnClicked();
        }

        void OnStateChanged(ChestState state)
        {
            if (this.stateHandlers.TryGetValue(state, out var handler))
            {
                handler.Invoke();
            }
        }

        void HandleIdleState()
        {
            this.background.enabled = false;
            this.actionButton.interactable = true;
            this.chest.sprite = this.idleSprite;
        }

        void HandleOpeningState()
        {
            this.background.enabled = true;
            this.background.color = Color.yellow;
            this.actionButton.interactable = false;
        }

        void HandleOpenedState()
        {
            this.background.enabled = false;
            this.actionButton.interactable = false;
            this.chest.sprite = this.viewModel.IsWinner ? this.treasureSprite : this.emptySprite;
        }

        void OnDestroy()
        {
            if (this.viewModel != null)
            {
                this.viewModel.StateChanged -= OnStateChanged;
            } 
            this.actionButton.onClick.RemoveAllListeners();
        }
    }
}
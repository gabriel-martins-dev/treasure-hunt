namespace TreasureHunt.View
{
    using System;
    using System.Collections.Generic;
    using TreasureHunt.Presentation;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Handles the sprites and UI states of single chest
    /// </summary>
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

        void Start()
        {
            this.actionButton.onClick.AddListener(HandleClick);
        }

        public void Bind(ChestViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.viewModel.StateUpdated += this.HandleStateUpdated;

            this.stateHandlers ??= new Dictionary<ChestState, Action>
            {
                { ChestState.Idle, this.HandleIdleState },
                { ChestState.Opening, this.HandleOpeningState },
                { ChestState.Opened, this.HandleOpenedState },
                { ChestState.Locked, this.HandleLockedState }
            };

            // starting visual state
            this.HandleStateUpdated(this.viewModel.State);
            this.transform.SetSiblingIndex(this.viewModel.Index);
        }

        void HandleClick()
        {
            this.viewModel.OnClicked();
        }

        void HandleStateUpdated(ChestState state)
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
            this.actionButton.interactable = true;
        }

        void HandleOpenedState()
        {
            this.background.enabled = false;
            this.actionButton.interactable = false;
            this.chest.sprite = this.viewModel.IsWinner ? this.treasureSprite : this.emptySprite;
        }

        void HandleLockedState()
        {
            this.actionButton.interactable = false;
        }

        void OnDestroy()
        {
            if (this.viewModel != null)
            {
                this.viewModel.StateUpdated -= HandleStateUpdated;
            } 
            this.actionButton.onClick.RemoveAllListeners();
        }
    }
}
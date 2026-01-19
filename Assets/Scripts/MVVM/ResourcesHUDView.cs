namespace TreasureHunt.View
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using TreasureHunt.Presentation;
    using TreasureHunt.Services;
    using UnityEngine;

    [Serializable]
    public class ResourceHUDItem
    {
        [SerializeField] string name;
        [SerializeField] TextMeshProUGUI amountText;

        public string Name => name;
        public void UpdateValue(int newValue)
        {
            this.amountText.text = $"{name}: {newValue}";
        }
    }

    public class ResourcesHUDView : MonoBehaviour
    {
        [SerializeField] private ResourceHUDItem[] resourceCounter;

        readonly Dictionary<string, ResourceHUDItem> resoucesMap = new();
        GameViewModel viewModel;

        public void Bind(GameViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.viewModel.ResourceUpdate += HandleResourceUpdated;

            for (int i = 0; i < resourceCounter.Length; i++)
            {
                this.resoucesMap.Add(resourceCounter[i].Name, resourceCounter[i]);
                resourceCounter[i].UpdateValue(0);
            }
        }

        private void HandleResourceUpdated(ResourceUpdateEvent resourceUpdateEvent)
        {
            if (this.resoucesMap.TryGetValue(resourceUpdateEvent.Name, out var hudItem))
            {
                hudItem.UpdateValue(resourceUpdateEvent.NewTotal);
            }
        }

        void OnDestroy()
        {
            if (viewModel != null)
            {
                viewModel.ResourceUpdate -= HandleResourceUpdated;
            }
        }
    }
}
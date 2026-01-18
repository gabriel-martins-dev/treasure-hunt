using NUnit.Framework;
using TreasureHunt.GameMode;
using TreasureHunt.Services;
using TreasureHunt.Tests.Mocks;
using UnityEngine;
using UnityEngine.TestTools;

public class TreasureHuntGameModeTests
{
    private TreasureHuntGameMode gameMode;
    private MockRoundConfig mockConfig;
    private MockWalletService mockWallet;
    private MockRewardService mockReward;
    private MockRandomService mockRandom;

    [SetUp]
    public void Setup()
    {
        this.mockConfig = new MockRoundConfig();
        this.mockWallet = new MockWalletService();
        this.mockReward = new MockRewardService("Coins", 100);  
        this.mockRandom = new MockRandomService(5);
        this.gameMode = new TreasureHuntGameMode(mockConfig, mockWallet, mockReward, mockRandom);
    }

    [Test]
    public void StartGameInitialAttempts()
    {
        int? updatedAttempts = null;
        this.gameMode.AttemptsUpdated += (attempts) =>
        {
            updatedAttempts = attempts;
        };

        this.gameMode.StartGame();

        Assert.IsNotNull(updatedAttempts, "StartGame should fire initial max attempts event");
        Assert.AreEqual(updatedAttempts, this.mockConfig.MaxAttemptsPerRound);
    }

    [Test]
    public void OpenActionWrongIndexDecrementsAttempts()
    {
        var updatedAttempts = -1;

        this.gameMode.StartGame();
        this.gameMode.AttemptsUpdated += (attempts) => updatedAttempts = attempts;
        this.gameMode.OpenAction(1);
        
        Assert.AreEqual(this.mockConfig.MaxAttemptsPerRound - 1, updatedAttempts);
    }

    [Test]
    public void  OpenActionTriggersLoss()
    {
        this.mockConfig.MaxAttemptsPerRound = 1; // Set up scenario where we only have 1 life
        bool? victory = null;
        this.gameMode.GameCompleted += (win) => victory = win;

        this.gameMode.StartGame();
        this.gameMode.OpenAction(1);

        Assert.IsNotNull(victory, "GameCompleted event should fire");
        Assert.IsFalse(victory.Value, "Should be a Loss");
    }

    [Test]
    public void OpenActionTriggersVictoryAndReward()
    {
        this.mockConfig.MaxAttemptsPerRound = 3;
        var victory = false;
        ResourceUpdateEvent? reward = null;
        this.gameMode.GameCompleted += (win) => victory = win;
        this.mockWallet.ResourceUpdated += (evt) => reward = evt;

        this.gameMode.StartGame();
        this.gameMode.OpenAction(5);

        Assert.IsTrue(victory, "Should be a victory");
        Assert.IsNotNull(reward, "GameCompleted event should make Wallet Service receive reward");
        Assert.AreEqual("Coins", reward.Value.Name);
        Assert.AreEqual(100, reward.Value.Amount);
        Assert.AreEqual(100, reward.Value.NewTotal);
    }

    [Test]
    public void OpenActionAfterGameCompleted()
    {
        this.mockConfig.MaxAttemptsPerRound = 3;
        var totalAttempts = 0;
        this.gameMode.AttemptsUpdated += (attempts) => totalAttempts = attempts;

        this.gameMode.StartGame();
        this.gameMode.OpenAction(5); // win game
        this.gameMode.OpenAction(0); // open again after game is completeded

        var expectedMessage = "[GameMode] User tried to open chest in finished game.";
        LogAssert.Expect(LogType.Error, expectedMessage);
        Assert.AreEqual(totalAttempts, this.mockConfig.MaxAttemptsPerRound - 1);
    }
}

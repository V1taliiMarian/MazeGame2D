using LabyrinthGame;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;

public class CollisionManager
{
    private readonly MazeService mazeService;
    private readonly Canvas canvas;
    private readonly Player player;
    private readonly Rectangle exit;
    private readonly List<Rectangle> coins;
    private readonly CoinManager coinManager;
    private readonly StatisticsManager statsManager;
    public event Action OnCoinCollected;

    public CollisionManager(
        MazeService mazeService,
        Canvas canvas,
        Player player,
        Rectangle exit,
        List<Rectangle> coins,
        CoinManager coinManager,
        StatisticsManager statsManager)
    {
        this.mazeService = mazeService;
        this.canvas = canvas;
        this.player = player;
        this.exit = exit;
        this.coins = coins;
        this.coinManager = coinManager;
        this.statsManager = statsManager;
    }
    public void CheckAllCollisions()
    {
        CheckWallCollisions();
        CheckCoinCollisions();
        CheckExitCollision();
    }

    private void CheckWallCollisions()
    {
        int playerX = (int)((Canvas.GetLeft(player.GetVisual()) - mazeService.CellSize * 0.1) / mazeService.CellSize);
        int playerY = (int)((Canvas.GetTop(player.GetVisual()) - mazeService.CellSize * 0.1) / mazeService.CellSize);

        if (mazeService.IsWallAt(playerX, playerY))
        {
            player.UndoMove();
            AudioManager.PlaySound("hitwallsound.mp3", 0.4f);
        }
        else
        {
            AudioManager.PlaySound("stepsound.mp3", 0.4f); 
        }
    }

    private void CheckCoinCollisions()
    {
        int playerX = (int)((Canvas.GetLeft(player.GetVisual()) - mazeService.CellSize * 0.1) / mazeService.CellSize);
        int playerY = (int)((Canvas.GetTop(player.GetVisual()) - mazeService.CellSize * 0.1) / mazeService.CellSize);

        for (int i = coins.Count - 1; i >= 0; i--)
        {
            var coin = coins[i];
            int coinX = (int)((Canvas.GetLeft(coin) - mazeService.CellSize * 0.2) / mazeService.CellSize);
            int coinY = (int)((Canvas.GetTop(coin) - mazeService.CellSize * 0.2) / mazeService.CellSize);

            if (playerX == coinX && playerY == coinY)
            {
                canvas.Children.Remove(coin);
                coins.RemoveAt(i);
                coinManager.AddCoins(1);
                statsManager.IncrementCoinsCollected();
                AudioManager.PlaySound("coinsound.mp3");
                OnCoinCollected?.Invoke(); 
            }
        }
    }

    private void CheckExitCollision()
    {
        int playerX = (int)((Canvas.GetLeft(player.GetVisual()) - mazeService.CellSize * 0.1) / mazeService.CellSize);
        int playerY = (int)((Canvas.GetTop(player.GetVisual()) - mazeService.CellSize * 0.1) / mazeService.CellSize);
        int exitX = (int)((Canvas.GetLeft(exit) - mazeService.CellSize * 0.1) / mazeService.CellSize);
        int exitY = (int)((Canvas.GetTop(exit) - mazeService.CellSize * 0.1) / mazeService.CellSize);

        if (playerX == exitX && playerY == exitY)
        {
            AudioManager.PlaySound("winsound.mp3");
            OnExitReached?.Invoke();
        }
    }
    public event Action OnExitReached;
}

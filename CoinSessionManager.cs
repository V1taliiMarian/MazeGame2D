using System.Windows.Controls;

namespace LabyrinthGame
{
    public class CoinSessionManager
    {
        private readonly CoinManager coinManager;
        private int sessionStartCoins;
        private int sessionCollectedCoins;

        public int SessionCollectedCoins => sessionCollectedCoins;

        public CoinSessionManager(CoinManager coinManager)
        {
            this.coinManager = coinManager;
        }

        public void StartSession()
        {
            sessionStartCoins = coinManager.Coins;
            sessionCollectedCoins = 0;
        }

        public void UpdateSessionCoins()
        {
            sessionCollectedCoins = coinManager.Coins - sessionStartCoins;
        }

        public void UpdateCoinBalance(TextBlock coinTextBlock = null)
        {
            coinManager.UpdateCoinBalance(coinTextBlock);
            UpdateSessionCoins();
        }
    }
}

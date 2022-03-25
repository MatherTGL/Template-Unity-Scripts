using UIGame;

namespace Data.Player
{
    public struct PlayerData
    {
        private static ushort _score;


        public static void AddScore(ushort amount)
        {
            _score += amount;
            UpdateUIElements.UIScoreDisplay.UpdateScoreText(_score);
        }

        public static void ClearScore()
        {
            _score = 0;
            UpdateUIElements.UIScoreDisplay.UpdateScoreText(_score);
        }
    }
}
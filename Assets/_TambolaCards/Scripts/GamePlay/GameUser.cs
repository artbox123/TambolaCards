using UnityEngine.UI;
using TMPro;

namespace ArtboxGames
{
    public class GameUser : GameManager
    {
        private string _playerId;
        private string _playerName;
        private string _playerImage;
        private int _claim;

        public Image PlayerImage;
        public TextMeshProUGUI PlayerName;
        public TextMeshProUGUI Claim;

        public string playerId
        {
            get
            {
                return _playerId;
            }
            set
            {
                _playerId = value;
                gameObject.name = value;
            }
        }
        public string playerName
        {
            get
            {
                return _playerName;
            }
            set
            {
                _playerName = value;
                PlayerName.text = value;
            }
        }
        public string playerImage
        {
            get
            {
                return _playerImage;
            }
            set
            {
                _playerImage = value;
                StartCoroutine(LoadImageFromPath(value, PlayerImage));
            }
        }
        public int claim
        {
            get
            {
                return _claim;
            }
            set
            {
                _claim = value;
                Claim.text = "Claim : " + value;
            }
        }
    }
}
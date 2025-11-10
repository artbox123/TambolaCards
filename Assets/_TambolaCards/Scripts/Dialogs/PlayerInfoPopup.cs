using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ArtboxGames
{
    public class PlayerInfoPopup : GameManager
    {
        [SerializeField] private Image playerImage;
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private TextMeshProUGUI playerCoins;
        [SerializeField] private TMP_InputField editName;

        [Space(5)]
        [SerializeField] private TextMeshProUGUI totalPlayed;
        [SerializeField] private TextMeshProUGUI totalWin;
        [SerializeField] private TextMeshProUGUI totalLose;
        [SerializeField] private TextMeshProUGUI winStreak;

        private void OnEnable()
        {
            Invoke("SetPlayerInfo", 0.3f);
        }

        private void SetPlayerInfo()
        {
            playerCoins.text = PlayerInfo.Instance.coins.ToString();
            playerImage.sprite = HomeScreen.Instance.playerImage.sprite;
            playerName.text = PlayerInfo.Instance.userName;
            editName.text = PlayerInfo.Instance.userName;

            totalPlayed.text = PlayerInfo.Instance.totalPlayed.ToString("00");
            totalWin.text = PlayerInfo.Instance.totalWin.ToString("00");
            int lose = PlayerInfo.Instance.totalPlayed - PlayerInfo.Instance.totalWin;
            totalLose.text = lose.ToString("00");
            if (PlayerInfo.Instance.totalPlayed > 0)
            {
                float _winStreak = ((float)PlayerInfo.Instance.totalWin / (float)PlayerInfo.Instance.totalPlayed);
                _winStreak = _winStreak >= 0f ? _winStreak : 0.0f;
                winStreak.text = (_winStreak * 100).ToString("F2") + "%";
            }
        }

        public void Done()
        {
            if (string.IsNullOrEmpty(editName.text))
                ShowMessageBox("Please enter your name");
            else
            {
                playerName.text = editName.text;
                HomeScreen.Instance.playerName.text = editName.text;
                PlayerInfo.Instance.userName = editName.text;

                AdsManager.Instance?.ShowInterstitial();
            }
        }

        public void ChangeProfile(int index)
        {
            playerImage.sprite = GetPlayerImage(index);
            HomeScreen.Instance.playerImage.sprite = GetPlayerImage(index);
            PlayerInfo.Instance.userImage = index.ToString();
        }

        public void PickImage()
        {
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
            {
                //Debug.Log("Image path: " + path);
                if (path != null)
                {
                    // Create Texture from selected image
                    Texture2D texture = NativeGallery.LoadImageAtPath(path);
                    if (texture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        return;
                    }
                    float width = (float)texture.width;
                    float height = (float)texture.height;
                    Rect rect = new Rect(0, 0, width, height);

                    playerImage.sprite = Sprite.Create(texture, rect, new Vector2(0, 0), 1);
                    HomeScreen.Instance.playerImage.sprite = playerImage.sprite;
                    PlayerInfo.Instance.userImage = path;
                }
            }, "Select a image", "image/*");
        }
    }
}
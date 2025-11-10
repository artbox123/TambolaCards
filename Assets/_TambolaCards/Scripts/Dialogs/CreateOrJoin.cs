using TMPro;
using UnityEngine;

namespace ArtboxGames
{
    public class CreateOrJoin : GameManager
    {
        [SerializeField] private TextMeshProUGUI bootValue;

        private void OnEnable()
        {
            int _bootValue = (GetLevel() * 3000);
            bootValue.text = _bootValue.ToString();
            ServerCode.Instance.bootAmount = _bootValue;
        }

        public void CreateParty()
        {
            if (PlayerInfo.Instance.coins >= ServerCode.Instance.bootAmount)
            {
                DoNotDestroy.Instance.loadingPanel.SetActive(true);
                PlayerInfo.Instance.playType = PlayType.Party;
                ServerCode.Instance.CreateNewRoom(GenerateRoomId());
            }
            else
            {
                ShowMessageBox("You dont have enough coins");
            }
        }

        public void JoinParty(TMP_InputField roomCode)
        {
            if (PlayerInfo.Instance.coins >= ServerCode.Instance.bootAmount)
            {
                DoNotDestroy.Instance.loadingPanel.SetActive(true);
                if (string.IsNullOrEmpty(roomCode.text))
                {
                    ShowMessageBox("Please enter room code");
                }
                else
                {
                    PlayerInfo.Instance.playType = PlayType.Party;
                    ServerCode.Instance.JoinRoom(roomCode.text.Trim());
                }
            }
            else
            {
                ShowMessageBox("You dont have enough coins");
            }
        }

        public void Play(int playType)
        {
            if (playType == 0)
            {
                int _bootValue = (GetLevel() * 3000);
                if (!CheckBalance(_bootValue))
                    return;
                PlayerInfo.Instance.playType = PlayType.Play;
                ServerCode.Instance.bootAmount = _bootValue;
                LoadScene("Game", DoNotDestroy.Instance.loadingPanel, 1);
            }
            else if (playType == 1)
            {
                if (!CheckBalance(2000))
                    return;
                PlayerInfo.Instance.playType = PlayType.Party;
                DoNotDestroy.Instance.loadingPanel.SetActive(true);
                //ServerCode.Instance.CreateRoom(false);
            }
        }
    }
}
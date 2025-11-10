using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ArtboxGames
{
    public class PlayerWaiting : GameTargets
    {
        public static PlayerWaiting Instance;

        [SerializeField] private GameObject waitingMsg;
        [SerializeField] private Transform parent;
        [SerializeField] private GameObject oppPlayerPrefab;
        [SerializeField] private TextMeshProUGUI bootAmount;
        [SerializeField] private TextMeshProUGUI roomCode;
        [SerializeField] private GameObject startButton;

        private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
        private ServerCode serverCode;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            serverCode = ServerCode.Instance;
            roomCode.text = "Room code " + serverCode.roomCode;
            bootAmount.text = serverCode.bootAmount.ToString();
        }

        public void Close()
        {
            serverCode.SendDisconnect();
            serverCode.joinedPlayer.Clear();
            players.Clear();

            for (int i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        public void GeneratePlayer()
        {
            if (serverCode.joinedPlayer.Count >= 2)
            {
                waitingMsg.SetActive(false);
                if (PlayerInfo.Instance.playType == PlayType.Party && serverCode.isAdmin)
                    startButton.SetActive(true);
            }
            foreach (PlayerData data in serverCode.joinedPlayer)
            {
                if (data.playerID == PlayerInfo.Instance.userID)
                {
                    data.TrailOrSet = new List<string>();
                    data.TrailOrSet = GetTrailOrSet();

                    data.PureSequence = new List<string>();
                    data.PureSequence = GetPureSequence();

                    data.Sequence = new List<string>();
                    data.Sequence = GetSequence();

                    data.Color = new List<string>();
                    data.Color = GetColor();

                    data.Pair = new List<string>();
                    data.Pair = GetPair();

                    data.HighCard = new List<string>();
                    data.HighCard = GetHighCard();
                }
                else if (!players.ContainsKey(data.playerID))
                {
                    GameObject player = Instantiate(oppPlayerPrefab, parent, false);
                    player.name = data.playerID;
                    Image playerImg = player.transform.Find("Background/ProfileImage").GetComponent<Image>();
                    StartCoroutine(LoadImageFromPath(data.playerImage, playerImg));
                    player.transform.Find("NamePanel/PlayerName").GetComponent<TextMeshProUGUI>().text = data.playerName;
                    players.Add(data.playerID, player.gameObject);
                }
            }
        }

        public void RemovePlayer(string playerId)
        {
            if (players.ContainsKey(playerId))
            {
                GameObject player;
                players.TryGetValue(playerId, out player);
                players.Remove(playerId);
                Destroy(player);

                if (serverCode.joinedPlayer.Count < 2)
                {
                    waitingMsg.SetActive(true);
                    if (PlayerInfo.Instance.playType == PlayType.Party)
                        startButton.SetActive(false);
                }
            }
        }

        public void StartParty()
        {
            serverCode.SendStartParty(PlayerInfo.Instance.userID);
        }

        public void Share()
        {
            NativeShare share = new NativeShare();
            share.SetSubject("Share Party Code").SetTitle("Share").SetText("I want to play Tambola Cards with you! Please install from : " + ShareURL() + " Start game and go to Party and enter Party Code : " + serverCode.roomCode).Share();
        }
    }
}
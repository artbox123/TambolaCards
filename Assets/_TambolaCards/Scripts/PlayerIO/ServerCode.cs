using PlayerIOClient;
using System.Collections.Generic;
using UnityEngine;

namespace ArtboxGames
{
    public class ServerCode : GameManager
    {
        public static ServerCode Instance;
        public GameScreen gameScreen;

        public static Client player;
        public static Connection piocon;
        public List<PlayerData> joinedPlayer = new List<PlayerData>();
        public bool isAdmin = false;
        public string roomCode;
        public int bootAmount;

        private const string GameID = "tambola-casino-l4njtxhlkuutn8ycnne52w";
        private const string password = "12345678";

        private const string roomType = "TambolaCasino";

        private List<Message> msgList = new List<Message>(); //  Messsage queue implementation    

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (player == null)
                Authentication();
        }

        void handlemessage(object sender, Message m)
        {
            msgList.Add(m);
        }

        void FixedUpdate()
        {
            // process message queue
            foreach (Message m in msgList)
            {
                switch (m.Type)
                {
                    case "JoinPlayer":
                        string playerId = m.GetString(0);
                        string playerName = m.GetString(1);
                        string playerImage = m.GetString(2);

                        if (!joinedPlayer.Exists(x => x.playerID == playerId))
                        {
                            PlayerData jPlayer = new PlayerData();
                            jPlayer.playerID = playerId;
                            jPlayer.playerName = playerName;
                            jPlayer.playerImage = playerImage;
                            joinedPlayer.Add(jPlayer);

                            if (PlayerWaiting.Instance != null)
                            {
                                PlayerWaiting.Instance.GeneratePlayer();
                            }
                        }
                        break;

                    case "PlayerLeft":
                        // remove characters from the scene when they leave                    
                        string playerId1 = m.GetString(0).Remove(0, 6);
                        //Debug.Log("player left : " + joinedPlayer.Count + "\t : " + playerId1);
                        if (joinedPlayer.Exists(x => x.playerID == playerId1))
                        {
                            joinedPlayer.Remove(joinedPlayer.Find(x => x.playerID == playerId1));

                            if (PlayerWaiting.Instance != null)
                            {
                                PlayerWaiting.Instance.RemovePlayer(playerId1);
                            }
                        }
                        break;

                    case "DisconectPlayer":
                        // remove characters from the scene when they leave                    
                        string playerId2 = m.GetString(0).Remove(0, 6);
                        //Debug.Log("DisconectPlayer" + joinedPlayer.Count + "\t : " + playerId2);
                        if (joinedPlayer.Exists(x => x.playerID == playerId2))
                        {
                            joinedPlayer.Remove(joinedPlayer.Find(x => x.playerID == playerId2));

                            if (PlayerWaiting.Instance != null)
                            {
                                PlayerWaiting.Instance.RemovePlayer(playerId2);
                            }
                        }
                        break;

                    case "GameStart":
                        StaticData.ballTime = m.GetInt(1);
                        LoadScene("Game", DoNotDestroy.Instance.loadingPanel, 1);
                        break;

                    case "Timer":
                        int time = m.GetInt(0);
                        //Debug.Log("Timer : " + time);
                        break;

                    case "Chat":
                        string playerId3 = m.GetString(0);
                        string playerName3 = m.GetString(1);
                        string message = m.GetString(2);
                        //if (GameScreen.Instance != null && PlayerInfo.Instance.userID != playerId3) {
                        //GameScreen.Instance.ShowPlayerMessage(playerId, playerName, message);
                        //}
                        break;

                    case "NotAllowToJoin":
                        string playerId4 = m.GetString(0);
                        //Debug.Log("===== NotAllowToJoin : " + playerId4);
                        if (playerId4 == player.ConnectUserId)
                        {
                            bool playPrivate = PlayerInfo.Instance.playType == PlayType.Party ? true : false;
                            if (playPrivate)
                            {
                                joinedPlayer.Clear();
                                piocon.Disconnect();
                                HomeScreen.Instance.playerWaiting.SetActive(false);
                                ShowMessageBox("Room already started, please create OR join another room");
                            }
                            else
                                CreateRoom();
                        }
                        break;

                    case "TrailOrSet":
                        string playerId5 = m.GetString(0);
                        gameScreen.CompleteClaim(Targets.TrailOrSet, playerId5);
                        break;

                    case "PureSequence":
                        string playerId6 = m.GetString(0);
                        gameScreen.CompleteClaim(Targets.PureSequence, playerId6);
                        break;

                    case "Sequence":
                        string playerId7 = m.GetString(0);
                        gameScreen.CompleteClaim(Targets.Sequence, playerId7);
                        break;

                    case "Color":
                        string playerId8 = m.GetString(0);
                        gameScreen.CompleteClaim(Targets.Color, playerId8);
                        break;

                    case "Pair":
                        string playerId9 = m.GetString(0);
                        gameScreen.CompleteClaim(Targets.Pair, playerId9);
                        break;

                    case "HighCard":
                        string playerId10 = m.GetString(0);
                        gameScreen.CompleteClaim(Targets.HighCard, playerId10);
                        break;

                    case "Card":
                        string card = m.GetString(0);
                        gameScreen.GenerateCard(card);
                        break;
                }
            }
            // clear message queue after it's been processed
            msgList.Clear();
        }

        // Create new room
        public void CreateNewRoom(string roomName)
        {
            //Debug.Log("CreateNewRoom");
            if (player == null)
            {
                ShowMessageBox("Unable to connect with server, please try again");
                Authentication();
                return;
            }

            player.Multiplayer.CreateJoinRoom(
                roomName,             //Room id. If set to null a random roomid is used
                roomType,                                   //The room type started on the server
                true,                                         //Should the room be visible in the lobby?
                new Dictionary<string, string> {
                { "maxPlayers", "45" },
                { "currentPlayers", "0" }
                },
                new Dictionary<string, string> {
                { "DeviceId" , SystemInfo.deviceUniqueIdentifier }
                },
                delegate (Connection connection)
                {
                //Debug.Log("room created successfull...");
                piocon = connection;
                    piocon.OnMessage += handlemessage;
                    isAdmin = true;
                    PlayerInfo player = PlayerInfo.Instance;
                    roomCode = roomName;
                    DoNotDestroy.Instance.loadingPanel.SetActive(false);
                    HomeScreen.Instance.createOrJoin.SetActive(false);
                    HomeScreen.Instance.playerWaiting.SetActive(true);
                    SendJoinPlayer(player.userID, player.userName, player.userImage);
                },
                delegate (PlayerIOError error)
                {
                    Debug.Log("Error CreateOrJoin Room: " + error.Message);
                    ShowMessageBox(error.Message);
                }
            );
        }

        // Joining room
        public void JoinRoom(string roomID)
        {
            if (player == null)
            {
                ShowMessageBox("Unable to connect with server, please try again");
                Authentication();
                return;
            }

            player.Multiplayer.JoinRoom(
                roomID,
                new Dictionary<string, string> {
                { "DeviceId" , SystemInfo.deviceUniqueIdentifier },
                { "maxPlayers", "45" },
                { "currentPlayers", "0" }
                },
                delegate (Connection connection)
                {
                //Debug.Log("room joined successfull...");
                piocon = connection;
                    piocon.OnMessage += handlemessage;
                    isAdmin = false;
                    PlayerInfo player = PlayerInfo.Instance;
                    roomCode = roomID;
                    DoNotDestroy.Instance.loadingPanel.SetActive(false);
                    HomeScreen.Instance.createOrJoin.SetActive(false);
                    HomeScreen.Instance.playerWaiting.SetActive(true);
                    SendJoinPlayer(player.userID, player.userName, player.userImage);
                },
                delegate (PlayerIOError error)
                {
                    Debug.Log("Error Joining Room: " + error.ToString());
                    ShowMessageBox(error.Message);
                }
            );
        }

        public void CreateRoom()
        {
            PollRoomList();
        }

        // allready created room list function
        private void PollRoomList()
        {
            //Debug.Log("PollRoomList");
            if (player != null)
            {
                player.Multiplayer.ListRooms(roomType,
                    new Dictionary<string, string>
                    {
                    }
                    , 20, 0, OnRoomList, delegate (PlayerIOError error)
                    {
                        Debug.Log("Error PollRoomList : " + error.ToString());
                        ShowMessageBox(error.Message);
                    });
            }
            else
            {
                ShowMessageBox("Unable to connect with server, please try again");
                Authentication();
            }
        }

        // room information
        void OnRoomList(RoomInfo[] rooms)
        {
            int i = 0;
            //Debug.Log("Room count : " + rooms.Length.ToString());
            bool playPrivate = PlayerInfo.Instance.playType == PlayType.Party ? true : false;
            if (rooms.Length == 0)
            {
                CreateNewRoom(GenerateRoomId(10));
            }
            else
            {
                JoinRoom(rooms[i].Id);
            }
        }

        public void CreateNewUser(string name)
        {
            PlayerIO.Authenticate(
                GameID,                                 //Your game id
                "public",                               //Your SimpleUsers connection id
                new Dictionary<string, string> {        //Authentication arguments
                {"register", "true"},               //Register a user
                {"username", name},                 //Username - required
                {"password", password }
                },
                null,                                   //PlayerInsight segments
                delegate (Client client)
                {
                //Success!
                //Debug.Log("user registered success");
                player = client;
                },
                delegate (PlayerIOError error)
                {
                //Error registering.
                //Check error.Message to find out in what way it failed,
                //if any registration data was missing or invalid, etc.
                Debug.Log("ERROR_NewUser Reg. : " + error.Message);
                //MainScreen.Instance.ShowToastMsg(error.Message, Color.red);
            }
            );
        }

        public void Authentication(string deviceID = null)
        {
            deviceID = SystemInfo.deviceUniqueIdentifier;
            PlayerIO.Authenticate(
                GameID,                                 //Your game id
                "public",                               //Your SimpleUsers connection id
                new Dictionary<string, string> {        //Authentication arguments
                {"username", deviceID},             //Username - either this or email, or both                
                {"password", password}              //Password - required
                },
                null,                                   //PlayerInsight segments
                delegate (Client client)
                {
                //Success!
                //Debug.Log("authentication success : " + client.ConnectUserId);
                player = client;
                },
                delegate (PlayerIOError error)
                {
                //Error authenticating.
                Debug.Log("ERROR_Authentication : " + error.Message);
                    if (error.Message.StartsWith("UnknownUser"))
                    {
                        CreateNewUser(deviceID);
                    }
                }
            );
        }

        private void SendJoinPlayer(string playerId, string playerName, string playerImage)
        {
            if (piocon != null && piocon.Connected)
                piocon.Send("JoinPlayer", playerId, playerName, playerImage);
        }

        public void SendDisconnect()
        {
            if (piocon != null && piocon.Connected)
            {
                piocon.Send("DisconectPlayer");
                piocon.Disconnect();
            }
        }

        public void SendStartParty(string playerId)
        {
            if (piocon != null && piocon.Connected)
            {
                piocon.Send("StartParty", playerId);
            }
        }

        public void SendWinner(string playerId)
        {
            if (piocon != null && piocon.Connected)
            {
                piocon.Send("Winner", playerId);
            }
        }

        public void LeaveRoom()
        {
            if (piocon != null && piocon.Connected)
            {
                piocon.Disconnect();
            }
        }

        public void SendPlayerMessage(string playerId, string playerName, string message)
        {
            if (piocon != null && piocon.Connected)
            {
                piocon.Send("Chat", playerId, playerName, message);
            }
        }

        public void SendTrailOrSet(string playerId)
        {
            if (piocon != null && piocon.Connected)
            {
                piocon.Send("TrailOrSet", playerId);
            }
        }

        public void SendPureSequence(string playerId)
        {
            if (piocon != null && piocon.Connected)
            {
                piocon.Send("PureSequence", playerId);
            }
        }

        public void SendSequence(string playerId)
        {
            if (piocon != null && piocon.Connected)
            {
                piocon.Send("Sequence", playerId);
            }
        }

        public void SendColor(string playerId)
        {
            if (piocon != null && piocon.Connected)
            {
                piocon.Send("Color", playerId);
            }
        }

        public void SendPair(string playerId)
        {
            if (piocon != null && piocon.Connected)
            {
                piocon.Send("Pair", playerId);
            }
        }

        public void SendHighCard(string playerId)
        {
            if (piocon != null && piocon.Connected)
            {
                piocon.Send("HighCard", playerId);
            }
        }
    }

    public class PlayerData
    {
        public string playerID { get; set; }
        public string playerName { get; set; }
        public string playerImage { get; set; }
        public List<string> TrailOrSet { get; set; }
        public List<string> PureSequence { get; set; }
        public List<string> Sequence { get; set; }
        public List<string> Color { get; set; }
        public List<string> Pair { get; set; }
        public List<string> HighCard { get; set; }
    }
}
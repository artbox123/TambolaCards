using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace ArtboxTambolaCasino
{
    public class Player : BasePlayer
    {

    }

    public class PlayerData
    {
        public string playerID { get; set; }
        public string playerName { get; set; }
        public string playerImage { get; set; }
    }

    [RoomType("TambolaCasino")]
    public class GameCode : Game<Player>
    {
        private List<PlayerData> joinedPlayer = new List<PlayerData>();
        private bool isRoomFull = false;
        private Timer waitTimer;
        private Timer timer;
        private List<string> outedCards = new List<string>();

        private bool trailOrSet;
        private bool pureSequence;
        private bool sequence;
        private bool color;
        private bool pair;
        private bool highCard;

        private List<string> cardList = new List<string> { "Spade_1", "Spade_2", "Spade_3", "Spade_4", "Spade_5", "Spade_6", "Spade_7", "Spade_8", "Spade_9", "Spade_10", "Spade_11", "Spade_12", "Spade_13"
    ,"Club_1", "Club_2", "Club_3", "Club_4", "Club_5", "Club_6", "Club_7", "Club_8", "Club_9", "Club_10", "Club_11", "Club_12", "Club_13"
    ,"Heart_1", "Heart_2", "Heart_3", "Heart_4", "Heart_5", "Heart_6", "Heart_7", "Heart_8", "Heart_9", "Heart_10", "Heart_11", "Heart_12", "Heart_13"
    ,"Diamond_1", "Diamond_2", "Diamond_3", "Diamond_4", "Diamond_5", "Diamond_6", "Diamond_7", "Diamond_8", "Diamond_9", "Diamond_10", "Diamond_11", "Diamond_12", "Diamond_13"};

        // This method is called when an instance of your the game is created
        public override void GameStarted() {
            // anything you write to the Console will show up in the 
            // output window of the development server
            Console.WriteLine("Game is started: " + RoomId);
        }

        // This method is called when the last player leaves the room, and it's closed down.
        public override void GameClosed() {
            Console.WriteLine("RoomId: " + RoomId);
        }

        // This method is called whenever a player joins the game
        public override void UserJoined(Player player) {
            foreach (Player pl in Players) {
                if (pl.ConnectUserId != player.ConnectUserId) {

                }
            }
        }

        // This method is called when a player leaves the game
        public override void UserLeft(Player player) {
            Broadcast("PlayerLeft", player.ConnectUserId);
            string playerid = player.ConnectUserId;
            playerid = playerid.Remove(0, 6);
            if (joinedPlayer.Exists(x => x.playerID == playerid)) {
                joinedPlayer.Remove(joinedPlayer.Find(x => x.playerID == playerid));
            }
        }

        //This method is called before a user joins a room.
        //If you return false, the user is not allowed to join.
        public override bool AllowUserJoin(Player player) {
            if (PlayerCount >= 5) {
                Visible = false;
                isRoomFull = true;
            }

            if (!isRoomFull)
                return true; // allow joining
            else {
                player.Send("NotAllowToJoin", player.ConnectUserId);
                return false;
            }
        }

        // This method is called when a player sends a message into the server code
        public override void GotMessage(Player player, Message message) {
            switch (message.Type) {
                // called when a player clicks on the ground
                case "JoinPlayer":
                    string playerId = message.GetString(0);
                    string playerName = message.GetString(1);
                    string playerImage = message.GetString(2);

                    if (!joinedPlayer.Exists(x => x.playerID == playerId)) {
                        PlayerData jPlayer = new PlayerData();
                        jPlayer.playerID = playerId;
                        jPlayer.playerName = playerName;
                        jPlayer.playerImage = playerImage;
                        joinedPlayer.Add(jPlayer);

                        foreach (PlayerData pData in joinedPlayer) {
                            Broadcast("JoinPlayer", pData.playerID, pData.playerName, pData.playerImage);
                        }
                        if (joinedPlayer.Count >= 5) {
                            Visible = false;
                            isRoomFull = true;
                        }
                    }
                    break;

                case "StartParty":
                    Visible = false;
                    isRoomFull = true;
                    string playerId2 = message.GetString(0);
                    Broadcast("GameStart", true, 5);
                    WaitBeforeStart(2000);
                    break;

                case "DisconectPlayer":
                    Broadcast("DisconectPlayer", player.ConnectUserId);
                    string playerid = player.ConnectUserId;
                    playerid = playerid.Remove(0, 6);
                    if (joinedPlayer.Exists(x => x.playerID == playerid)) {
                        joinedPlayer.Remove(joinedPlayer.Find(x => x.playerID == playerid));
                    }
                    break;

                case "TrailOrSet":
                    if (!trailOrSet) {
                        string playerId1 = message.GetString(0);
                        trailOrSet = true;
                        Broadcast("TrailOrSet", playerId1);
                        CheckClaim();
                    }
                    break;

                case "PureSequence":
                    if (!pureSequence) {
                        string playerId1 = message.GetString(0);
                        pureSequence = true;
                        Broadcast("PureSequence", playerId1);
                        CheckClaim();
                    }
                    break;

                case "Sequence":
                    if (!sequence) {
                        string playerId1 = message.GetString(0);
                        sequence = true;
                        Broadcast("Sequence", playerId1);
                        CheckClaim();
                    }
                    break;

                case "Color":
                    if (!color) {
                        string playerId1 = message.GetString(0);
                        color = true;
                        Broadcast("Color", playerId1);
                        CheckClaim();
                    }
                    break;

                case "Pair":
                    if (!pair) {
                        string playerId1 = message.GetString(0);
                        pair = true;
                        Broadcast("Pair", playerId1);
                        CheckClaim();
                    }
                    break;

                case "HighCard":
                    if (!highCard) {
                        string playerId1 = message.GetString(0);
                        highCard = true;
                        Broadcast("HighCard", playerId1);
                        CheckClaim();
                    }
                    break;

                case "Chat":

                    break;
            }
        }

        private void WaitBeforeStart(int interval = 5000) {
            waitTimer = AddTimer(delegate {
                GiveCard();
                waitTimer.Stop();
            },
            interval);
        }

        private void GiveCard() {
            Random random = new Random();
            //Console.WriteLine("Game is started");
            int time = 52;
            timer = AddTimer(delegate {
                time--;
                string card = cardList[random.Next(0, cardList.Count)];
                if (outedCards.Contains(card)) {
                    card = cardList[random.Next(0, cardList.Count)];
                }
                outedCards.Add(card);
                cardList.Remove(card);
                Broadcast("Card", card);
                CheckClaim();
                if (time == 0) {
                    timer.Stop();
                }
            },
            6000);
        }

        private void CheckClaim() {
            if (trailOrSet && pureSequence && sequence && color && pair && highCard) {
                timer.Stop();
            }
        }
    }
}
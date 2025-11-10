using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ArtboxGames
{
    public class GameScreen : GameTargets
    {
        private List<string> outedCards = new List<string>();
        private List<string> selectedCards = new List<string>();
        private List<string> finalCardList = new List<string>();
        private List<string> botPlayers = new List<string>();
        public List<GameUser> gameUsers = new List<GameUser>();

        [SerializeField] private GameObject cardBoard;
        [SerializeField] private GameObject claimBoard;
        [SerializeField] private Transform[] positions;
        [SerializeField] private TextMeshProUGUI[] sideNumbers;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private GameObject hand;
        [SerializeField] private ClaimWinner claimWinner;
        [SerializeField] private TextMeshProUGUI claims;

        [SerializeField] private Transform usersContent;
        [SerializeField] private GameUser userPrefab;

        [Header("Message Box")]
        [SerializeField] private GameObject m_loadingPanel;
        [SerializeField] private GameObject m_messageBox;
        [SerializeField] private TextMeshProUGUI m_message;

        [Header("Popups")]
        [SerializeField] private GameObject waitingScreen;
        [SerializeField] private GameObject winGame;
        [SerializeField] private GameObject settings;
        [SerializeField] private GameObject quitGame;

        [Header("Targets")]
        public Target trailOrSet;
        public Target pureSequence;
        public Target sequence;
        public Target color;
        public Target pair;
        public Target highCard;

        private bool isTrailOrSet;
        private bool isPureSequence;
        private bool isSequence;
        private bool isColor;
        private bool isPair;
        private bool isHighCard;

        private bool isFirstGame;
        private bool isGameover;

        private int _claimCounter;
        private int claimCounter
        {
            get
            {
                return _claimCounter;
            }
            set
            {
                _claimCounter = value;
                claims.text = "CLAIM " + value.ToString("00");
            }
        }

        private List<string> cardList = new List<string> { "Spade_1", "Spade_2", "Spade_3", "Spade_4", "Spade_5", "Spade_6", "Spade_7", "Spade_8", "Spade_9", "Spade_10", "Spade_11", "Spade_12", "Spade_13"
    ,"Club_1", "Club_2", "Club_3", "Club_4", "Club_5", "Club_6", "Club_7", "Club_8", "Club_9", "Club_10", "Club_11", "Club_12", "Club_13"
    ,"Heart_1", "Heart_2", "Heart_3", "Heart_4", "Heart_5", "Heart_6", "Heart_7", "Heart_8", "Heart_9", "Heart_10", "Heart_11", "Heart_12", "Heart_13"
    ,"Diamond_1", "Diamond_2", "Diamond_3", "Diamond_4", "Diamond_5", "Diamond_6", "Diamond_7", "Diamond_8", "Diamond_9", "Diamond_10", "Diamond_11", "Diamond_12", "Diamond_13"};

        private void Awake()
        {
            InitializeMessageBox(m_loadingPanel, m_messageBox, m_message);
            waitingScreen.SetActive(true);
            ServerCode.Instance.gameScreen = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            SetScreenRes();
            SetAllTargets();
            claimCounter = 6;
            if (PlayerInfo.Instance.totalPlayed == 0)
                isFirstGame = true;
            Actions action = Actions.Party;
            if (PlayerInfo.Instance.playType == PlayType.Play)
            {
                StartCoroutine(GenerateNumberBall(52));
                action = Actions.Play;
            }
            PlayerInfo.Instance.UpdateCoins(CoinAction.Minuse, ServerCode.Instance.bootAmount);
            AddCoinHistory(action, ServerCode.Instance.bootAmount, TransactionType.Debit);
            PlayerInfo.Instance.totalPlayed++;
            //InvokeRepeating("CheckNetwork", 5f, 7f);
            PlaySound(3);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (m_loadingPanel.activeSelf)
                    return;

                if (waitingScreen.activeSelf)
                {
                }
                else if (settings.activeSelf)
                {
                    settings.SetActive(false);
                }
                else if (winGame.activeSelf)
                {
                    LoadScene("Home", m_loadingPanel);
                    DisconnectPlayer();
                }
                else if (quitGame.activeSelf)
                {
                    quitGame.SetActive(false);
                }
                else
                {
                    quitGame.SetActive(true);
                }
            }
        }

        private void WinAlgorithm(PlayerData data)
        {
            PlayerInfo info = PlayerInfo.Instance;
            if (info.playType == PlayType.Party || info.totalPlayed > 0)
            {
                finalCardList = cardList.OrderBy(x => Random.value).ToList();
                return;
            }
            if (info.totalPlayed == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    string card1 = data.TrailOrSet[i];
                    if (!finalCardList.Contains(card1))
                    {
                        finalCardList.Add(card1);
                    }

                    string card2 = data.PureSequence[i];
                    if (!finalCardList.Contains(card2))
                    {
                        finalCardList.Add(card2);
                    }

                    string card3 = data.Sequence[i];
                    if (!finalCardList.Contains(card3))
                    {
                        finalCardList.Add(card3);
                    }

                    string card4 = data.Color[i];
                    if (!finalCardList.Contains(card4))
                    {
                        finalCardList.Add(card4);
                    }

                    string card5 = data.Pair[i];
                    if (!finalCardList.Contains(card5))
                    {
                        finalCardList.Add(card5);
                    }

                    string card6 = data.HighCard[i];
                    if (!finalCardList.Contains(card6))
                    {
                        finalCardList.Add(card6);
                    }
                }
                for (int j = 0; j < cardList.Count; j++)
                {
                    string card = cardList[j];
                    if (!finalCardList.Contains(card))
                        finalCardList.Add(card);
                }
            }
        }

        // generate number ball one by one with given time
        private IEnumerator GenerateNumberBall(int numbers)
        {
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < numbers; i++)
            {
                string card = finalCardList[0];
                finalCardList.Remove(card);
                GenerateCard(card);
                yield return new WaitForSeconds(StaticData.ballTime);
            }
            StartCoroutine(ShowWinGame(2f));
        }

        // set board number color
        private void SetNumberBoard(string card)
        {
            sideNumbers.ToList().Find(x => x.name == card).color = NumberColor(card);
        }

        // generate number ball
        public void GenerateCard(string card)
        {
            if (isGameover)
                return;
            waitingScreen.SetActive(false);
            MoveCardPosition();
            GameObject cardObj = Instantiate(cardPrefab) as GameObject;
            cardObj.transform.SetParent(positions[0], false);
            cardObj.GetComponent<Image>().sprite = CardImage(card);
            string cardNum = CardNumber(card, true);
            string cardColor = card;
            PlayVoice(int.Parse(cardNum), cardColor);
            cardObj.transform.Find("CardNumber").GetComponent<TextMeshProUGUI>().text = CardNumber(card);
            cardObj.transform.Find("FillAmount").GetComponent<Image>().color = NumberColor(card);

            if (!outedCards.Contains(card))
            {
                outedCards.Add(card);
            }
            if (PlayerInfo.Instance.playType == PlayType.Play)
                Invoke("CheckBotPlayerClaim", 3f);

            SetNumberBoard(card);
            SetHand(cardBoard.transform.Find(card).transform.position, true);

            if (outedCards.Count == 52)
            {
                StartCoroutine(ShowWinGame(StaticData.ballTime + 2f));
            }
        }

        // move ball position when new ball is spawn
        private void MoveCardPosition()
        {
            if (positions[4].childCount > 0)
            {
                for (int i = 0; i < positions[4].childCount; i++)
                {
                    positions[4].GetChild(i).GetComponent<Animation>().Play("ball_destroy");
                    Destroy(positions[4].GetChild(i).gameObject, 0.4f);
                }
            }
            if (positions[3].childCount > 0)
            {
                StartCoroutine(MoveBall(positions[3].GetChild(0).gameObject, positions[3].position, positions[4].position, positions[4], 0.4f));
            }
            if (positions[2].childCount > 0)
            {
                StartCoroutine(MoveBall(positions[2].GetChild(0).gameObject, positions[2].position, positions[3].position, positions[3], 0.4f));
            }
            if (positions[1].childCount > 0)
            {
                StartCoroutine(MoveBall(positions[1].GetChild(0).gameObject, positions[1].position, positions[2].position, positions[2], 0.4f));
            }
            if (positions[0].childCount > 0)
            {
                StartCoroutine(MoveBall(positions[0].GetChild(0).gameObject, positions[0].position, positions[1].position, positions[1], 0.4f));
            }
        }

        // smoothly move ball from one position to second
        private IEnumerator MoveBall(GameObject obj, Vector3 startingPos, Vector3 newPosition, Transform parent, float time)
        {
            obj.transform.SetParent(parent, false);
            obj.transform.position = Vector3.zero;

            float elapsedTime = 0;
            while (elapsedTime < time)
            {
                obj.transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        // set screen ui with screen current resolution
        private void SetScreenRes()
        {
            float ratio = (float)Screen.height / (float)Screen.width;
            //Debug.Log("=== ratio : " + ratio);
            if (ratio >= 0.65f)
            {
                GetComponent<CanvasScaler>().matchWidthOrHeight = 0.15f;
            }
            else if (ratio <= 0.5f)
            {
                GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
            }
        }

        //set all tagets (set or trail, pure sequence, sequence....)
        private void SetAllTargets()
        {
            PlayerData pdata;
            if (PlayerInfo.Instance.playType == PlayType.Play)
            {
                PlayerData data = new PlayerData();
                data.playerID = PlayerInfo.Instance.userID;
                data.playerName = PlayerInfo.Instance.userName;
                data.playerImage = PlayerInfo.Instance.userImage;

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

                SetTargetData(data);

                ServerCode.Instance.joinedPlayer.Add(data);
                StartCoroutine(SetBotPlayerData());
                pdata = data;
            }
            else
            {
                PlayerData data = ServerCode.Instance.joinedPlayer.Find(x => x.playerID == PlayerInfo.Instance.userID);
                SetTargetData(data);
                StartCoroutine(GenerateUsers());
                pdata = data;
            }

            WinAlgorithm(pdata);
        }

        private void SetTargetData(PlayerData pData)
        {
            int winAmount = (GetLevel() * 1000);
            SetTarget(pData.TrailOrSet, Targets.TrailOrSet, (winAmount + 7000));
            SetTarget(pData.PureSequence, Targets.PureSequence, (winAmount + 6000));
            SetTarget(pData.Sequence, Targets.Sequence, (winAmount + 4000));
            SetTarget(pData.Color, Targets.Color, (winAmount + 3000));
            SetTarget(pData.Pair, Targets.Pair, (winAmount + 1000));
            SetTarget(pData.HighCard, Targets.HighCard, (winAmount * 1));
        }

        private IEnumerator SetBotPlayerData()
        {
            for (int i = 1; i < 5; i++)
            {
                PlayerData data = new PlayerData();
                data.playerID = "bot_" + BotName();
                data.playerName = BotName();
                data.playerImage = Random.Range(0, 9).ToString();

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

                ServerCode.Instance.joinedPlayer.Add(data);
                yield return new WaitForEndOfFrame();
            }
            StartCoroutine(GenerateUsers());
        }

        private string BotName()
        {
            string botName = "Vaidik";
            botName = PlayerInfo.Instance.playerName[Random.Range(0, PlayerInfo.Instance.playerName.Count)];
            while (botPlayers.Contains(botName))
            {
                botName = PlayerInfo.Instance.playerName[Random.Range(0, PlayerInfo.Instance.playerName.Count)];
            }
            botPlayers.Add(botName);
            return botName;
        }

        private IEnumerator GenerateUsers()
        {
            foreach (PlayerData data in ServerCode.Instance.joinedPlayer)
            {
                GameUser user = Instantiate(userPrefab, usersContent, false);
                user.playerId = data.playerID;
                user.playerName = data.playerName;
                user.playerImage = data.playerImage;
                user.claim = 0;
                gameUsers.Add(user);
                yield return new WaitForEndOfFrame();
            }
        }

        //it will return comma separated string
        private string ConvertArrayToStringJoiner(string[] array)
        {
            // Use string Join to concatenate the string elements.
            string result = string.Join(",", array);
            return result;
        }

        private void SetTarget(List<string> cards, Targets target, int winAmount)
        {
            Transform claim = claimBoard.transform.Find(target.ToString());
            claim.GetComponent<Target>().cards = cards;
            claim.GetComponent<Target>().winAmount = winAmount;
            claim.transform.Find("WinAmount").GetComponent<TextMeshProUGUI>().text = winAmount.ToString();
            for (int i = 1; i <= 3; i++)
            {
                Transform card = claim.transform.Find("Card" + i);
                card.GetComponent<Image>().sprite = CardImage(cards[i - 1]);
                card.GetComponentInChildren<TextMeshProUGUI>().text = CardNumber(cards[i - 1]);
            }
        }

        public void Claim(Target target)
        {
            target.GetComponent<Animation>().Play();
            if (target.cards.Except(selectedCards).Any())
                return;

            SetHand(Vector2.zero);
            if (PlayerInfo.Instance.playType == PlayType.Play)
                CompleteClaim(target.target, PlayerInfo.Instance.userID);
            else
            {
                target.loader.SetActive(true);
                if (target.target == Targets.TrailOrSet)
                    ServerCode.Instance.SendTrailOrSet(PlayerInfo.Instance.userID);
                else if (target.target == Targets.PureSequence)
                    ServerCode.Instance.SendPureSequence(PlayerInfo.Instance.userID);
                else if (target.target == Targets.Sequence)
                    ServerCode.Instance.SendSequence(PlayerInfo.Instance.userID);
                else if (target.target == Targets.Color)
                    ServerCode.Instance.SendColor(PlayerInfo.Instance.userID);
                else if (target.target == Targets.Pair)
                    ServerCode.Instance.SendPair(PlayerInfo.Instance.userID);
                else if (target.target == Targets.HighCard)
                    ServerCode.Instance.SendHighCard(PlayerInfo.Instance.userID);
            }
        }

        public void CompleteClaim(Targets targetName, string playerId)
        {
            Target target = trailOrSet;
            if (targetName == Targets.TrailOrSet)
                target = trailOrSet;
            else if (targetName == Targets.PureSequence)
                target = pureSequence;
            else if (targetName == Targets.Sequence)
                target = sequence;
            else if (targetName == Targets.Color)
                target = color;
            else if (targetName == Targets.Pair)
                target = pair;
            else if (targetName == Targets.HighCard)
                target = highCard;

            GameUser myData = gameUsers.Find(x => x.playerId == playerId);
            target.claimDone.SetActive(true);
            target.loader.SetActive(false);
            target.GetComponent<Button>().enabled = false;
            target.GetComponent<Animation>().enabled = false;
            target.transform.localPosition = new Vector2(target.transform.localPosition.x, 0f);

            if (target.target == Targets.TrailOrSet && !isTrailOrSet)
            {
                isTrailOrSet = true;
                myData.claim++;
                ShowClaimUser(PlayerInfo.Instance.userID, "trailOrSet", trailOrSet.cards);

                if (playerId == PlayerInfo.Instance.userID)
                {
                    target.isClaimed = true;
                    target.transform.Find("Particle").gameObject.SetActive(true);
                }
            }
            else if (target.target == Targets.PureSequence && !isPureSequence)
            {
                isPureSequence = true;
                myData.claim++;
                ShowClaimUser(PlayerInfo.Instance.userID, "pureSequence", pureSequence.cards);

                if (playerId == PlayerInfo.Instance.userID)
                {
                    target.isClaimed = true;
                    target.transform.Find("Particle").gameObject.SetActive(true);
                }
            }
            else if (target.target == Targets.Sequence && !isSequence)
            {
                isSequence = true;
                myData.claim++;
                ShowClaimUser(PlayerInfo.Instance.userID, "sequence", sequence.cards);

                if (playerId == PlayerInfo.Instance.userID)
                {
                    target.isClaimed = true;
                    target.transform.Find("Particle").gameObject.SetActive(true);
                }
            }
            else if (target.target == Targets.Color && !isColor)
            {
                isColor = true;
                myData.claim++;
                ShowClaimUser(PlayerInfo.Instance.userID, "color", color.cards);

                if (playerId == PlayerInfo.Instance.userID)
                {
                    target.isClaimed = true;
                    target.transform.Find("Particle").gameObject.SetActive(true);
                }
            }
            else if (target.target == Targets.Pair && !isPair)
            {
                isPair = true;
                myData.claim++;
                ShowClaimUser(PlayerInfo.Instance.userID, "pair", pair.cards);

                if (playerId == PlayerInfo.Instance.userID)
                {
                    target.isClaimed = true;
                    target.transform.Find("Particle").gameObject.SetActive(true);
                }
            }
            else if (target.target == Targets.HighCard && !isHighCard)
            {
                isHighCard = true;
                myData.claim++;
                ShowClaimUser(PlayerInfo.Instance.userID, "highCard", highCard.cards);

                if (playerId == PlayerInfo.Instance.userID)
                {
                    target.isClaimed = true;
                    target.transform.Find("Particle").gameObject.SetActive(true);
                }
            }
            Vibration.VibratePop();
            SetHand(Vector2.zero);
            SetClaimWinner(myData, target.target.ToString());
        }

        private void IsAllClaimDone()
        {
            if (isTrailOrSet && isPureSequence && isSequence && isColor && isPair && isHighCard)
            {
                StartCoroutine(ShowWinGame(2f));
                isGameover = true;
            }
        }

        public void SelectCard()
        {
            SelectCard card = EventSystem.current.currentSelectedGameObject.GetComponent<SelectCard>();
            card.GetComponent<Animation>().Play();
            if (card.Selected.activeSelf)
            {
                if (selectedCards.Contains(card.name))
                {
                    selectedCards.Remove(card.name);
                }
                card.Selected.SetActive(false);
            }
            else
            {
                if (!selectedCards.Contains(card.name) && outedCards.Contains(card.name))
                {
                    selectedCards.Add(card.name);
                    card.Selected.SetActive(true);
                    PlaySound();
                    Invoke("CheckClaimForAnimation", 1f);
                    SetHand(Vector2.zero);
                }
            }
        }

        private IEnumerator ShowWinGame(float wait)
        {
            StopCoroutine(GenerateNumberBall(52));
            yield return new WaitForSeconds(wait);
            winGame.SetActive(true);
        }

        private void CheckBotPlayerClaim()
        {
            foreach (PlayerData player in ServerCode.Instance.joinedPlayer)
            {
                if (player.playerID.StartsWith("bot_"))
                {
                    if (!player.TrailOrSet.Except(outedCards).Any() && !isTrailOrSet)
                    {
                        GameUser user = gameUsers.Find(x => x.playerId == player.playerID);
                        trailOrSet.claimDone.SetActive(true);
                        trailOrSet.GetComponent<Button>().enabled = false;
                        trailOrSet.GetComponent<Animation>().enabled = false;
                        trailOrSet.transform.localPosition = new Vector2(trailOrSet.transform.localPosition.x, 0f);
                        user.claim++;
                        isTrailOrSet = true;
                        ShowClaimUser(player.playerName, "trailOrSet", player.TrailOrSet);
                        SetHand(Vector2.zero);
                        SetClaimWinner(user, Targets.TrailOrSet.ToString());
                        break;
                    }
                    else if (!player.PureSequence.Except(outedCards).Any() && !isPureSequence)
                    {
                        GameUser user = gameUsers.Find(x => x.playerId == player.playerID);
                        pureSequence.claimDone.SetActive(true);
                        pureSequence.GetComponent<Button>().enabled = false;
                        pureSequence.GetComponent<Animation>().enabled = false;
                        pureSequence.transform.localPosition = new Vector2(pureSequence.transform.localPosition.x, 0f);
                        user.claim++;
                        isPureSequence = true;
                        ShowClaimUser(player.playerName, "pureSequence", player.PureSequence);
                        SetHand(Vector2.zero);
                        SetClaimWinner(user, Targets.PureSequence.ToString());
                        break;
                    }
                    else if (!player.Sequence.Except(outedCards).Any() && !isSequence)
                    {
                        GameUser user = gameUsers.Find(x => x.playerId == player.playerID);
                        sequence.claimDone.SetActive(true);
                        sequence.GetComponent<Button>().enabled = false;
                        sequence.GetComponent<Animation>().enabled = false;
                        sequence.transform.localPosition = new Vector2(sequence.transform.localPosition.x, 0f);
                        user.claim++;
                        isSequence = true;
                        ShowClaimUser(player.playerName, "sequence", player.Sequence);
                        SetHand(Vector2.zero);
                        SetClaimWinner(user, Targets.Sequence.ToString());
                        break;
                    }
                    else if (!player.Color.Except(outedCards).Any() && !isColor)
                    {
                        GameUser user = gameUsers.Find(x => x.playerId == player.playerID);
                        color.claimDone.SetActive(true);
                        color.GetComponent<Button>().enabled = false;
                        color.GetComponent<Animation>().enabled = false;
                        color.transform.localPosition = new Vector2(color.transform.localPosition.x, 0f);
                        user.claim++;
                        isColor = true;
                        ShowClaimUser(player.playerName, "color", player.Color);
                        SetHand(Vector2.zero);
                        SetClaimWinner(user, Targets.Color.ToString());
                        break;
                    }
                    else if (!player.Pair.Except(outedCards).Any() && !isPair)
                    {
                        GameUser user = gameUsers.Find(x => x.playerId == player.playerID);
                        pair.claimDone.SetActive(true);
                        pair.GetComponent<Button>().enabled = false;
                        pair.GetComponent<Animation>().enabled = false;
                        pair.transform.localPosition = new Vector2(pair.transform.localPosition.x, 0f);
                        user.claim++;
                        isPair = true;
                        ShowClaimUser(player.playerName, "pair", player.Pair);
                        SetHand(Vector2.zero);
                        SetClaimWinner(user, Targets.Pair.ToString());
                        break;
                    }
                    else if (!player.HighCard.Except(outedCards).Any() && !isHighCard)
                    {
                        GameUser user = gameUsers.Find(x => x.playerId == player.playerID);
                        highCard.claimDone.SetActive(true);
                        highCard.GetComponent<Button>().enabled = false;
                        highCard.GetComponent<Animation>().enabled = false;
                        highCard.transform.localPosition = new Vector2(highCard.transform.localPosition.x, 0f);
                        user.claim++;
                        isHighCard = true;
                        ShowClaimUser(player.playerName, "highCard", player.HighCard);
                        SetHand(Vector2.zero);
                        SetClaimWinner(user, Targets.HighCard.ToString());
                        break;
                    }
                }
            }
        }

        private void ShowClaimUser(string playerId, string target, List<string> cards)
        {
            if (claimCounter > 0)
                claimCounter--;

            IsAllClaimDone();

            if (playerId == PlayerInfo.Instance.userID)
                PlaySound(2);
            else
                PlaySound(5);

            /*
            Debug.Log("name : " + playerName + " , target : " + target);
            for (int i = 0; i < cards.Count; i++) {
                Debug.Log("Card:" + (i + 1) + " , " + cards[i]);
            }
            Debug.Log("==============================================");
            */
        }

        private void CheckClaimForAnimation()
        {
            PlayerData myData = ServerCode.Instance.joinedPlayer.Find(x => x.playerID == PlayerInfo.Instance.userID);
            if (!myData.TrailOrSet.Except(selectedCards).Any() && !isTrailOrSet)
            {
                trailOrSet.GetComponent<Animation>().Play("trail_set_hint");
                SetHand(trailOrSet.transform.position, true);
            }
            if (!myData.PureSequence.Except(selectedCards).Any() && !isPureSequence)
            {
                pureSequence.GetComponent<Animation>().Play("pure_seq_hint");
                SetHand(pureSequence.transform.position, true);
            }
            if (!myData.Sequence.Except(selectedCards).Any() && !isSequence)
            {
                sequence.GetComponent<Animation>().Play("sequence_hint");
                SetHand(sequence.transform.position, true);
            }
            if (!myData.Color.Except(selectedCards).Any() && !isColor)
            {
                color.GetComponent<Animation>().Play("color_hint");
                SetHand(color.transform.position, true);
            }
            if (!myData.Pair.Except(selectedCards).Any() && !isPair)
            {
                pair.GetComponent<Animation>().Play("pair_hint");
                SetHand(pair.transform.position, true);
            }
            if (!myData.HighCard.Except(selectedCards).Any() && !isHighCard)
            {
                highCard.GetComponent<Animation>().Play("high_card_hint");
                SetHand(highCard.transform.position, true);
            }
        }

        private void SetHand(Vector2 pos, bool show = false)
        {
            if (!isFirstGame)
                return;
            if (show)
            {
                hand.SetActive(true);
                hand.transform.position = pos;
            }
            else
            {
                hand.SetActive(false);
            }
        }

        private void SetClaimWinner(GameUser user, string claimName)
        {
            claimWinner.gameObject.SetActive(true);
            claimWinner.PlayerImage.sprite = user.PlayerImage.sprite;
            claimWinner.PlayerName.text = user.playerName;
            claimWinner.Claim.text = claimName;
            if (user.playerId == PlayerInfo.Instance.userID)
                claimWinner.Star.SetActive(true);
            else
                claimWinner.Star.SetActive(false);

            CancelInvoke("CloseClaimWinner");
            Invoke("CloseClaimWinner", 3f);
        }

        private void CloseClaimWinner()
        {
            claimWinner.gameObject.SetActive(false);
        }

        private void CheckNetwork()
        {
            if (PlayerInfo.Instance.playType == PlayType.Play)
                return;
            StartCoroutine(CheckInternet((isConnected) =>
            {
                if (!isConnected)
                {
                    ShowMessageBox("You lost internet connection!, please try again");
                }
            }));
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

namespace ArtboxGames
{
    public class ResultDialog : GameManager
    {
        private GameScreen gameScreen;

        [Header("Targets")]
        [SerializeField] private GameObject trailOrSet;
        [SerializeField] private GameObject pureSequence;
        [SerializeField] private GameObject sequence;
        [SerializeField] private GameObject color;
        [SerializeField] private GameObject pair;
        [SerializeField] private GameObject highCard;

        [Space(5)]
        [SerializeField] private Transform usersContent;
        [SerializeField] private GameObject userPrefab;

        [Space(5)]
        [SerializeField] private TextMeshProUGUI txtWinAmount;
        [SerializeField] private TextMeshProUGUI txtTotalAmount;
        [SerializeField] private GameObject winParticle;

        // Start is called before the first frame update
        void Start()
        {
            gameScreen = FindAnyObjectByType<GameScreen>();
            SetClaimData();
            StartCoroutine(GenerateUsers());
            ShowClaims();
            CheckForAchievement();
            Vibration.VibratePop();
            PlaySound(1);
            AdsManager.Instance?.ShowInterstitial();
        }

        private void SetClaimData()
        {
            for (int i = 0; i < 3; i++)
            {
                // trailOrSet
                string card0 = gameScreen.trailOrSet.cards[i];
                trailOrSet.transform.Find("Card" + (i + 1)).GetComponent<Image>().sprite = CardImage(card0);
                trailOrSet.transform.Find("Card" + (i + 1) + "/Text").GetComponent<TextMeshProUGUI>().text = CardNumber(card0);

                // pureSequence
                string card1 = gameScreen.pureSequence.cards[i];
                pureSequence.transform.Find("Card" + (i + 1)).GetComponent<Image>().sprite = CardImage(card1);
                pureSequence.transform.Find("Card" + (i + 1) + "/Text").GetComponent<TextMeshProUGUI>().text = CardNumber(card1);

                // sequence
                string card2 = gameScreen.sequence.cards[i];
                sequence.transform.Find("Card" + (i + 1)).GetComponent<Image>().sprite = CardImage(card2);
                sequence.transform.Find("Card" + (i + 1) + "/Text").GetComponent<TextMeshProUGUI>().text = CardNumber(card2);

                // color
                string card3 = gameScreen.color.cards[i];
                color.transform.Find("Card" + (i + 1)).GetComponent<Image>().sprite = CardImage(card3);
                color.transform.Find("Card" + (i + 1) + "/Text").GetComponent<TextMeshProUGUI>().text = CardNumber(card3);

                // pair
                string card4 = gameScreen.pair.cards[i];
                pair.transform.Find("Card" + (i + 1)).GetComponent<Image>().sprite = CardImage(card4);
                pair.transform.Find("Card" + (i + 1) + "/Text").GetComponent<TextMeshProUGUI>().text = CardNumber(card4);

                // highCard
                string card5 = gameScreen.highCard.cards[i];
                highCard.transform.Find("Card" + (i + 1)).GetComponent<Image>().sprite = CardImage(card5);
                highCard.transform.Find("Card" + (i + 1) + "/Text").GetComponent<TextMeshProUGUI>().text = CardNumber(card5);
            }
        }

        private IEnumerator GenerateUsers()
        {
            foreach (GameUser gameUser in gameScreen.gameUsers)
            {
                GameObject user = Instantiate(userPrefab, usersContent, false);
                StartCoroutine(LoadImageFromPath(gameUser.playerImage, user.transform.Find("Background/ProfileImage").GetComponent<Image>()));
                user.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = gameUser.playerName;
                user.transform.Find("Claims/Text").GetComponent<TextMeshProUGUI>().text = "CLAIM " + gameUser.claim.ToString("00");
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void ShowClaims()
        {
            int winAmount = 0;

            // trailOrSet
            if (gameScreen.trailOrSet.isClaimed)
            {
                trailOrSet.transform.Find("ClaimDone/Cancel").gameObject.SetActive(false);
                trailOrSet.transform.Find("WinAmount").GetComponent<TextMeshProUGUI>().text = gameScreen.trailOrSet.winAmount.ToString();
                winAmount += gameScreen.trailOrSet.winAmount;
            }

            // pureSequence
            if (gameScreen.pureSequence.isClaimed)
            {
                pureSequence.transform.Find("ClaimDone/Cancel").gameObject.SetActive(false);
                pureSequence.transform.Find("WinAmount").GetComponent<TextMeshProUGUI>().text = gameScreen.pureSequence.winAmount.ToString();
                winAmount += gameScreen.pureSequence.winAmount;
            }

            // sequence
            if (gameScreen.sequence.isClaimed)
            {
                sequence.transform.Find("ClaimDone/Cancel").gameObject.SetActive(false);
                sequence.transform.Find("WinAmount").GetComponent<TextMeshProUGUI>().text = gameScreen.sequence.winAmount.ToString();
                winAmount += gameScreen.sequence.winAmount;
            }

            // color
            if (gameScreen.color.isClaimed)
            {
                color.transform.Find("ClaimDone/Cancel").gameObject.SetActive(false);
                color.transform.Find("WinAmount").GetComponent<TextMeshProUGUI>().text = gameScreen.color.winAmount.ToString();
                winAmount += gameScreen.color.winAmount;
            }

            // pair
            if (gameScreen.pair.isClaimed)
            {
                pair.transform.Find("ClaimDone/Cancel").gameObject.SetActive(false);
                pair.transform.Find("WinAmount").GetComponent<TextMeshProUGUI>().text = gameScreen.pair.winAmount.ToString();
                winAmount += gameScreen.pair.winAmount;
            }

            // highCard
            if (gameScreen.highCard.isClaimed)
            {
                highCard.transform.Find("ClaimDone/Cancel").gameObject.SetActive(false);
                highCard.transform.Find("WinAmount").GetComponent<TextMeshProUGUI>().text = gameScreen.highCard.winAmount.ToString();
                winAmount += gameScreen.highCard.winAmount;
            }

            txtWinAmount.text = "WIN COINS " + winAmount;
            PlayerInfo.Instance.coins += winAmount;
            txtTotalAmount.text = "TOTAL COINS " + PlayerInfo.Instance.coins;

            if (winAmount > 0)
            {
                PlayerInfo.Instance.totalWin++;
                winParticle.SetActive(true);
                AddCoinHistory(Actions.GameWin, winAmount, TransactionType.Credit);
            }
        }

        private void CheckForAchievement()
        {
            Achievements achievement3 = PlayerInfo.Instance.achievements.Find(x => x.id == "achievement3");
            if (!achievement3.status && achievement3.fillAmount < 10)
            {
                achievement3.fillAmount += 1;
                PlayerInfo.Instance.SaveAchievements();
            }

            Achievements achievement4 = PlayerInfo.Instance.achievements.Find(x => x.id == "achievement4");
            if (!achievement4.status && achievement4.fillAmount < 20)
            {
                achievement4.fillAmount += 1;
                PlayerInfo.Instance.SaveAchievements();
            }

            Achievements achievement5 = PlayerInfo.Instance.achievements.Find(x => x.id == "achievement5");
            if (!achievement5.status && achievement5.fillAmount < 30)
            {
                achievement5.fillAmount += 1;
                PlayerInfo.Instance.SaveAchievements();
            }
        }

        public void Home()
        {
            if (SceneManager.GetActiveScene().name == "Game")
            {
                DisconnectPlayer();
            }
            LoadScene("Home", DoNotDestroy.Instance.loadingPanel);
        }
    }
}
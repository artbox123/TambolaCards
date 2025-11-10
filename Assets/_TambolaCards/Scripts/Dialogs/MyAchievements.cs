using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ArtboxGames
{
    public class MyAchievements : GameManager
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject panelPrefab;

        private void OnEnable()
        {
            StartCoroutine(GenerateAchievements());
        }

        private void OnDisable()
        {
            for (int i = 0; i < content.childCount; i++)
            {
                Destroy(content.transform.GetChild(i).gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            //StartCoroutine(GenerateAchievements());
        }

        private IEnumerator GenerateAchievements()
        {
            yield return new WaitForSeconds(0.2f);
            foreach (Achievements achievement in PlayerInfo.Instance.achievements)
            {
                if (achievement.id.Equals("achievement1") && Application.platform == RuntimePlatform.IPhonePlayer)
                    continue;

                GameObject panel = Instantiate(panelPrefab, content, false);
                panel.name = achievement.id;
                panel.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = achievement.description;
                panel.transform.Find("Progress/OutOf").GetComponent<TextMeshProUGUI>().text = achievement.fillAmount + "/" + achievement.limit;
                panel.transform.Find("Progress/FillAmount").GetComponent<Image>().fillAmount = (achievement.fillAmount / achievement.limit);

                if (achievement.fillAmount == achievement.limit && achievement.status)
                {
                    panel.transform.Find("ClaimNow").gameObject.SetActive(false);
                    panel.transform.Find("ClaimIdle").gameObject.SetActive(false);
                }
                else if (achievement.fillAmount > 0 && achievement.fillAmount == achievement.limit && !achievement.status)
                {
                    panel.transform.Find("ClaimNow").gameObject.SetActive(true);
                    panel.transform.Find("ClaimIdle").gameObject.SetActive(false);
                }
                panel.transform.Find("ClaimNow").GetComponent<Button>().onClick.AddListener(() =>
                {
                    GiveWinAmount(achievement.winAmount);
                    panel.transform.Find("ClaimNow").gameObject.SetActive(false);
                    panel.transform.Find("ClaimIdle").gameObject.SetActive(false);
                    CheckForAchievement(panel.name);
                    PlaySound(2);                    
                });
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void GiveWinAmount(int winAmount)
        {
            AddCoinHistory(Actions.Achievement, winAmount, TransactionType.Credit);
            PlayerInfo.Instance.UpdateCoins(CoinAction.Add, winAmount);
            AdsManager.Instance?.ShowInterstitial();
        }

        private void CheckForAchievement(string id)
        {
            Achievements achievement = PlayerInfo.Instance.achievements.Find(x => x.id == id);
            if (!achievement.status)
            {
                achievement.status = true;
                PlayerInfo.Instance.SaveAchievements();
            }
        }
    }
}
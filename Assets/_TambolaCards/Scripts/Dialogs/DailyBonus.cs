using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace ArtboxGames
{
    public class DailyBonus : GameManager
    {
        [SerializeField] private GameObject[] packages;
        [SerializeField] private GameObject[] coinExplosion;

        public TextMeshProUGUI message;

        private void OnEnable()
        {
            CheckDailyBonus();
        }

        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log("111 DailyBonus : " + PlayerPrefs.GetString("DailyBonus"));
            //Debug.Log("222 LastTimeOfDB : " + PlayerPrefs.GetString("LastTimeOfDB"));
        }

        private void CheckDailyBonus()
        {
            if (!PlayerPrefs.HasKey("DailyBonus"))
            {
                //Debug.Log("first day");
                packages[0].transform.Find("Today").gameObject.SetActive(true);
                message.text = "Ready to get bonus!";
            }
            else
            {
                bool isFinishedBonus = PlayerPrefs.HasKey("FinishedDailyBonus");
                DateTime currentData = DateTime.Now;
                DateTime prevDate = Convert.ToDateTime(PlayerPrefs.GetString("DailyBonus"));

                TimeSpan diff = (currentData - prevDate);
                //Debug.Log("=== diff : " + diff.Days);

                if (diff.Days < 5)
                {
                    //Debug.Log("player days : " + diff.Days);
                    for (int i = 0; i <= diff.Days; i++)
                    {
                        packages[i].transform.Find("Disable").gameObject.SetActive(true);
                    }
                    if (diff.Days < 0)
                        return;

                    DateTime currentData1 = DateTime.Now;
                    DateTime prevDate1 = Convert.ToDateTime(PlayerPrefs.GetString("LastTimeOfDB"));

                    TimeSpan diff1 = (currentData1 - prevDate1);
                    //Debug.Log("### : " + diff1.Days);
                    if (diff1.Days > 1)
                    { // reset counter if player skip any day
                      //Debug.Log("player skip day : " + diff1.Days);
                        for (int i = 0; i <= diff.Days; i++)
                        {
                            packages[i].transform.Find("Disable").gameObject.SetActive(false);
                        }
                        packages[0].transform.Find("Disable").gameObject.SetActive(false);
                        packages[0].transform.Find("Today").gameObject.SetActive(true);
                        PlayerPrefs.DeleteKey("DailyBonus");
                        PlayerPrefs.DeleteKey("LastTimeOfDB");
                        PlayerPrefs.Save();
                        message.text = "Ready to get bonus!";
                        return;
                    }
                    else if (diff1.Days == 1)
                    {
                        //Debug.Log("next day : " + diff.Days);
                        packages[diff.Days].transform.Find("Disable").gameObject.SetActive(false);
                        packages[diff.Days].transform.Find("Today").gameObject.SetActive(true);
                        message.text = "Ready to get bonus!";
                    }
                    StartCoroutine(PackageAnimation(diff.Days + 1));
                }
                else if (diff.Days >= 5)
                {
                    if (isFinishedBonus)
                    {
                        message.text = "You already achieved this rewards.";
                        for (int i = 0; i <= diff.Days; i++)
                        {
                            packages[i].transform.Find("Disable").gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        //Debug.Log("========== reset day");
                        for (int i = 0; i < 5; i++)
                        {
                            packages[i].transform.Find("Disable").gameObject.SetActive(false);
                        }
                        packages[0].transform.Find("Disable").gameObject.SetActive(false);
                        packages[0].transform.Find("Today").gameObject.SetActive(true);
                        PlayerPrefs.DeleteKey("DailyBonus");
                        PlayerPrefs.DeleteKey("LastTimeOfDB");
                        PlayerPrefs.Save();
                        message.text = "Ready to get bonus!";
                        return;
                    }
                }
            }
        }

        private IEnumerator PackageAnimation(int startVal)
        {
            yield return new WaitForSeconds(0.2f);
            for (int i = startVal; i < packages.Length; i++)
            {
                packages[i].transform.localScale = new Vector3(1.1f, 1.1f, 1f);
                yield return new WaitForSeconds(0.1f);
                packages[i].transform.localScale = Vector3.one;
            }
        }

        public void OnClick(int day)
        {
            if (day == 1)
            {
                PlayerPrefs.SetString("DailyBonus", DateTime.Now.ToString());
            }
            PlayerPrefs.SetString("LastTimeOfDB", DateTime.Now.ToString());
            PlayerPrefs.Save();
            packages[day - 1].transform.Find("Disable").gameObject.SetActive(true);
            packages[day - 1].transform.Find("Today").gameObject.SetActive(false);
            PlayerInfo.Instance.UpdateCoins(CoinAction.Add, day * 1000);
            AddCoinHistory(Actions.DailyBonus, day * 1000, TransactionType.Credit);
            message.text = "Come back tomorrow for more reward!";
            coinExplosion[day - 1].SetActive(true);
            StartCoroutine(DisableCoinExplosion(day - 1));

            if (day == 5)
            {
                CheckForAchievement("achievement2");
                PlayerPrefs.SetInt("FinishedDailyBonus", 1);
                PlayerPrefs.Save();
                message.text = "You achieved 5 days rewards.";
            }
            PlaySound(2);
            AdsManager.Instance?.ShowInterstitial();
        }

        private IEnumerator DisableCoinExplosion(int index)
        {
            yield return new WaitForSeconds(2f);
            coinExplosion[index].SetActive(false);
        }

        private void CheckForAchievement(string id)
        {
            Achievements achievement = PlayerInfo.Instance.achievements.Find(x => x.id == id);
            if (!achievement.status)
            {
                achievement.fillAmount = 1;
                PlayerInfo.Instance.SaveAchievements();
            }
        }
    }
}
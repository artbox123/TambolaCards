//using Google;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArtboxGames
{
    public class SocialLogin : GameManager
    {
        public static SocialLogin Instance;

        private string playerName;
        private string playerImage;

        void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            PlayerInfo.Instance.login = 1;
            PlayerInfo.Instance.userID = SystemInfo.deviceUniqueIdentifier;
            PlayerInfo.Instance.userName = "Guest_" + Random.Range(100, 1000);
            PlayerInfo.Instance.userImage = "0";
            LoadScene("Home", null, 1f);
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
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace ArtboxGames
{
    public class DialogManager : GameManager
    {
        public void RateUs()
        {
#if UNITY_IOS
        Device.RequestStoreReview();
#elif UNITY_ANDROID
            Application.OpenURL(ShareURL());
#endif
        }

        public void QuitYes(string sceneName)
        {
            if (sceneName == "Quit")
            {
                Application.Quit();
                return;
            }

            if (SceneManager.GetActiveScene().name == "Game")
            {
                DisconnectPlayer();
            }
            LoadScene(sceneName, DoNotDestroy.Instance.loadingPanel);
            AdsManager.Instance?.ShowInterstitial();
        }

        public void WatchVideo()
        {
            if (!AdsManager.Instance.ShowRewardVideo())
            {
                ShowMessageBox("No video to serve, please try after few minutes");
            }
        }
    }
}
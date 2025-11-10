//using Google;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArtboxGames
{
    public class SocialLogin : GameManager
    {
        public static SocialLogin Instance;

        private string webClientId = "811109521383-kd9si7g055fe8svc3iidf8mm536u4db6.apps.googleusercontent.com";

        //private GoogleSignInConfiguration configuration;

        private loginCallBack l_callback;
        private string playerName;
        private string playerImage;

        void Awake()
        {
            Instance = this;

#if UNITY_ANDROID
            l_callback = loginCallBack.None;

            //configuration = new GoogleSignInConfiguration
            //{
            //    WebClientId = webClientId,
            //    RequestEmail = true,
            //    RequestIdToken = true
            //};
#endif
        }

        public void OnSignIn()
        {
            //GoogleSignIn.Configuration = configuration;
            //GoogleSignIn.Configuration.UseGameSignIn = false;
            //GoogleSignIn.Configuration.RequestIdToken = true;
            //AddStatusText("Calling SignIn");

            //GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
            //  OnAuthenticationFinished);
        }

        public void OnSignOut()
        {
            AddStatusText("Calling SignOut");
            //GoogleSignIn.DefaultInstance.SignOut();
            SceneManager.LoadScene("Login");
        }

        public void OnDisconnect()
        {
            AddStatusText("Calling Disconnect");
            //GoogleSignIn.DefaultInstance.Disconnect();
        }

        //internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        //{
        //    if (task.IsFaulted)
        //    {
        //        using (IEnumerator<System.Exception> enumerator =
        //                task.Exception.InnerExceptions.GetEnumerator())
        //        {
        //            if (enumerator.MoveNext())
        //            {
        //                GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
        //                AddStatusText("Got Error: " + error.Status + " " + error.Message);
        //                l_callback = loginCallBack.Cancel;
        //            }
        //            else
        //            {
        //                AddStatusText("Got Unexpected Exception?!?" + task.Exception);
        //                l_callback = loginCallBack.Failed;
        //            }
        //        }
        //    }
        //    else if (task.IsCanceled)
        //    {
        //        AddStatusText("Canceled");
        //        l_callback = loginCallBack.Cancel;
        //    }
        //    else
        //    {
        //        AddStatusText("Welcome: " + task.Result.DisplayName + "!");
        //        playerName = task.Result.DisplayName;
        //        playerImage = task.Result.ImageUrl.ToString();
        //        PlayerInfo.Instance.login = 2;
        //        l_callback = loginCallBack.Success;
        //    }
        //}

        //public void OnSignInSilently()
        //{
        //    GoogleSignIn.Configuration = configuration;
        //    GoogleSignIn.Configuration.UseGameSignIn = false;
        //    GoogleSignIn.Configuration.RequestIdToken = true;
        //    AddStatusText("Calling SignIn Silently");

        //    GoogleSignIn.DefaultInstance.SignInSilently()
        //          .ContinueWith(OnAuthenticationFinished);
        //}

        //public void OnGamesSignIn()
        //{
        //    GoogleSignIn.Configuration = configuration;
        //    GoogleSignIn.Configuration.UseGameSignIn = true;
        //    GoogleSignIn.Configuration.RequestIdToken = false;

        //    AddStatusText("Calling Games SignIn");

        //    GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
        //      OnAuthenticationFinished);
        //}

        private List<string> messages = new List<string>();
        void AddStatusText(string text)
        {
            if (messages.Count == 5)
            {
                messages.RemoveAt(0);
            }
            messages.Add(text);
            string txt = "";
            foreach (string s in messages)
            {
                txt += "\n" + s;
            }
            Debug.Log("=== Message : " + txt);
        }

        private enum loginCallBack
        {
            None,
            Cancel,
            Success,
            Failed
        }

        private void Update()
        {
            if (l_callback == loginCallBack.Cancel)
            {
                ShowMessageBox("User cancelled login");
            }
            else if (l_callback == loginCallBack.Success)
            {
                PlayerInfo.Instance.userName = playerName;
                PlayerInfo.Instance.userImage = playerImage;
                LoadScene("Home", DoNotDestroy.Instance.loadingPanel);

                if (PlayerInfo.Instance.login == 3 && PlayerPrefs.GetInt("facebook") == 0)
                {
                    PlayerPrefs.SetInt("facebook", 1);
                    PlayerPrefs.Save();
                    PlayerInfo.Instance.UpdateCoins(CoinAction.Add, 5000);
                    CheckForAchievement("achievement1");
                }
            }
            else if (l_callback == loginCallBack.Failed)
            {
                ShowMessageBox("Login failed!");
            }
            l_callback = loginCallBack.None;
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
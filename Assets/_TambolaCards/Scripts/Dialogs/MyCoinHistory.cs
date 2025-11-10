using System.Collections;
using UnityEngine;
using TMPro;

namespace ArtboxGames
{
    public class MyCoinHistory : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject panelPrefab;
        [SerializeField] private TextMeshProUGUI message;

        private void OnEnable()
        {
            StartCoroutine(GenerateCoinHistory());
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
            //StartCoroutine(GenerateCoinHistory());
        }

        private IEnumerator GenerateCoinHistory()
        {
            if (PlayerInfo.Instance.coinHistory.Count > 0)
                message.text = "";
            else
                message.text = "No history found!";
            yield return new WaitForSeconds(0.2f);
            foreach (CoinHistory history in PlayerInfo.Instance.coinHistory)
            {
                GameObject panel = Instantiate(panelPrefab, content, false);
                panel.transform.Find("Date").GetComponent<TextMeshProUGUI>().text = history.date;
                panel.transform.Find("Action").GetComponent<TextMeshProUGUI>().text = history.action.ToString();
                if (history.action == Actions.GameWin)
                {
                    panel.transform.Find("Action").GetComponent<TextMeshProUGUI>().color = Color.green;
                }
                else if (history.action == Actions.GameLose)
                {
                    panel.transform.Find("Action").GetComponent<TextMeshProUGUI>().color = Color.red;
                }
                panel.transform.Find("Coins").GetComponent<TextMeshProUGUI>().text = history.coins;
                panel.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = history.type;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
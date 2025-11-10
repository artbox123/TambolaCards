using UnityEngine;
using UnityEngine.UI;

namespace ArtboxGames
{
	public class NumberBall : MonoBehaviour
	{
		public Image fillImage;
		float time;

		void Start()
		{
			time = StaticData.ballTime;
		}

		void Update()
		{
			if (time > 0)
			{
				time -= Time.deltaTime * 1f;
				fillImage.fillAmount = time / StaticData.ballTime;
			}
			else
			{
				this.gameObject.GetComponent<NumberBall>().enabled = false;
			}
		}
	}
}
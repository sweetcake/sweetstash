using UnityEngine;
using System;
namespace Game
{
	public class HUD : MonoBehaviour
	{
		public bool UseTime = false;
		private Game game;
		private UILabel time;

		void Awake()
		{
			game = GameObject.Find("Game").GetComponent<Game>();
			time = gameObject.transform.Find("Time").GetComponent<UILabel>();
		}

		void LateUpdate()
		{
			if(game != null)
			{
				if(UseTime)
				{
					TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.Ceil(game.GameTime));
					time.text = string.Format("{0:0}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
				}
				else
				{
					time.text = game.Distance.ToString("n0");
				}
			}
		}

		public void OnRestart()
		{
			if(game != null)
			{
				game.Restart();
			}
		}

		public void OnCreateNewTrack()
		{
			if(game != null)
			{
				game.CreateNewTrack();
			}
		}

		public void OnPause()
		{
			if(game != null)
			{
				game.Pause();
			}
		}
	}
}


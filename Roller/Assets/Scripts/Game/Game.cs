using System;
using UnityEngine;
namespace Game
{
	public class Game : MonoBehaviour
	{
		private Ball ball;
		private Road road;

		private bool gameStarted = false;

		private bool paused = false;

		private float gameTime = 0.0f;
		private float distance = 0.0f;

		void Start()
		{
			Application.targetFrameRate = 60;
			ball = transform.FindChild("Ball").gameObject.GetComponent<Ball>();
			road = GameObject.Find("Road").GetComponent<Road>();
			road.Populate();
		}

		private int frames = 0;
		private float time = 0.0f;
		void Update()
		{
			if(!gameStarted)
			{
				Restart();
				gameStarted = true;
			}
			else if(ball.Alive)
			{
				gameTime += Time.deltaTime;
				//distance = road.Spline.ConvertNormalizedParameterToDistance(road.Spline.GetClosestPointParam(ball.transform.position, 5));
			}

			time += Time.deltaTime;
			++frames;
			if(time > 1)
			{
				//Debug.Log("FRAMERATE: " + frames);
				frames = 0;
				time = 0;
			}
		}

		public void Restart()
		{
			time = 0;
			distance = 0;
			ball.Deactivate();
			if(road != null)
			{
				ball.InitTransform(road.StartPoint);
			}
			else
			{
				ball.InitTransform(transform);
			}
			ball.Activate();
		}

		public void Pause()
		{
			paused = !paused;
			Time.timeScale = paused ? 0.0f : 1.0f;
		}

		public float GameTime 
		{
			get { return gameTime; }
 		}

		public float Distance 
		{
			get { return distance; }
		}
	}
}


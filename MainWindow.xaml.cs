using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FlappyBird
{
	public partial class MainWindow : Window
	{
		private double gravity;
		private int score;
		private bool gameOver;

		private bool spaceIsHeld;

		private readonly Random random = new();

		public MainWindow()
		{
			InitializeComponent();

			CompositionTarget.Rendering += GameEngine;

			StartGame();
		}

		private void StartGame()
		{
			score = 0;
			gravity = -3;
			gameOver = false;

			Canvas.SetTop(flappyBird, 114);

			var obstacles = mainCanvas.Children.OfType<Canvas>().Where(o => o.Name.ToLower().StartsWith("obstacle")).ToArray();

			for(int i = 0; i < obstacles.Length; i++)
			{
				var obstacle = obstacles[i];

				Canvas.SetTop(obstacle, -random.Next(141));
				if(obstacle.Name == "obstacle" + i)
				{
					Canvas.SetLeft(obstacle, 100 * (i + 1));
				}
			}

			var clouds = mainCanvas.Children.OfType<Image>().Where(o => o.Name.ToLower().StartsWith("cloud")).ToArray();

			for(int i = 0; i < clouds.Length; i++)
			{
				var cloud = clouds[i];

				if(cloud.Name == "cloud" + i)
				{
					Canvas.SetLeft(cloud, random.Next(50, 223));
				}
			}
		}

		private void StopGame()
		{
			gameOver = true;
			scoreLabel.Content += "   Game over, press R to try again.";
		}

		private void GameEngine(object sender, EventArgs arguments)
		{
			if(Keyboard.IsKeyDown(Key.Space))
			{
				if(!spaceIsHeld)
				{
					spaceIsHeld = true;
					gravity = -3;
				}
			}
			else
			{
				spaceIsHeld = false;
			}

			if(Keyboard.IsKeyDown(Key.R) && gameOver)
			{
				StartGame();
			}

			if(gameOver) return;

			if(gravity >= 8)
			{
				gravity = 8;
			}
			else
			{
				gravity += 0.25;
			}

			flappyBird.RenderTransform = new RotateTransform(gravity * 15, flappyBird.Width * 0.5, flappyBird.Height * 0.5);

			scoreLabel.Content = "Score: " + score;
			Rect flappyRect = new(Canvas.GetLeft(flappyBird), Canvas.GetTop(flappyBird), flappyBird.Width, flappyBird.Height);
			Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + gravity);

			if(Canvas.GetTop(flappyBird) <= -flappyBird.Height || Canvas.GetTop(flappyBird) >= mainCanvas.Height)
			{
				StopGame();
			}

			foreach(var pipes in mainCanvas.Children.OfType<Canvas>().Where(o => o.Name.ToLower().StartsWith("obstacle")))
			{
				foreach(var pipe in pipes.Children.OfType<Image>())
				{
					Rect pillars = new(Canvas.GetLeft(pipes), Canvas.GetTop(pipes) + Canvas.GetTop(pipe), 26, 160);

					if(flappyRect.IntersectsWith(pillars))
					{
						StopGame();
					}
				}

				Canvas.SetLeft(pipes, Canvas.GetLeft(pipes) - 1);

				if((string)pipes.Tag == "up")
				{
					Canvas.SetTop(pipes, Canvas.GetTop(pipes) - 1);
					if(Canvas.GetTop(pipes) <= -140)
					{
						pipes.Tag = "down";
					}
				}
				else if((string)pipes.Tag == "down")
				{
					Canvas.SetTop(pipes, Canvas.GetTop(pipes) + 1);
					if(Canvas.GetTop(pipes) >= 0)
					{
						pipes.Tag = "up";
					}
				}

				if(Canvas.GetLeft(pipes) < -26)
				{
					Canvas.SetLeft(pipes, Canvas.GetLeft(pipes) + 400);
					Canvas.SetTop(pipes, -random.Next(141));
					score += 1;
				}
			}

			foreach(var image in mainCanvas.Children.OfType<Image>().Where(o => o.Name.ToLower().StartsWith("cloud")))
			{
				Canvas.SetLeft(image, Canvas.GetLeft(image) - 1);

				if(Canvas.GetLeft(image) < -image.Width)
				{
					Canvas.SetLeft(image, random.Next(320, 421));
				}
			}
		}
	}
}

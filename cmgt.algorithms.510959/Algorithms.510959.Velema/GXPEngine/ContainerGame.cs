using GXPEngine;
using System;
using System.Drawing;
//using GXPEngine.Scenes;

internal class ContainerGame : Game
{
	private bool showfps;
	private readonly EasyDraw fpsCounter;

	private AlgorithmsAssignment algoScene;

	public float SoundVolume = 0.05f;

	public ContainerGame() : base(1200, 900, false, true, -1, -1, false)
	{
		// Configure FPS
		TargetFps = int.MaxValue;
		// Configure FPS tracker UI-object
		showfps = true;
		fpsCounter = new EasyDraw(200, 50);
		fpsCounter.TextAlign(CenterMode.Min, CenterMode.Min);
		AddChild(fpsCounter);

		algoScene = new AlgorithmsAssignment();
		algoScene.Load();

		//Console.WriteLine("MyGame initialized");
	}

	private void Update()
	{
		HandleInput();

		if (showfps)
		{
			fpsCounter.ClearTransparent();
			fpsCounter.Fill(Color.Green);
			fpsCounter.Text($"{CurrentFps}");
		}
	}

	private void HandleInput()
	{
		if (Input.GetKeyDown(Key.F1))
		{
			showfps = !showfps;
			fpsCounter.visible = showfps;
		}
	}

	static void Main()
	{
		new ContainerGame().Start();
	}
}
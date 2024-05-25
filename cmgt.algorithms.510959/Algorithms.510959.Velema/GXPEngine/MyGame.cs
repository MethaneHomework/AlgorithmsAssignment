using GXPEngine;
using GXPEngine.Assets;
using GXPEngine.Control;
using GXPEngine.Debugging;
using System;
using System.Collections.Generic;
using System.Drawing;
//using GXPEngine.Scenes;

internal class MyGame : Game
{
	private bool showfps;
	private readonly EasyDraw fpsCounter;

	public float SoundVolume = 0.05f;

	public MyGame() : base(800, 600, false, true, -1, -1, false)
	{
		// Configure FPS
		TargetFps = int.MaxValue;
		// Configure FPS tracker UI-object
		showfps = true;
		fpsCounter = new EasyDraw(200, 50);
		fpsCounter.TextAlign(CenterMode.Min, CenterMode.Min);
		AddChild(fpsCounter);

		AlgorithmsAssignment aa = new AlgorithmsAssignment();
		aa.Load();

		Console.WriteLine("MyGame initialized");
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

		if (Input.GetKeyDown(Key.R)) new AlgorithmsAssignment().Load();
	}
	static void Main()
	{
		new MyGame().Start();
	}
}
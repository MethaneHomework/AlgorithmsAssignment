using System;
using System.Collections.Generic;
using System.Drawing;
using GXPEngine;
using GXPEngine.OpenGL;

/**
 * This is the main 'game' for the Algorithms Assignment that accompanies the Algorithms course.
 * 
 * Read carefully through the assignment that you are currently working on
 * and then through the code looking for all pointers & TODO's that you have to implement.
 * 
 * The course is 6 weeks long and this is the only assignment/code that you will get,
 * split into 3 major parts (see below). This means that you have three 2 week sprints to
 * work on your assignments.
 */
class AlgorithmsAssignment : Game
{
	readonly Size _size;

	//Required for assignment 1
	Dungeon _dungeon = null;

	//Required for assignment 2
	NodeGraph _graph = null;
	TiledView _tiledView = null;
	NodeGraphAgent _agent = null;

	//Required for assignment 3
	PathFinder _pathFinder = null;

	//common settings
	private const int SCALE = 16;				// Done: experiment with changing this
	private const int MIN_ROOM_SIZE = 7;		// Done: use this setting in your dungeon generator

	public AlgorithmsAssignment() : base(1200, 800, false, true, -1, -1, false)
	{
		/////////////////////////////////////////////////////////////////////////////////////////
		///	BASE SETUP - FEEL FREE TO SKIP

		//set our default background color and title
		GL.ClearColor(1, 1, 1, 1);
		GL.glfwSetWindowTitle("Algorithms Game");

		// The simplest approach to visualize a dungeon, is using black and white squares
		// to show where the walls (black) and walkable areas/doors (white) are.
		// A quick and easy way to implement that is by creating a small canvas, 
		// draw black and white pixels on it and scale it up by an insane amount (e.g. 40).
		//
		// To visualize where these scaled pixels are we also add a grid, where we use
		// this same SCALE value as a grid size setting. Comment out the next line to hide it.
		Grid grid = new Grid(width, height, SCALE);
		AddChild(grid);

		/////////////////////////////////////////////////////////////////////////////////////////
		///	ASSIGNMENT 1 : DUNGEON - READ CAREFULLY
		///

		// The Dungeon in this assignment is an object that holds Rooms & Doors instances, and
		// extends a canvas that we scale up so that it can visualize these rooms & doors.
		// In a 'real' setting you would split this 'model' of the dungeon from the visualization,
		// but we chose to not make it more complicated than necessary.
		//
		// To calculate the size of the dungeon we can create, we take our screen size and
		// divide it by how much we want to scale everything up. For example if our screen size is 800 
		// and the dungeon scale 40, we would like our dungeon to have a max width of 20 'units'
		// so that if we scale it up by 40, its screenwidth is 800 pixels again.
		// Basically this means every pixel drawn in the dungeon has the size of the SCALE setting.
		// Eg walls are SCALE pixels thick, doors are squares with an area of SCALE * SCALE pixels.
		_size = new Size(width / SCALE, height / SCALE);

		InitializeDungeon();
	}

	private void InitializeDungeon()
	{
		///////////////////////////////////////
		// Assignment 1.1 Sufficient (Mandatory)
		// 
		// TODO: Create SufficientDungeon class
		// TODO: Create GoodDungeon class
		// TODO: Create ExcellentDungeon class

		//_dungeon = new SampleDungeon(_size);
		_dungeon = new SufficientDungeon(_size);
		//_dungeon = new GoodDungeon(_size);
		//_dungeon = new ExcellentDungeon(_size);

		if (_dungeon != null)
		{
			//assign the SCALE we talked about above, so that it no longer looks like a tinietiny stamp:
			_dungeon.scale = SCALE;
			//Tell the dungeon to generate rooms and doors with the given MIN_ROOM_SIZE
			_dungeon.Generate(MIN_ROOM_SIZE);
		}

		// -----------------------------------------------------------------
		// Assignment 2.1 Sufficient (Mandatory) OnGraphWayPointAgent
		//
		// TODO: Implement an OnGraphWayPointAgent class
		// TODO: Implement HighLevelDungeonNodeGraph
		//
		// -----------------------------------------------------------------
		//
		// Assignment 2.2 Good (Optional) TiledView & RandomWayPointagent
		//
		// TODO: Implement TiledView
		// TODO: Implement a RandomWayPointAgent class
		//
		// -----------------------------------------------------------------
		//
		// Assignment 2.3 Excellent (Optional) LowLevelDungeonNodeGraph
		//
		// TODO: Implement LowLevelDungeonNodeGraph
		//

		//_graph = new SampleDungeonNodeGraph(_dungeon);
		//_graph = new HighLevelDungeonNodeGraph(_dungeon);
		//_graph = new LowLevelDungeonNodeGraph(_dungeon);
		_graph?.Generate();

		//_tiledView = new SampleTiledView(_dungeon, TileType.GROUND);
		//_tiledView = new TiledDungeonView(_dungeon, TileType.GROUND); 
		_tiledView?.Generate();

		//_agent = new SampleNodeGraphAgent(_graph);
		//_agent = new OnGraphWayPointAgent(_graph);
		//_agent = new RandomWayPointAgent(_graph);	


		// -----------------------------------------------------------------
		// Assignment 3.1 Sufficient (Mandatory) - Recursive Pathfinding
		//
		// TODO: Implement a RecursivePathFinder
		// TODO: Implement a BreadthFirstPathFinder
		//
		// TODO: Implement a PathFindingAgent that uses one of your pathfinder implementations (should work with any pathfinder implementation)
		// -----------------------------------------------------------------
		// Assignment 3.2 Good & 3.3 Excellent (Optional)
		//
		// There are no more explicit TODO's to guide you through these last two parts.
		// You are on your own. Good luck, make the best of it. Make sure your code is testable.
		// For example for A*, you must choose a setup in which it is possible to demonstrate your 
		// algorithm works. Find the best place to add your code, and don't forget to move the
		// PathFindingAgent below the creation of your PathFinder!

		//_pathFinder = new SamplePathFinder(_graph);
		//_pathFinder = new RecursivePathFinder(_graph);
		//_pathFinder = new BreadthFirstPathFinder(_graph);

		//_agent = new PathFindingAgent(_graph, _pathFinder);


		// REQUIRED BLOCK OF CODE TO ADD ALL OBJECTS YOU CREATED TO THE SCREEN IN THE CORRECT ORDER
		// LOOK BUT DON'T TOUCH :)
		if (_dungeon != null) AddChild(_dungeon);
		if (_tiledView != null) AddChild(_tiledView);
		if (_graph != null) AddChild(_graph);
		if (_pathFinder != null) AddChild(_pathFinder);             //pathfinder on top of that
		if (_graph != null) AddChild(new NodeLabelDrawer(_graph));  //node label display on top of that
		if (_agent != null) AddChild(_agent);                       //and last but not least the agent itself
	}

	private void Update()
	{
        if (Input.GetKeyDown(Key.R))
        {
			ClearChildren();
			Console.Clear();
			InitializeDungeon();
        }
    }

	private void ClearChildren()
	{
		List<GameObject> children = GetChildren();
		for (int i = children.Count - 1; i >= 0; i--)
		{
			GameObject child = children[i];
			if (child is Grid) continue;
			RemoveChild(child);
			child.Destroy();
		}
	}
}



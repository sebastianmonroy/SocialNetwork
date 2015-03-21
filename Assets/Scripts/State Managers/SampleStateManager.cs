using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is a sample state manager that uses
public class SampleStateManager : MonoBehaviour {
	#region VARIABLES
	// INSTANCE
	public static SampleStateManager instance;

	// STATES
	public SimpleStateMachine stateMachine;
	SimpleState menuState, gameState, winState;

	// Note: it is common to have the SampleStateManager oversee other StateManagers. (calling their StateManager.Execute() methods within this SampleStateManager's appropriate states' update method).
		// For example, if there were a BossStateManager attached to the boss enemy and it was defeated, the BossStateManager might enter the deathState. 
		// This SampleStateManager might see that the BossStateManager was in the deathState (ie. bossStateManager.state == "DEATH") and cause a transition to the winState as a result.
	#endregion

	void Awake () 
	{
		instance = this;
	}

	void Start () 
	{
		//DEFINE STATES
		menuState = new SimpleState(MenuEnter, MenuUpdate, MenuExit, "[MENU]");
		gameState = new SimpleState(GameEnter, GameUpdate, GameExit, "[GAME]");
		winState = new SimpleState(WinEnter, WinUpdate, WinExit, "[WIN]");

		// this is how you switch states!
		stateMachine.SwitchStates(menuState);
	}

	void Update() 
	{
		Execute();
	}

	// This is called every frame. 
	public void Execute () 
	{
		stateMachine.Execute();
	}

	#region MENU
	void MenuEnter() 
	{
		// initialize menu stuff
		// will be called once upon entering the menuState
	}

	void MenuUpdate() 
	{
		// receive player input to menu stuff, cause conditional transitions to other states
		// will be called every frame within the menuState
	}	

	void MenuExit()
	{
		// clean up menu stuff, switch scenes, whatever
		// will be called once upon exiting the menuState
	}
	#endregion

	#region GAME
	void GameEnter() 
	{
		// initialize game stuff
	}

	void GameUpdate() 
	{
		// core game loop, cause transition to winState if the player does winningest stuff
	}

	void GameExit() 
	{

	}
	#endregion

	#region WIN
	void WinEnter() 
	{
		// initialize win menu stuff
	}

	void WinUpdate() 
	{
		// win menu animations and player input
	}

	void WinExit() 
	{
		// clean up win menu stuff, switch to main menu, who knows
	}
	#endregion
}

using Godot;
using System;

public partial class Main : Node
{
	private PhaseSwitch _phaseSwitch = null;


	public override void _Ready()
	{
		_phaseSwitch = GetNode<PhaseSwitch>("PhaseSwitch");

		GD.Print(GetPath(), " is ready");
	}


	public override void _Process(double delta)
	{
	}


	private void OnEnterPhase(string phase, string prevPhase)
	{
		GD.Print(GetPath(), ": Entering phase ", phase);

		if (phase == string.Empty)
		{
			GetTree().Quit();
		}
		else if (phase.Equals("start"))
		{
			SetupDefaults();
		}
	}


	private void OnQuitPhase(string phase, string nextPhase)
	{
		if (phase == string.Empty)
		{
			GD.Print(GetPath(), ": Starting ..");
		}
	}

	private void SetupDefaults()
	{
		GD.Print(GetPath(), ": Setting up defaults ..");
	}

	private void OnQuitButtonPressed()
	{
		GD.Print(GetPath(), ": Quit button pressed");
		_phaseSwitch.Drop();
	}
}

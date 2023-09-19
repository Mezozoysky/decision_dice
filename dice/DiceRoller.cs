using Godot;
using System;

public partial class DiceRoller : Node
{
	[Signal]
	public delegate void FacesNumChangedEventHandler(uint facesNum);
	[Signal]
	public delegate void DicesNumChangedEventHandler(uint dicesNum);

	[Export]
	private uint _facesDefault = 6;
	[Export]
	private uint _facesMinimal = 2;
	[Export]
	private uint _dicesDefault = 1;
	[Export]
	private uint _dicesMinimal = 1;

	private uint _faces = 0;
	private uint _dices = 0;

	public uint Faces
	{
		get { return _faces; }
		set
		{
			if (value < _facesMinimal)
			{
				value = _facesMinimal;
			}
			if (value != _faces)
			{
				_faces = value;
				EmitSignal("FacesNumberChanged", _faces);
			}
		}
	}

	public uint Dices
	{
		get { return _dices; }
		set
		{
			if (value < _dicesMinimal)
			{
				value = _dicesMinimal;
			}
			if (value != _dices)
			{
				_dices = value;
				EmitSignal("DicesNumberChanged", _dices);
			}
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetupDefaults();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetupDefaults()
	{
		Faces = _facesDefault;
		Dices = _dicesDefault;
	}

	public void SetupMinimal()
	{
		Faces = _facesMinimal;
		Dices = _dicesMinimal;
	}
}

using Godot;
using System;
using System.Collections.Generic;

public partial class PhaseSwitch : Node
{
	[Signal]
	public delegate void QuitPhaseEventHandler(string phase, string nextPhase);
	[Signal]
	public delegate void EnterPhaseEventHandler(string phase, string prevPhase);
	[Signal]
	public delegate void PhaseStackedEventHandler(string phase);
	[Signal]
	public delegate void PhaseUnstackedEventHandler(string phase);

	[Export]
	private string[] _phases = { "start" };
	[Export]
	private string _startPhase = "start";

	private Stack<string> _stack = new Stack<string>();
	private List<string> _stackingList = new List<string>();
	private List<string> _unstackingList = new List<string>();
	private string _currPhase = string.Empty;
	private string _prevPhase = string.Empty;
	private bool _isSwitching = false;


	public string CurrPhase
	{
		get { return _currPhase; }
	}


	public string PrevPhase
	{
		get { return _prevPhase; }
	}


	public bool IsSwitching
	{
		get { return _isSwitching; }
	}

	public string GetTopPhase()
	{
		if (_stack.Count == 0)
		{
			return string.Empty;
		}
		return _stack.Peek();
	}

	
	public bool IsPhaseStacked(string phase)
	{
		return _stack.Contains(phase);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (string.IsNullOrEmpty(_startPhase))
		{
			GD.PrintErr(GetPath(), ": no start phase set");
			return;
		}

		if (_phases.IsEmpty() || !Array.Exists(_phases, phase => phase == _startPhase))
		{
			GD.PrintErr(GetPath(), ": start phase '", _startPhase, "' isn't in list of available phases are [", string.Join(", ", _phases), "]");
			return;
		}

		Push(_startPhase);
	}


	public void Push(string phase)
	{
		if (string.IsNullOrEmpty(phase))
		{
			GD.PrintErr(GetPath(), ": failed to push empty phase");
			return;
		}

		if (!Array.Exists(_phases, phase => phase == _startPhase))
		{
			GD.PrintErr(GetPath(), ": failed to push phase '", _startPhase, "'; available phases are [", string.Join(", ", _phases), "]");
			return;
		}

		bool stacking = !_stackingList.Contains(phase);
		_stack.Push(phase);
		if (stacking)
		{
			_stackingList.Add(phase);
		}

		_isSwitching = true;
	}


	public void Pop()
	{
		if (_stack.Count == 0)
		{
			GD.PrintErr(GetPath(), ": failed to pop phase; stack is empty");
			return;
		}

		string phase = _stack.Pop();
		if (!_stack.Contains(phase))
		{
			_unstackingList.Add(phase);
		}

		_isSwitching = true;
	}


	public void PopUntil(string phase)
	{
		if (!IsPhaseStacked(phase))
		{
			GD.PrintErr(GetPath(), ": failed to pop until phase '", phase, "'; phase in not stacked");
			return;
		}

		while (phase != GetTopPhase())
		{
			Pop();
		}

		_isSwitching = true;
	}


	public void Replace(string phase)
	{
		if (string.IsNullOrEmpty(phase))
		{
			GD.PrintErr(GetPath(), ": failed to replace with empty phase");
			return;
		}

		if (!Array.Exists(_phases, phase => phase == _startPhase))
		{
			GD.PrintErr(GetPath(), ": failed to replace with phase '", _startPhase, "'; available phases are [", string.Join(", ", _phases), "]");
			return;
		}

		Pop();
		Push(phase);
	}


	public void Drop()
	{
		if (_stack.Count == 0)
		{
			GD.PrintErr(GetPath(), ": failed to drop phases; stack is empty");
			return;
		}

		while (_stack.Count > 0)
		{
			Pop();
		}
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isSwitching)
		{
			CallDeferred("Switch");
		}
	}


	private void Switch()
	{
		if (!_isSwitching)
		{
			GD.PrintErr(GetPath(), ": failed to switch; is not switching");
			return;
		}

		if (_currPhase == GetTopPhase())
		{
			GD.PrintErr(GetPath(), ": failed to switch; already switched to '", _currPhase, "'");
		}
		else
		{
			EmitSignal("QuitPhase", _currPhase, GetTopPhase());
			while (_unstackingList.Count > 0)
			{
				EmitSignal("PhaseUnstacked", _unstackingList[0]);
				_unstackingList.RemoveAt(0);
			}

			while (_stackingList.Count > 0)
			{
				EmitSignal("PhaseStacked", _stackingList[0]);
				_stackingList.RemoveAt(0);
			}

			_prevPhase = _currPhase;
			_currPhase = GetTopPhase();
			EmitSignal("EnterPhase", _currPhase, _prevPhase);
		}

		_isSwitching = false;
    }
}

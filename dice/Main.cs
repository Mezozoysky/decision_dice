using Godot;
using System;

public partial class Main : Node
{
    [Export]
    private uint _variantsDefault = 2;
    [Export]
    private uint _variantsMinimal = 1;

    private uint _variants = 0;
    public uint Variants
    {
        get { return _variants; }
        set
        {
            if (value < _variantsMinimal)
            {
                value = _variantsMinimal;
            }
            if (value != _variants)
            {
                _variants = value;
                OnVariantsNumberChanged();
            }
        }
    }

    bool _isVariantsLineInvalid = false;

    private PhaseSwitch _phaseSwitch = null;
    private DiceRoller _diceRoller = null;

    private LineEdit _variantsLine = null;
    private LineEdit _facesLine = null;
    private LineEdit _dicesLine = null;

    public override void _Ready()
    {
        _phaseSwitch = GetNode<PhaseSwitch>("PhaseSwitch");
        _diceRoller = GetNode<DiceRoller>("DiceRoller");

        _variantsLine = GetNode<LineEdit>("BottomView/HBox/CenterVBox/VariantsHBox/VariantsLine");
        _facesLine = GetNode<LineEdit>("BottomView/HBox/CenterVBox/FacesHBox/FacesLine");
        _dicesLine = GetNode<LineEdit>("BottomView/HBox/CenterVBox/DicesHBox/DicesLine");

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

        _diceRoller.SetupDefaults();
        Variants = _variantsDefault;
    }


    private void SetupMinimal()
    {
        GD.Print(GetPath(), ": Setting up minimal ..");

        _diceRoller.SetupMinimal();
        Variants = _variantsMinimal;
    }


    private void OnVariantsNumberChanged()
    {
        string newText = _variants.ToString();
        if (newText != _variantsLine.Text)
        {
            _variantsLine.Text = newText;
            _variantsLine.CaretColumn = newText.Length;
        }
    }


    private void OnQuitButtonPressed()
    {
        GD.Print(GetPath(), ": Quit button pressed");
        _phaseSwitch.Drop();
    }


    private void OnVariantsLineTextChanged(string newText)
    {
        GD.Print(GetPath(), ": Variants line text changed '", newText, "'");
        if (newText == string.Empty)
        {
            _isVariantsLineInvalid = true;
            return;
        }

        uint number = 0;
        if (!UInt32.TryParse(newText, out number))
        {
            GD.PrintErr(GetPath(), ": failed to parse variants number from string '", newText, "'");
            OnVariantsNumberChanged(); // not really changed, just set the old number into line
            return;
        }

        if (number < _variantsMinimal)
        {
            GD.PrintErr(GetPath(), ": entered variants number ", number, " less than minimal ", _variantsMinimal);
            _isVariantsLineInvalid = true;
            return;
        }

        Variants = number;
        _isVariantsLineInvalid = false;
    }


    private void OnVariantsLineTextSubmitted(string newText)
    {
        if (_isVariantsLineInvalid)
        {
            _variantsLine.Text = Variants.ToString();
            _isVariantsLineInvalid = false;
        }
    }


    private void OnVariantsLineFocusExited()
    {
        if (_isVariantsLineInvalid)
        {
            _variantsLine.Text = Variants.ToString();
            _isVariantsLineInvalid = false;
        }
    }

}


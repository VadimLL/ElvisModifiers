// Mediator pattern

using System;

namespace TestAnalyzerLib11;

file interface IMediator
{
    // Only colleagues can send messages via the mediator
    [OnlyYou<Colleague>]
    void Notify(object sender, string eventName);
}

file abstract class Colleague
{
    readonly IMediator _mediator;

    //[OnlyYou<Colleague>] !!! not implemented
    protected Colleague(IMediator mediator) => _mediator = mediator;
    protected void Send(string eventName) => _mediator.Notify(this, eventName);
}

file class Button : Colleague
{
    [OnlyYou<IMediator>]
    public Button(IMediator mediator) : base(mediator) { }

    public void Click()
    {
        Console.WriteLine("Button clicked");
        Send("buttonClicked"); // Allowed as a derived from the Colleague
    }
}

file class TextBox : Colleague
{
    [OnlyYou<IMediator>]
    public TextBox(IMediator mediator) : base(mediator) { }

    public string Text { get; private set; } = "";

    public void SetText(string text)
    {
        Text = text;
        Send("textChanged");
    }
}

// Specific mediator
file class MyDialogMediator : IMediator
{
    readonly Button _okButton;
    readonly TextBox _nameField;

    public MyDialogMediator()
    {
        _okButton = new Button(this);
        _nameField = new TextBox(this);
    }

    public void Notify(object sender, string eventName)
    {
        if (sender is TextBox && eventName == "textChanged")
        {
            // Enable the button if the field is not empty
            if (!string.IsNullOrEmpty(_nameField.Text))
            {
                Console.WriteLine("Enabling OK button");
            }
        }
        // ...
    }
}

// No one else can create colleagues and send messages
file class Outsider
{
    void TryHack()
    {
        var dialog = new MyDialogMediator();
        
        var button = /*EA_METH_001*/ new Button(dialog); // Compilation error! Outsider is not IMediator

        /*EA_METH_001*/ dialog.Notify(button, "hack event"); // Compilation error!
    }
}

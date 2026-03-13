// Шаблон "Посредник" (Mediator)

using System;

namespace TestAnalyzerLib11;

file interface IMediator
{
    // Только клллеги могут отправлять сообщения через посредника
    [OnlyYou<Colleague>]
    void Notify(object sender, string eventName);
}

file abstract class Colleague
{
    readonly IMediator _mediator;
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
        Send("buttonClicked"); // РАЗРЕШЕНО - Button наследник Colleague
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
        Send("textChanged"); // РАЗРЕШЕНО
    }
}

// Конкретный посредник
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
            // Включаем кнопку, если поле не пустое
            if (!string.IsNullOrEmpty(_nameField.Text))
            {
                Console.WriteLine("Enabling OK button");
            }
        }
        // ...
    }
}

// Никто другой не может созавать коллег и отправлять сообщения
file class Outsider
{
    void TryHack()
    {
        var dialog = new MyDialogMediator();
        
        var button = /*EA_METH_001*/ new Button(dialog); // ОШИБКА! Outsider не IMediator

        /*EA_METH_001*/ dialog.Notify(button, "hack event"); // ОШИБКА! 
    }
}

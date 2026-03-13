// Шаблон "Снимок/Хранитель" (Memento)

using System.Collections.Generic;

namespace TestAnalyzerLib11;

file class Originator // Создатель
{
    // Внутренний класс с доступом только для Originator
    [OnlyYou<Originator>]   
    public class Memento // Снимок/Хранитель
    {
        // Только Originator может создавать Memento
        public Memento(string state) => State = state;

        // Только Originator может читать состояние
        public string State { get; }
    }

    public string State { get; set; } = "";

    // Только Caretaker может создавать Снимок
    [OnlyYou<Caretaker>]
    public Memento Save() => new Memento(State);

    // Только Caretaker может восстанавливать Снимок
    [OnlyYou<Caretaker>]
    public void Restore(Memento memento) => State = memento.State;
}

// Может хранить Memento, но не может читать или изменять его
file class Caretaker // Опекун
{
    private Stack<Originator.Memento> _history = new ();
    public void SaveState(Originator originator) => _history.Push(originator.Save());
    public void Undo(Originator originator)
    {
        if (_history.Count > 0)
        {
            var memento = _history.Pop();
            var state = /*EA_TYPE_001*/ memento.State; // ОШИБКА компиляции! Caretaker не Originator
            originator.Restore(memento);
        }
    }
}

// Использование
file class Program
{
    static void Main()
    {
        var origin = new Originator();
        var care = new Caretaker();

        origin.State = "A";
        care.SaveState(origin);
        origin.State = "B";
        care.Undo(origin); // Вернет состояние "A"

        // Но:
        var memento = /*EA_METH_001*/ origin.Save(); // ОШИБКА компиляции!
        var state = /*EA_TYPE_001*/ memento.State; // ОШИБКА компиляции!
        memento = /*EA_TYPE_001*/ new Originator.Memento("bad state"); // ОШИБКА компиляции!
    }
}

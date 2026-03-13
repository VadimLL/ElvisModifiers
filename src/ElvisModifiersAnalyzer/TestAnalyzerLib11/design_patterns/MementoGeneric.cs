// Шаблон "Снимок/Хранитель" (Memento) generic версия

using System.Collections.Generic;

namespace TestAnalyzerLib11;

[OnlyYou(typeof(Originator<>))]
file class Memento<T> // Снимок/Хранитель
{
    // Только Originator может создавать Memento
    public Memento(T state) => State = state;

    // Только Originator может читать состояние
    public T State { get; }
}

file class Originator<T>  // Создатель
{
    public T State { get; set; } = default!;

    // Только Caretaker может создавать Снимок
    [OnlyYou(typeof(Caretaker<>))]
    public Memento<T> Save() => new Memento<T>(State);

    // Только Caretaker может восстанавливать Снимок
    [OnlyYou(typeof(Caretaker<>))]
    public void Restore(Memento<T> memento) => State = memento.State;
}

// Может хранить Memento, но не может читать или изменять его
file class Caretaker<T> // Опекун
{
    private Stack<Memento<T>> _history = new ();
    public void SaveState(Originator<T> originator) => _history.Push(originator.Save());
    public void Undo(Originator<T> originator)
    {
        if (_history.Count > 0)
        {
            var memento = _history.Pop();
            originator.Restore(memento);

            var state = /*EA_TYPE_001*/ memento.State; // ОШИБКА компиляции! Caretaker не Originator
        }
    }
}

// Использование
class Program
{
    static void Main()
    {
        var origin = new Originator<string>();
        var care = new Caretaker<string>();

        origin.State = "A";
        care.SaveState(origin);
        origin.State = "B";
        care.Undo(origin); // Восстановит состояние "A"

        // Но:
        var memento = /*EA_METH_001*/ origin.Save(); // ОШИБКА компиляции!
        var state = /*EA_TYPE_001*/ memento.State; // ОШИБКА компиляции!
        memento = /*EA_TYPE_001*/ new Memento<string>("bad state"); // ОШИБКА компиляции!
    }
}



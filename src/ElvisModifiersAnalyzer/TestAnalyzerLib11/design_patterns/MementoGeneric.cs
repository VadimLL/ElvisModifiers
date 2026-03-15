// Memento pattern (generic version)

using System.Collections.Generic;

namespace TestAnalyzerLib11;

[OnlyYou(typeof(Originator<>))]
file class Memento<T>
{
    // Only Originator can create Memento
    public Memento(T state) => State = state;

    // Only Originator can read state
    public T State { get; }
}

file class Originator<T>
{
    public T State { get; set; } = default!;

    // Only Caretaker can create Memento
    [OnlyYou(typeof(Caretaker<>))]
    public Memento<T> Save() => new Memento<T>(State);

    // Only Caretaker can restore Memento
    [OnlyYou(typeof(Caretaker<>))]
    public void Restore(Memento<T> memento) => State = memento.State;
}

// Caretaker can store Memento, but cannot read or modify it
file class Caretaker<T>
{
    private Stack<Memento<T>> _history = new ();
    public void SaveState(Originator<T> originator) => _history.Push(originator.Save());
    public void Undo(Originator<T> originator)
    {
        if (_history.Count > 0)
        {
            var memento = _history.Pop();
            originator.Restore(memento);

            var state = /*EA_TYPE_001*/ memento.State; // Compilation error! Caretaker is not Originator
        }
    }
}

// Usage
class Program
{
    static void Main()
    {
        var origin = new Originator<string>();
        var care = new Caretaker<string>();

        origin.State = "A";
        care.SaveState(origin);
        origin.State = "B";
        care.Undo(origin); // Restore the "A" state

        // But:
        var memento = /*EA_METH_001*/ origin.Save(); // Compilation error!
        var state = /*EA_TYPE_001*/ memento.State; // Compilation error!
        memento = /*EA_TYPE_001*/ new Memento<string>("bad state"); // Compilation error!
    }
}



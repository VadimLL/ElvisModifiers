// Memento pattern

using System.Collections.Generic;

namespace TestAnalyzerLib11;

file class Originator
{
    // Inner class with access only to Originator
    [OnlyYou<Originator>]   
    public class Memento
    {
        // Only Originator can create Memento
        public Memento(string state) => State = state;

        // Only Originator can read state
        public string State { get; }
    }

    public string State { get; set; } = "";

    // Only Caretaker can create Memento
    [OnlyYou<Caretaker>]
    public Memento Save() => new Memento(State);

    // Only Caretaker can restore Memento
    [OnlyYou<Caretaker>]
    public void Restore(Memento memento) => State = memento.State;
}

// Caretaker can store Memento, but cannot read or modify it
file class Caretaker
{
    private Stack<Originator.Memento> _history = new ();
    public void SaveState(Originator originator) => _history.Push(originator.Save());
    public void Undo(Originator originator)
    {
        if (_history.Count > 0)
        {
            var memento = _history.Pop();
            var state = /*EA_TYPE_001*/ memento.State; // Compilation error! Caretaker is not Originator
            originator.Restore(memento);
        }
    }
}

// Usage
file class Program
{
    static void Main()
    {
        var origin = new Originator();
        var care = new Caretaker();

        origin.State = "A";
        care.SaveState(origin);
        origin.State = "B";
        care.Undo(origin); // Restore the "A" state

        // But:
        var memento = /*EA_METH_001*/ origin.Save(); // Compilation error!
        var state = /*EA_TYPE_001*/ memento.State; // Compilation error!
        memento = /*EA_TYPE_001*/ new Originator.Memento("bad state"); // Compilation error!
    }
}

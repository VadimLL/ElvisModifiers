// Builder pattern

using System;
using System.Collections.Generic;

namespace TestAnalyzerLib11;

// Only the builder can set properties
[OnlyYouSet<PizzaBuilder>]
file class Pizza
{
    readonly List<string> _Toppings = new();
    public IEnumerable<string> Toppings => _Toppings;

    // Only the builder can create pizza
    [OnlyYou<PizzaBuilder>]
    public Pizza() { }

    public string Dough { get; set; } = "Default";    
    public string Sauce { get; set; } = "Default";

    // Only the builder can add
    [OnlyYou<PizzaBuilder>]
    public void AddToppings(string topping) => _Toppings.Add(topping);

    public string Description => $"Dough: {Dough}; Sauce: {Sauce};...";
}

//[OnlyYou<PizzaDirector>]
file class PizzaBuilder
{
    Pizza _pizza = new Pizza();

    //[Exclude]
    public PizzaBuilder() { }

    // Only the director can set the ingredients
    [OnlyYou<PizzaDirector>]
    public void SetDough(string dough)
    {
        _pizza.Dough = dough;
    }

    [OnlyYou<PizzaDirector>]
    public void SetSauce(string sauce)
    {
        _pizza.Sauce = sauce;
    }

    [OnlyYou<PizzaDirector>]
    public void AddTopping(string topping)
    {
        _pizza.AddToppings(topping);
    }

    //[Exclude]
    // Available to everyone - you can always get results
    public Pizza Build() => _pizza;
}

// The director is the only one who can manage the builder
file class PizzaDirector
{
    private PizzaBuilder _builder;

    public PizzaDirector(PizzaBuilder builder)
    {
        // The director gets the builder and now only one manages the builder
        _builder = builder;
    }

    public void MakeMargherita()
    {
        // Allowed - we are inside PizzaDirector
        _builder.SetDough("thin");
        _builder.SetSauce("tomato");
        _builder.AddTopping("mozzarella");
        _builder.AddTopping("basil");
    }

    public void MakePepperoni()
    {
        // Allowed
        _builder.SetDough("thick");
        _builder.SetSauce("tomato");
        _builder.AddTopping("pepperoni");
        _builder.AddTopping("cheese");
    }
}

// The client is now protected from accidental errors
file class Client
{
    void OrderPizza()
    {
        var try_pizza = /*EA_TYPE_001*/ new Pizza(); // Compilation error!
        /*EA_TYPE_002*/ try_pizza.Dough = "Some toxic"; // Compilation error!

        var builder = new PizzaBuilder();
        var director = new PizzaDirector(builder);

        director.MakePepperoni();
        var pizza = builder.Build(); // ok

        // But:
        /*EA_METH_001*/ builder.SetDough("Some toxic"); // Compilation error!
        /*EA_METH_001*/ builder.AddTopping("мухамор"); // Compilation error!

        // The client can only get the ready product
        Console.WriteLine($"Got pizza: {pizza.Description}");
        // and get (only for read) properties
        var dough = pizza.Dough; // ok
    }
}
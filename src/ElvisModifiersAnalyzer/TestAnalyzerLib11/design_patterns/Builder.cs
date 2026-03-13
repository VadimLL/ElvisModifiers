// Шаблон Строитель (Builder)

using System;
using System.Collections.Generic;

namespace TestAnalyzerLib11;

// Только строитель может устанавливать свойства
[OnlyYouSet<PizzaBuilder>]
file class Pizza
{
    readonly List<string> _Toppings = new();
    public IEnumerable<string> Toppings => _Toppings;

    // Только строитель может создавать пиццу
    [OnlyYou<PizzaBuilder>]
    public Pizza() { }

    public string Dough { get; set; } = "Default";    
    public string Sauce { get; set; } = "Default";

    // Только строитель может добавлять
    [OnlyYou<PizzaBuilder>]
    public void AddToppings(string topping) => _Toppings.Add(topping);

    public string Description => $"Dough: {Dough}; Sauce: {Sauce};...";
}

//[OnlyYou<PizzaDirector>]
file class PizzaBuilder
{
    Pizza _pizza = new Pizza();

    // Только директор может устанавливать ингредиенты
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
    // Доступен всем - получить результат можно всегда
    public Pizza Build() => _pizza;
}

// Директор - единственный, кто может управлять строителем
file class PizzaDirector
{
    private PizzaBuilder _builder;

    public PizzaDirector(PizzaBuilder builder)
    {
        // Директор получает строителя и теперь только он им управляет
        _builder = builder;
    }

    public void MakeMargherita()
    {
        // РАЗРЕШЕНО - мы внутри PizzaDirector
        _builder.SetDough("thin");
        _builder.SetSauce("tomato");
        _builder.AddTopping("mozzarella");
        _builder.AddTopping("basil");
    }

    public void MakePepperoni()
    {
        // РАЗРЕШЕНО
        _builder.SetDough("thick");
        _builder.SetSauce("tomato");
        _builder.AddTopping("pepperoni");
        _builder.AddTopping("cheese");
    }
}

// Клиент теперь защищён от случайных ошибок
file class Client
{
    void OrderPizza()
    {
        var try_pizza = /*EA_TYPE_001*/ new Pizza(); // ОШИБКА КОМПИЛЯЦИИ!
        /*EA_TYPE_002*/ try_pizza.Dough = "Some toxic"; // ОШИБКА КОМПИЛЯЦИИ!

        var builder = new PizzaBuilder();
        var director = new PizzaDirector(builder);

        director.MakePepperoni();
        var pizza = builder.Build(); // ok

        // но:
        /*EA_METH_001*/ builder.SetDough("Some toxic"); // ОШИБКА КОМПИЛЯЦИИ!
        /*EA_METH_001*/ builder.AddTopping("мухамор"); // ОШИБКА КОМПИЛЯЦИИ!

        // Клиент может только получить готовый продукт
        Console.WriteLine($"Got pizza: {pizza.Description}");
        // и получить (только для чтения) свойства
        var dough = pizza.Dough; // ok
    }
}
/// <summary>
/// Пример решения шаблона проектирования State с модификатором:
/// </summary>

namespace TestAnalyzerLib11;

using System;

// Банковский счет - контекст
class BankAccount
{
    private IAccountState _state;
    public decimal Balance { get; private set; }

    public BankAccount()
    {
        _state = new ActiveState(this);
    }

    // Только классы состояний могут менять состояние счета
    [OnlyYou<IAccountState>]
    public void SetState(IAccountState newState)
    {
        _state = newState;
        Console.WriteLine($"Состояние изменено на {newState.GetType().Name}");
    }

    // Только классы состояний могут изменять баланс
    [OnlyYou<IAccountState>]
    public void UpdateBalance(decimal amount)
    {
        Balance += amount;
    }

    // Публичные методы - интерфейс для клиентов
    public void Deposit(decimal amount) => _state.Deposit(amount);
    public void Withdraw(decimal amount) => _state.Withdraw(amount);
    public void Freeze() => _state.Freeze();
    public void Close() => _state.Close();
}

// Интерфейс состояния
interface IAccountState
{
    void Deposit(decimal amount);
    void Withdraw(decimal amount);
    void Freeze();
    void Close();
}

// Активное состояние
class ActiveState : IAccountState
{
    private BankAccount _account;

    public ActiveState(BankAccount account)
    {
        _account = account;
    }

    public void Deposit(decimal amount)
    {
        // Может обновлять баланс, потому что разрешено модификатором
        _account.UpdateBalance(amount);
        Console.WriteLine($"Внесено {amount}, баланс: {_account.Balance}");
    }

    public void Withdraw(decimal amount) { }

    public void Freeze() { }

    public void Close() { }
}

// Замороженное состояние
class FrozenState : IAccountState
{
    public FrozenState(BankAccount account) { }
    public void Close() { }
    public void Deposit(decimal amount) { }
    public void Freeze() { }
    public void Withdraw(decimal amount) { }
}

// Закрытое состояние
class ClosedState : IAccountState
{
    public void Close() { }
    public void Deposit(decimal amount) { }
    public void Freeze() { }
    public void Withdraw(decimal amount) { }
}

// Контролируемый переход состояний:
class FraudDetectionService
{
    public void CheckForFraud(BankAccount account)
    {
        // ОШИБКА КОМПИЛЯЦИИ! Нельзя напрямую менять состояние
        /*EA_METH_001*/ account.SetState(new FrozenState(account));

        // Только через публичный интерфейс
        account.Freeze(); // Правильно - через бизнес-логику
    }
}

// Безопасное изменение внутренних данных:
class ExternalService
{
    public void ProcessPayment(BankAccount account)
    {
        // ОШИБКА КОМПИЛЯЦИИ! Нельзя напрямую менять баланс
        /*EA_METH_001*/ account.UpdateBalance(1000);

        // Только через методы состояния
        account.Deposit(1000); // Правильно
    }
}


// Сценарий использования:
file class Program
{
    static void Main()
    {
        var account = new BankAccount();

        // Клиентский код - работает с публичным интерфейсом
        account.Deposit(1000); // Внесено 1000, баланс: 1000
        account.Withdraw(500); // Снято 500, баланс: 500

        // Система безопасности решает заморозить счет
        account.Freeze(); // Счет заморожен

        // Попытка снять деньги
        account.Withdraw(100); // Невозможно снять средства - счет заморожен

        // Внести все еще можно
        account.Deposit(200); // Внесено на замороженный счет 200, баланс: 700

        // После разбирательства счет закрывают
        account.Close(); // Счет закрыт

        // Дальнейшие операции невозможны
        account.Withdraw(100); // Невозможно снять средства - счет закрыт

        // НО! Никто не может случайно "разморозить" счет,
        // минуя бизнес-логику
        /*EA_METH_001*/ account.SetState(new ActiveState(account)); // Ошибка компиляции!
    }
}
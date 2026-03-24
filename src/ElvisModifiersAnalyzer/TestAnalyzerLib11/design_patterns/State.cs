// State pattern

namespace TestAnalyzerLib11;

using System;

// Bank account - context
file class BankAccount
{
    private IAccountState _state;
    public decimal Balance { get; private set; }

    public BankAccount()
    {
        _state = new ActiveState(this);
    }

    // Only state classes can change the account state
    [OnlyYou<IAccountState>]
    public void SetState(IAccountState newState)
    {
        _state = newState;
        Console.WriteLine($"Состояние изменено на {newState.GetType().Name}");
    }

    // Only state classes can change the balance
    [OnlyYou<IAccountState>]
    public void UpdateBalance(decimal amount)
    {
        Balance += amount;
    }

    // Public methods - interface for clients
    public void Deposit(decimal amount) => _state.Deposit(amount);
    public void Withdraw(decimal amount) => _state.Withdraw(amount);
    public void Freeze() => _state.Freeze();
    public void Close() => _state.Close();
}

// State interface
file interface IAccountState
{
    void Deposit(decimal amount);
    void Withdraw(decimal amount);
    void Freeze();
    void Close();
}

// Active state
file class ActiveState : IAccountState
{
    private BankAccount _account;

    public ActiveState(BankAccount account)
    {
        _account = account;
    }

    public void Deposit(decimal amount)
    {
        // Can update the balance because it is allowed by the Elvis-modifier
        _account.UpdateBalance(amount);
        Console.WriteLine($"Внесено {amount}, баланс: {_account.Balance}");
    }

    public void Withdraw(decimal amount) { }

    public void Freeze() { }

    public void Close() { }
}

// Frozen state
file class FrozenState : IAccountState
{
    public FrozenState(BankAccount account) { }
    public void Close() { }
    public void Deposit(decimal amount) { }
    public void Freeze() { }
    public void Withdraw(decimal amount) { }
}

// Closed state
file class ClosedState : IAccountState
{
    public void Close() { }
    public void Deposit(decimal amount) { }
    public void Freeze() { }
    public void Withdraw(decimal amount) { }
}

// Controlled state changing:
file class FraudDetectionService
{
    public void CheckForFraud(BankAccount account)
    {
        // Compilation error! You can't change the state directly
        /*EA_METH_001*/ account.SetState(new FrozenState(account));

        // Only via public interface
        account.Freeze(); // That's right - via business logic
    }
}

// Safe modification of internal data:
file class ExternalService
{
    public void ProcessPayment(BankAccount account)
    {
        // Compilation error! You can't change the balance directly
        /*EA_METH_001*/ account.UpdateBalance(1000);

        // Only via state methods
        account.Deposit(1000); // That's right
    }
}


// Usage scenario:
file class Program
{
    static void Main()
    {
        var account = new BankAccount();

        // Client code - works with the public interface
        account.Deposit(1000); // Deposit 1000, balance: 1000
        account.Withdraw(500); // Withdraw 500, balance: 500

        // The security system decides to freeze the account
        account.Freeze(); // Account frozen

        // Trying to withdraw money
        account.Withdraw(100); // It is impossible to withdraw funds - the account is frozen

        // You can still deposit
        account.Deposit(200); // Deposited to frozen account 200, balance: 700

        // After the investigation the account is closed
        account.Close(); // Account closed

        // No further operations are possible
        account.Withdraw(100); // Unable to withdraw funds - account is closed

        // But! No one can accidentally "unfreeze" an account, bypassing the business logic.
        /*EA_METH_001*/ account.SetState(new ActiveState(account)); // Compilation error!
    }
}
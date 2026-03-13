// Шаблон "Фабричный метод" (Factory Method)

using System;

namespace TestAnalyzerLib11;

file abstract class Document
{
    public abstract void Open();
    public abstract void Save();
}

file class TextDocument : Document
{
    public const string Type = nameof(TextDocument);

    // Только фабрика может создавать документы
    //[OnlyYou<DocumentFactory>]
    [OnlyYou(typeof(DocumentFactory))]
    public TextDocument() { }

    public override void Open() => Console.WriteLine("Opening text document");
    public override void Save() => Console.WriteLine("Saving text document");
}

file class SpreadsheetDocument : Document
{
    public const string Type = nameof(SpreadsheetDocument);

    // Только фабрика может создавать документы
    //[OnlyYou<DocumentFactory>]
    [OnlyYou(typeof(DocumentFactory))]
    public SpreadsheetDocument() { }

    public override void Open() => Console.WriteLine("Opening spreadsheet");
    public override void Save() => Console.WriteLine("Saving spreadsheet");
}

// Фабрика - единственная, кто может создавать документы
file class DocumentFactory
{
    public Document CreateDocument(string type)
    {
        return type switch
        {
            TextDocument.Type => new TextDocument(), // РАЗРЕШЕНО
            SpreadsheetDocument.Type => new SpreadsheetDocument(), // РАЗРЕШЕНО
            _ => throw new ArgumentException()
        };
    }
}

// Клиентский код не может создать документ напрямую
file class Client
{
    void Test()
    {
        var doc_try = /*EA_METH_001*/ new TextDocument(); // ОШИБКА! Client не DocumentFactory

        // Правильно:
        var factory = new DocumentFactory();
        var doc = factory.CreateDocument(TextDocument.Type);
    }
}

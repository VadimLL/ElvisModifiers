// "Factory Method" pattern

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

    // Only the factory can create documents
    //[OnlyYou<DocumentFactory>]
    [OnlyYou(typeof(DocumentFactory))]
    public TextDocument() { }

    public override void Open() => Console.WriteLine("Opening text document");
    public override void Save() => Console.WriteLine("Saving text document");
}

file class SpreadsheetDocument : Document
{
    public const string Type = nameof(SpreadsheetDocument);

    // Only the factory can create documents
    //[OnlyYou<DocumentFactory>]
    [OnlyYou(typeof(DocumentFactory))]
    public SpreadsheetDocument() { }

    public override void Open() => Console.WriteLine("Opening spreadsheet");
    public override void Save() => Console.WriteLine("Saving spreadsheet");
}

// The factory is the only one who can create documents
file class DocumentFactory
{
    public Document CreateDocument(string type)
    {
        return type switch
        {
            TextDocument.Type => new TextDocument(), // Allowed
            SpreadsheetDocument.Type => new SpreadsheetDocument(), // Allowed
            _ => throw new ArgumentException()
        };
    }
}

// Client code cannot create document directly
file class Client
{
    void Test()
    {
        var doc_try = /*EA_METH_001*/ new TextDocument(); // Error! Client is not DocumentFactory

        // Correctly:
        var factory = new DocumentFactory();
        var doc = factory.CreateDocument(TextDocument.Type);
    }
}

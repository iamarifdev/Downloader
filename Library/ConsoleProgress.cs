namespace Library;

using System;
using System.IO;

public class ConsoleProgress : IProgress<double>, IDisposable
{
    private readonly TextWriter _writer;
    private readonly int _posX;
    private readonly int _posY;

    private int _lastLength;

    public ConsoleProgress(TextWriter writer)
    {
        _writer = writer;
        _posX = Console.CursorLeft;
        _posY = Console.CursorTop;
    }

    public ConsoleProgress()
        : this(Console.Out)
    {
    }

    private void EraseLast()
    {
        if (_lastLength > 0)
        {
            Console.SetCursorPosition(_posX, _posY);
            _writer.Write(new string(' ', _lastLength));
            Console.SetCursorPosition(_posX, _posY);
        }
    }

    private void Write(string text)
    {
        EraseLast();
        _writer.Write(text);
        _lastLength = text.Length;
    }

    public void Report(double progress) => Console.Write($"\rDownload progress: {progress:P2}".PadRight(20));

    public void Dispose() => EraseLast();
}
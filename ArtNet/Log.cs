// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         Logging.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/13/2024 13:38
//    Created Date:     10/13/2024 13:38
// -----------------------------------------

namespace ArtNet;

using Spectre.Console;
using Spectre.Console.Json;

internal static class Log
{
    /// <summary>
    /// Logs an object in json form.
    /// </summary>
    /// <param name="obj">The object to log</param>
    /// <param name="header">The header message.</param>
    internal static void Object(object obj, string header = "")
    {
        AnsiConsole.Write(
            new Panel(new JsonText(Newtonsoft.Json.JsonConvert.SerializeObject(obj))
                    .BracesColor(Color.Red)
                    .BracketColor(Color.Green)
                    .ColonColor(Color.Blue)
                    .CommaColor(Color.Red)
                    .StringColor(Color.Green)
                    .NumberColor(Color.Blue)
                    .BooleanColor(Color.Red)
                    .NullColor(Color.Green))
                .Header(header)
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Yellow));
    }
    
    /// <summary>
    /// Logs an exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">An optional message before the exception information.</param>
    /// <param name="header">The header of the log.</param>
    /// <param name="panelTitle">The title of the exception panel.</param>
    internal static void Exception(Exception exception,string message = "An exception has been caught.", string header = "ArtNet", string panelTitle = "Artnet Exception")
    {
        var ex = exception.GetRenderable(ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
            ExceptionFormats.ShortenMethods);
        if(message is not "")
            Error(message, header);
        AnsiConsole.Write(
            new Panel(ex)
                .Header(panelTitle)
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Yellow));
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="msg">The message to log.</param>
    /// <param name="canLog">A bool that indicates whether or not the message can be logged.</param>
    /// <param name="header">The header to log.</param>
    internal static void Debug(string msg, bool canLog = true, string header = "ArtNet")
    {
        if(!canLog)
            return;
        AnsiConsole.Markup($"[green][[[/][green bold]Debug[/][green]]] [[[/][green bold]{header}[/][green]]] {msg}[/]\n");
        Server.Logs.Enqueue($"[Debug] [{header}] {msg}"); 
    }

    /// <summary>
    /// Logs an info message.
    /// </summary>
    /// <param name="msg">The info message to log.</param>
    /// <param name="header">The header to log.</param>
    internal static void Info(string msg, string header = "ArtNet")
    {
        AnsiConsole.Markup($"[cyan][[[/][cyan bold]Info [/][cyan]]] [[[/][cyan bold]{header}[/][cyan]]] {msg}[/]\n");
        Server.Logs.Enqueue($"[Info ] [{header}] {msg}");
    }
    
    /// <summary>
    /// Logs a warning.
    /// </summary>
    /// <param name="msg">The warning message to log.</param>
    /// <param name="header">The header to log.</param>
    internal static void Warn(string msg, string header = "ArtNet")
    {
        AnsiConsole.Markup($"[red][[[/][red bold]Warn [/][red]]] [[[/][red bold]{header}[/][red]]] {msg}[/]\n");
        Server.Logs.Enqueue($"[Warn ] [{header}] {msg}");
    }
    
    /// <summary>
    /// Logs an error.
    /// </summary>
    /// <param name="msg">Error message to log.</param>
    /// <param name="header">The header to log.</param>
    internal static void Error(string msg, string header = "ArtNet")
    {
        AnsiConsole.Markup($"[darkred][[[/][darkred bold]Error[/][darkred]]] [[[/][darkred bold]{header}[/][darkred]]][/][darkred italic] {msg}[/]\n");
        Server.Logs.Enqueue($"[Error] [{header}] {msg}"); 
    }
}

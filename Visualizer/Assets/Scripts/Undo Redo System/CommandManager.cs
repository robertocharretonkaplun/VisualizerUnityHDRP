using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager responsable de ejecutar, deshacer y rehacer comandos en el juego.
/// </summary>
public class CommandManager : MonoBehaviour
{
    /// <summary>
    /// Stack que guarda el historial de comandos ejecutados.
    /// </summary>
    private Stack<IAction> historyStack = new Stack<IAction>();
    /// <summary>
    /// Stack que guarda el historial de comandos deshechos para poder rehacerlos.
    /// </summary>
    private Stack<IAction> redoHistoryStack = new Stack<IAction>();
    /// <summary>
    /// Ejecuta un comando y lo agrega al historial.
    /// </summary>
    /// <param name="action">El comando a ejecutar.</param>
    public void ExecuteCommand(IAction action)
    {
        action.ExecuteCommand();
        historyStack.Push(action);
        redoHistoryStack.Clear();
    }

    /// <summary>
    /// Deshace el último comando ejecutado.
    /// </summary>
    public void UndoCommand()
    {
        if (historyStack.Count > 0)
        {
            redoHistoryStack.Push(historyStack.Peek());
            historyStack.Pop().UndoCommand();
        }
    }

    /// <summary>
    /// Rehace el último comando deshecho.
    /// </summary>
    public void RedoCommand()
    {
        if (redoHistoryStack.Count > 0)
        {
            historyStack.Push(redoHistoryStack.Peek());
            redoHistoryStack.Pop().ExecuteCommand();
        }
    }
}

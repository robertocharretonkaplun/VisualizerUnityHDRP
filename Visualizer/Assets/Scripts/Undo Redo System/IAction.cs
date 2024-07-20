using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interfaz que define los métodos para ejecutar y deshacer comandos.
/// </summary>
public interface IAction 
{
    /// <summary>
    /// Método para ejecutar un comando.
    /// </summary>
    void ExecuteCommand();

    /// <summary>
    /// Método para deshacer un comando.
    /// </summary>
    void UndoCommand();
}

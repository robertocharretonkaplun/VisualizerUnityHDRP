using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class Test_Gizmo_Position
{
    private GameObject table;
    private Vector3 originalPosition; 

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        //Load scene "D&D"
        SceneManager.LoadScene("D&D");
        yield return new WaitForSeconds(1f);
        Debug.Log("Escena cargada");

        //Look "table" object
        table = GameObject.Find("Table");
        Assert.IsNotNull(table, "El objeto 'Table' no fue encontrado en la escena");
        Debug.Log("Objeto 'Table' encontrado");

        //Save original rotation
        originalPosition = table.transform.position;
        Debug.Log("Posición original guardada: " + originalPosition);
    }

    [UnityTest]
    public IEnumerator CheckPosition()
    {
        //Wait 5 seconds to move manually
        Debug.Log("Esperando 5 segundos para que puedas mover manualmente el objeto");
        yield return new WaitForSeconds(5f);

        //Get actual position from table
        Vector3 currentPosition = table.transform.position;
        Debug.Log("Posición actual: " + currentPosition);

        //Verify that x axis position is diferrent 
        Assert.AreNotEqual(originalPosition.x, currentPosition.x, "La posición en el eje X no ha cambiado");
        Debug.Log("La posición en el eje X ha cambiado.");

        //Verify that y axis position is diferrent 
        Assert.AreNotEqual(originalPosition.y, currentPosition.y, "La posición en el eje Y no ha cambiado");
        Debug.Log("La posición en el eje Y ha cambiado.");

        //Verify that z axis position is diferrent 
        Assert.AreNotEqual(originalPosition.z, currentPosition.z, "La posición en el eje Z no ha cambiado");
        Debug.Log("La posición en el eje Z ha cambiado");

        yield return null;
    }
}

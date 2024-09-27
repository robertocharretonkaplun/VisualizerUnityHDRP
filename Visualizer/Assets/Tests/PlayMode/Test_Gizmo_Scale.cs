using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class Test_Gizmo_Scale
{
    private GameObject table;
    private Vector3 originalScale; // Guardar la escala original

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

        //Save original scale
        originalScale = table.transform.localScale;
        Debug.Log("Escala original guardada: " + originalScale);
    }

    [UnityTest]
    public IEnumerator CheckScale()
    {
        //Wait 5 seconds to scale manually
        Debug.Log("Esperando 5 segundos para que puedas cambiar manualmente la escala del objeto");
        yield return new WaitForSeconds(5f);

        //Get actual scale from table
        Vector3 currentScale = table.transform.localScale;
        Debug.Log("Escala actual: " + currentScale);

        //Verify that x axis scale is diferrent 
        Assert.AreNotEqual(originalScale.x, currentScale.x, "La escala en el eje X no ha cambiado");
        Debug.Log("La escala en el eje X ha cambiado.");

        //Verify that y axis scale is diferrent 
        Assert.AreNotEqual(originalScale.y, currentScale.y, "La escala en el eje Y no ha cambiado");
        Debug.Log("La escala en el eje Y ha cambiado.");

        //Verify that z axis scale is diferrent 
        Assert.AreNotEqual(originalScale.z, currentScale.z, "La escala en el eje Z no ha cambiado");
        Debug.Log("La escala en el eje Z ha cambiado");

        yield return null;
    }
}

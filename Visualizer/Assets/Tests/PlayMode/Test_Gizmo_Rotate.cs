using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class Test_Gizmo_Rotate
{
    private GameObject table;
    private Quaternion originalRotation; 

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
        Debug.Log("Objeto encontrado");

        //Save original rotation
        originalRotation = table.transform.rotation;
        Debug.Log("Rotación original guardada: " + originalRotation.eulerAngles);
    }

    [UnityTest]
    public IEnumerator CheckRotation()
    {
        //Wait 5 seconds to rotate manually
        Debug.Log("Esperando 5 segundos para que puedas rotar manualmente el objeto");
        yield return new WaitForSeconds(5f);

        //Get actual rotation from table
        Vector3 currentRotation = table.transform.rotation.eulerAngles;
        Debug.Log("Rotación actual: " + currentRotation);

        //Verify that x axis rotation is diferrent 
        Assert.AreNotEqual(originalRotation.x, currentRotation.x, "La rotación en el eje X no ha cambiado");
        Debug.Log("La rotación en el eje X ha cambiado.");

        //Verify that y axis rotation is diferrent 
        Assert.AreNotEqual(originalRotation.y, currentRotation.y, "La rotación en el eje Y no ha cambiado");
        Debug.Log("La rotación en el eje Y ha cambiado.");

        //Verify that z axis rotation is diferrent 
        Assert.AreNotEqual(originalRotation.z, currentRotation.z, "La rotación en el eje Z no ha cambiado");
        Debug.Log("La rotación en el eje Z ha cambiado");

        yield return null;
    }
}

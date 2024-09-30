using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

//Class to change material to object with DragAndDrop
public class DragAndDropTest
{
    private GameObject table;
    private Renderer[] childRenderers;
    private Material[] originalMaterials;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        //Load scene "D&D"
        SceneManager.LoadScene("D&D");
        yield return new WaitForSeconds(1f);
        Debug.Log("Escena cargada");

        //Look table
        table = GameObject.Find("Table");
        Assert.IsNotNull(table, "El objeto 'Table' no fue encontrado en la escena");
        Debug.Log("Objeto encontrado");

        //Get renderer components
        childRenderers = table.GetComponentsInChildren<Renderer>();
        Assert.IsTrue(childRenderers.Length > 0, "No se encontraron Renderers en los hijos del objeto 'Table'");
        Debug.Log("Renderers de hijos obtenidos");

        //Save original materials to compare with new materials
        originalMaterials = new Material[childRenderers.Length];
        for (int i = 0; i < childRenderers.Length; i++)
        {
            originalMaterials[i] = childRenderers[i].material;
        }
        Debug.Log("Materiales originales guardados");
    }

    //Method to check reference material when I change it
    [UnityTest]
    public IEnumerator DragAndDropMaterialToGameObject()
    {
        //Wait 5 seconds to change material
        Debug.Log("Esperando 5 segundos para que puedas cambiar manualmente el material");
        yield return new WaitForSeconds(5f);

        //Check if material is change into children renderer objetcts
        for (int i = 0; i < childRenderers.Length; i++)
        {
            Renderer renderer = childRenderers[i];
            Material originalMaterial = originalMaterials[i];

            //Check to material has changed
            Assert.AreNotEqual(originalMaterial, renderer.material, "El material no cambió en uno de los hijos de 'Table'");
            Debug.Log("Material cambiado correctamente en el Renderer de un hijo");
        }
    }
}

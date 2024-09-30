using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEditor;

//Class to comprobe to panels are a correctly activation and desactivation
public class UI_Panel_Test
{
    //Need reference the panels and buttons
    public GameObject[] panels;
    public Button button;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Create panels
        panels = new GameObject[3];
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i] = new GameObject($"Panel{i}");
            panels[i].SetActive(false);
        }

        // Create button and add component
        var buttonObject = new GameObject("Button");
        button = buttonObject.AddComponent<Button>();

        // Config event onClick to desactivate this panel and activate others
        button.onClick.AddListener(() =>
        {
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].SetActive(i == 1);
            }
        });

        yield return null; // Wait to complete the configuration
    }

    //Method to off panels when i open other panel
    [UnityTest]
    public IEnumerator On_Off_Panels_UI()
    {
        // Simulate the onClick function
        button.onClick.Invoke();

        // Wait a frame to the logic has a really function
        yield return null;

        // Check that only the second panel has activate
        Assert.IsFalse(panels[0].activeSelf);
        Assert.IsTrue(panels[1].activeSelf);
        Assert.IsFalse(panels[2].activeSelf);

        yield return null; // End of the test
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Clear objects created in the test
        foreach (var panel in panels)
        {
            Object.Destroy(panel);
        }
        Object.Destroy(button.gameObject);

        yield return null; // Wait to complete the task
    }


}

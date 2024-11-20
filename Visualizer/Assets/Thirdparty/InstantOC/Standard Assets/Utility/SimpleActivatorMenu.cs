using System;
using UnityEngine;
using UnityEngine.UI; // Asegúrate de agregar esta línea

namespace UnityStandardAssets.Utility
{
    public class SimpleActivatorMenu : MonoBehaviour
    {
        // Un menú increíblemente simple que, al recibir referencias
        // a gameobjects en la escena
        public Text camSwitchButton; // Cambiado a Text
        public GameObject[] objects;


        private int m_CurrentActiveObject;


        private void OnEnable()
        {
            // El objeto activo comienza desde el primero en el array
            m_CurrentActiveObject = 0;
            camSwitchButton.text = objects[m_CurrentActiveObject].name; // Cambiado a Text
        }


        public void NextCamera()
        {
            int nextactiveobject = m_CurrentActiveObject + 1 >= objects.Length ? 0 : m_CurrentActiveObject + 1;

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(i == nextactiveobject);
            }

            m_CurrentActiveObject = nextactiveobject;
            camSwitchButton.text = objects[m_CurrentActiveObject].name; // Cambiado a Text
        }
    }
}

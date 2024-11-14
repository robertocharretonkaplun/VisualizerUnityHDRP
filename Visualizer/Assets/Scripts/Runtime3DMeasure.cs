using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Measure
{
    public class Runtime3DMeasure : MonoBehaviour
    {
        //Prefab to instance points
        [SerializeField] private GameObject pointPrefab;
        //Prefab to instance line renderer
        [SerializeField] private GameObject linePrefab;
        //Prefab to text instance
        [SerializeField] private TextMesh distanceTextPrefab;  

        //List of all points generated
        private List<GameObject> points = new List<GameObject>();
        //List of all lines generated
        private List<GameObject> lines = new List<GameObject>();

        //Controla si estamos en modo puntos o modo objeto
        private bool isPointMode = true;
        [SerializeField ]private bool canToggleMode = true;

        //Materiales para cada eje
        [SerializeField] private Material materialX;
        [SerializeField] private Material materialY;
        [SerializeField] private Material materialZ;

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && canToggleMode)
            {
                isPointMode = !isPointMode;
                canToggleMode = false;
                Debug.Log(isPointMode ? "Modo de Puntos Activado" : "Modo de Objeto Activado");
            }

            // Permite el cambio de modo solo cuando la tecla se suelta
            if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftShift))
            {
                canToggleMode = true;
            }

            if (isPointMode)
            {
                PointMode();
            }
            else
            {
                ObjectMode();
            }

            // Detectar Shift Izquierdo + C para borrar todos los puntos y líneas
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
            {
                ClearAllPointsAndLines();
            }
        }

        private void PointMode()
        {
            //Click and control detection to instance points
            if (Input.GetMouseButtonDown(0) && (Input.GetKey(KeyCode.LeftControl)))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    AddPoint(hit.point);
                }
            }
        }

        private void ObjectMode()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider != null)
                    {
                        GenerateObjectMeasurements(hit.collider.gameObject);
                    }
                }
            }
        }

        private void AddPoint(Vector3 position)
        {
            //Instance new point on click position
            GameObject newPoint = Instantiate(pointPrefab, position, Quaternion.identity);

            //Conect new point between all points
            foreach (var point in points)
            {
                DrawHitLine(newPoint, point);
            }

            //Add new point to list
            points.Add(newPoint);
        }

        private void DrawHitLine(GameObject pointA, GameObject pointB)
        {
            //Create line between pointA and pointB
            GameObject line = Instantiate(linePrefab);
            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, pointA.transform.position);
            lineRenderer.SetPosition(1, pointB.transform.position);

            //Add line ti list of lines
            lines.Add(line);

            //Calculate distance between points
            float distance = Vector3.Distance(pointA.transform.position, pointB.transform.position);

            //Pos text in the middle of the line
            Vector3 midPoint = (pointA.transform.position + pointB.transform.position) / 2;
            //Move little bit up text to give us a better vision to the text
            midPoint += Vector3.up * 0.1f;
            TextMesh distanceText = Instantiate(distanceTextPrefab, midPoint, Quaternion.identity, line.transform);
            Debug.Log("Text puesto");
            distanceText.text = $"{distance:F2} m";
        }

        private void DrawObjectLine(Vector3 start, Vector3 end, string label)
        {
            //Create line between pointA and pointB
            GameObject line = Instantiate(linePrefab);
            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            // Asignar el material según el eje predominante
            Material lineMaterial = GetMaterialByAxis(end - start);
            lineRenderer.material = lineMaterial;

            //Add line ti list of lines
            lines.Add(line);

            //Calculate distance between points
            float distance = Vector3.Distance(start, end);

            //Pos text in the middle of the line
            Vector3 midPoint = (start + end) / 2;
            //Move little bit up text to give us a better vision to the text
            midPoint += Vector3.up * 0.1f;
            TextMesh distanceText = Instantiate(distanceTextPrefab, midPoint, Quaternion.identity, line.transform);
            Debug.Log("Text puesto");
            distanceText.text = $"{label}: {distance:F2} m";

            // Cambiar el tamaño del texto
            distanceText.characterSize = 0.5f; // Ajusta el valor para el tamaño deseado
            distanceText.fontSize = 300; // Ajusta el tamaño de fuente

            // Cambiar el color del texto según el eje predominante
            Color textColor = GetTextColorByAxis(end - start);
            distanceText.color = textColor;
        }

        private void ClearAllPointsAndLines()
        {
            //Destroy all points and clear list
            foreach (var point in points)
            {
                Destroy(point);
            }
            points.Clear();

            //Destroy lines and clear list
            foreach (var line in lines)
            {
                Destroy(line);
            }
            lines.Clear();
        }

        private void GenerateObjectMeasurements(GameObject selectedObject)
        {
            // Obtén todos los MeshRenderers en los hijos del objeto seleccionado
            MeshRenderer[] meshRenderers = selectedObject.GetComponentsInChildren<MeshRenderer>();

            if (meshRenderers.Length == 0)
            {
                Debug.LogWarning("No se encontraron MeshRenderers en el objeto seleccionado.");
                return;
            }

            // Calcula los límites globales del objeto a partir de todos los MeshRenderers
            Bounds bounds = meshRenderers[0].bounds;
            foreach (var renderer in meshRenderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            Vector3 bottomFrontLeft = bounds.min;
            Vector3 topFrontLeft = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
            Vector3 bottomFrontRight = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
            Vector3 bottomBackLeft = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);

            DrawObjectLine(bottomFrontLeft, topFrontLeft, "Y");
            DrawObjectLine(bottomFrontLeft, bottomFrontRight, "X");
            DrawObjectLine(bottomFrontLeft, bottomBackLeft, "Z");
        }

        private Material GetMaterialByAxis(Vector3 direction)
        {
            // Determina el material según el eje predominante
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) && Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                return materialX; // Eje X
            }
            else if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x) && Mathf.Abs(direction.y) > Mathf.Abs(direction.z))
            {
                return materialY; // Eje Y
            }
            else
            {
                return materialZ; // Eje Z
            }
        }

        private Color GetTextColorByAxis(Vector3 direction)
        {
            // Determina el color del texto según el eje predominante
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) && Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                return Color.red; // Eje X
            }
            else if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x) && Mathf.Abs(direction.y) > Mathf.Abs(direction.z))
            {
                return Color.green; // Eje Y
            }
            else
            {
                return Color.blue; // Eje Z
            }
        }

    }
}

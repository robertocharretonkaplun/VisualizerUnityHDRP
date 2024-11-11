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
        [SerializeField] private TMP_Text distanceTextPrefab;  

        //List of all points generated
        private List<GameObject> points = new List<GameObject>();
        //List of all lines generated
        private List<GameObject> lines = new List<GameObject>(); 


        private void Update()
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

            // Detectar Shift Izquierdo + C para borrar todos los puntos y líneas
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
            {
                ClearAllPointsAndLines();
            }
        }

        private void AddPoint(Vector3 position)
        {
            //Instance new point on click position
            GameObject newPoint = Instantiate(pointPrefab, position, Quaternion.identity);

            //Conect new point between all points
            foreach (var point in points)
            {
                DrawLine(newPoint, point);
            }

            //Add new point to list
            points.Add(newPoint);
        }

        private void DrawLine(GameObject pointA, GameObject pointB)
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
            TMP_Text distanceText = Instantiate(distanceTextPrefab, midPoint, Quaternion.identity, line.transform);
            Debug.Log("Text puesto");
            distanceText.text = $"{distance:F2} m";
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
    }
}

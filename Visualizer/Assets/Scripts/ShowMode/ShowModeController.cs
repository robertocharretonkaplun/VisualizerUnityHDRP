using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is control the play/pause/stop button
/// </summary>
public class ShowModeController : MonoBehaviour
{
    public CameraWaypointMove CameraWaypointMoveRef;
    /// <summary>
    /// initialize the timescale to one to avoid bugs
    /// </summary>
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Only need fixed update to debug timescale
    /// </summary>
    private void FixedUpdate()
    {
        if (Time.timeScale > 0f)
        {
            Debug.Log("Animacion");
        }
        else if (Time.timeScale < 1f)
        {
            Debug.Log("Pausa");
        }

    }

    /// <summary>
    /// This void will be linked to the play button (In the future this function realize the animation with the waypoints)
    /// </summary>
    public void Play()
    {
        Time.timeScale = 1.0f;
        CameraWaypointMoveRef.StartMoving();
    }

    /// <summary>
    /// This void will be linked to the pause button (In the future this function pause the animation with the waypoints)
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// This void will be linked to the stop button (In the future this function stop the animation with the waypoints)
    /// </summary>
    public void StopButton()
    {

    }
}

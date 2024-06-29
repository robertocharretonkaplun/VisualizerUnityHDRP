//MIT License
//Copyright (c) 2023 DA LAB (https://www.youtube.com/@DA-LAB)
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using UnityEngine;
using TMPro;

public class TransformUI : MonoBehaviour
{
    public TMP_InputField inputPosX;
    public TMP_InputField inputPosY;
    public TMP_InputField inputPosZ;
    public TMP_InputField inputRotationX;
    public TMP_InputField inputRotationY;
    public TMP_InputField inputRotationZ;
    public TMP_InputField inputScaleX;
    public TMP_InputField inputScaleY;
    public TMP_InputField inputScaleZ;
    public SelectTransformGizmo selectTransformGizmo;

    private Transform selectedTransform;
    private Vector3 lastPosition;
    private Vector3 lastRotation;
    private Vector3 lastScale;

    void Start()
    {
        // Subscribe to input field value changed events
        inputPosX.onValueChanged.AddListener(delegate { UpdatePosition(); });
        inputPosY.onValueChanged.AddListener(delegate { UpdatePosition(); });
        inputPosZ.onValueChanged.AddListener(delegate { UpdatePosition(); });

        inputRotationX.onValueChanged.AddListener(delegate { UpdateRotation(); });
        inputRotationY.onValueChanged.AddListener(delegate { UpdateRotation(); });
        inputRotationZ.onValueChanged.AddListener(delegate { UpdateRotation(); });

        inputScaleX.onValueChanged.AddListener(delegate { UpdateScale(); });
        inputScaleY.onValueChanged.AddListener(delegate { UpdateScale(); });
        inputScaleZ.onValueChanged.AddListener(delegate { UpdateScale(); });

        // Subscribe to selection change event
        selectTransformGizmo.OnSelectionChanged += UpdateUI;
    }
    

    void OnDestroy()
    {
        // Unsubscribe from events
        inputPosX.onValueChanged.RemoveAllListeners();
        inputPosY.onValueChanged.RemoveAllListeners();
        inputPosZ.onValueChanged.RemoveAllListeners();

        inputRotationX.onValueChanged.RemoveAllListeners();
        inputRotationY.onValueChanged.RemoveAllListeners();
        inputRotationZ.onValueChanged.RemoveAllListeners();

        inputScaleX.onValueChanged.RemoveAllListeners();
        inputScaleY.onValueChanged.RemoveAllListeners();
        inputScaleZ.onValueChanged.RemoveAllListeners();

        selectTransformGizmo.OnSelectionChanged -= UpdateUI;
    }

    void Update()
    {
        // Check if selected transform has changed
        if (selectedTransform != null)
        {
            // Check if position has changed
            if (selectedTransform.position != lastPosition)
            {
                UpdatePositionUI();
                lastPosition = selectedTransform.position;
            }

            // Check if rotation has changed
            if (selectedTransform.eulerAngles != lastRotation)
            {
                UpdateRotationUI();
                lastRotation = selectedTransform.eulerAngles;
            }

            // Check if scale has changed
            if (selectedTransform.localScale != lastScale)
            {
                UpdateScaleUI();
                lastScale = selectedTransform.localScale;
            }
        }
    }

    void UpdateUI(Transform newSelection)
    {
        selectedTransform = newSelection;
        if (selectedTransform != null)
        {
            lastPosition = selectedTransform.position;
            lastRotation = selectedTransform.eulerAngles;
            lastScale = selectedTransform.localScale;

            UpdatePositionUI();
            UpdateRotationUI();
            UpdateScaleUI();
        }
        else
        {
            ClearUI();
        }
    }

    void ClearUI()
    {
        inputPosX.text = "";
        inputPosY.text = "";
        inputPosZ.text = "";
        inputRotationX.text = "";
        inputRotationY.text = "";
        inputRotationZ.text = "";
        inputScaleX.text = "";
        inputScaleY.text = "";
        inputScaleZ.text = "";
    }

    void UpdatePositionUI()
    {
        Vector3 pos = selectedTransform.position;
        inputPosX.text = pos.x.ToString("F3");
        inputPosY.text = pos.y.ToString("F3");
        inputPosZ.text = pos.z.ToString("F3");
    }

    void UpdateRotationUI()
    {
        Vector3 rot = selectedTransform.eulerAngles;
        inputRotationX.text = rot.x.ToString("F3");
        inputRotationY.text = rot.y.ToString("F3");
        inputRotationZ.text = rot.z.ToString("F3");
    }

    void UpdateScaleUI()
    {
        Vector3 scale = selectedTransform.localScale;
        inputScaleX.text = scale.x.ToString("F3");
        inputScaleY.text = scale.y.ToString("F3");
        inputScaleZ.text = scale.z.ToString("F3");
    }

    void UpdatePosition()
    {
        if (selectedTransform != null)
        {
            Vector3 pos = selectedTransform.position;
            if (float.TryParse(inputPosX.text, out float newX))
                pos.x = newX;
            if (float.TryParse(inputPosY.text, out float newY))
                pos.y = newY;
            if (float.TryParse(inputPosZ.text, out float newZ))
                pos.z = newZ;
            selectedTransform.position = pos;
        }
    }

    void UpdateRotation()
    {
        if (selectedTransform != null)
        {
            Vector3 rot = selectedTransform.eulerAngles;
            if (float.TryParse(inputRotationX.text, out float newX))
                rot.x = newX;
            if (float.TryParse(inputRotationY.text, out float newY))
                rot.y = newY;
            if (float.TryParse(inputRotationZ.text, out float newZ))
                rot.z = newZ;
            selectedTransform.eulerAngles = rot;
        }
    }

    void UpdateScale()
    {
        if (selectedTransform != null)
        {
            Vector3 scale = selectedTransform.localScale;
            if (float.TryParse(inputScaleX.text, out float newX))
                scale.x = newX;
            if (float.TryParse(inputScaleY.text, out float newY))
                scale.y = newY;
            if (float.TryParse(inputScaleZ.text, out float newZ))
                scale.z = newZ;
            selectedTransform.localScale = scale;
        }
    }
}

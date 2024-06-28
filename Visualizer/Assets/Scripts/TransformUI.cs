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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    private void Start()
    {
        // Add listeners for input fields
        inputPosX.onEndEdit.AddListener(delegate { SetPosX(); });
        inputPosY.onEndEdit.AddListener(delegate { SetPosY(); });
        inputPosZ.onEndEdit.AddListener(delegate { SetPosZ(); });
        inputRotationX.onEndEdit.AddListener(delegate { SetRotationX(); });
        inputRotationY.onEndEdit.AddListener(delegate { SetRotationY(); });
        inputRotationZ.onEndEdit.AddListener(delegate { SetRotationZ(); });
        inputScaleX.onEndEdit.AddListener(delegate { SetScaleX(); });
        inputScaleY.onEndEdit.AddListener(delegate { SetScaleY(); });
        inputScaleZ.onEndEdit.AddListener(delegate { SetScaleZ(); });

        selectTransformGizmo.OnSelectionChanged += UpdateUI;
    }

    private void OnDestroy()
    {
        selectTransformGizmo.OnSelectionChanged -= UpdateUI;
    }

    private void UpdateUI(Transform selected)
    {
        selectedTransform = selected;

        if (selectedTransform != null)
        {
            UpdatePosUI();
            UpdateRotationUI();
            UpdateScaleUI();
        }
        else
        {
            ClearUI();
        }
    }

    private void ClearUI()
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

    // Position
    private void UpdatePosUI()
    {
        Vector3 currPos = selectedTransform.position;
        inputPosX.text = currPos.x.ToString("F3");
        inputPosY.text = currPos.y.ToString("F3");
        inputPosZ.text = currPos.z.ToString("F3");
    }

    public void SetPosX()
    {
        if (float.TryParse(inputPosX.text, out float posX) && selectedTransform != null)
        {
            Vector3 pos = selectedTransform.position;
            pos.x = posX;
            selectedTransform.position = pos;
        }
    }

    public void SetPosY()
    {
        if (float.TryParse(inputPosY.text, out float posY) && selectedTransform != null)
        {
            Vector3 pos = selectedTransform.position;
            pos.y = posY;
            selectedTransform.position = pos;
        }
    }

    public void SetPosZ()
    {
        if (float.TryParse(inputPosZ.text, out float posZ) && selectedTransform != null)
        {
            Vector3 pos = selectedTransform.position;
            pos.z = posZ;
            selectedTransform.position = pos;
        }
    }

    // Rotation
    private void UpdateRotationUI()
    {
        Vector3 currRotation = selectedTransform.eulerAngles;
        inputRotationX.text = currRotation.x.ToString("F3");
        inputRotationY.text = currRotation.y.ToString("F3");
        inputRotationZ.text = currRotation.z.ToString("F3");
    }

    public void SetRotationX()
    {
        if (float.TryParse(inputRotationX.text, out float rotationX) && selectedTransform != null)
        {
            Vector3 rotation = selectedTransform.eulerAngles;
            rotation.x = rotationX;
            selectedTransform.eulerAngles = rotation;
        }
    }

    public void SetRotationY()
    {
        if (float.TryParse(inputRotationY.text, out float rotationY) && selectedTransform != null)
        {
            Vector3 rotation = selectedTransform.eulerAngles;
            rotation.y = rotationY;
            selectedTransform.eulerAngles = rotation;
        }
    }

    public void SetRotationZ()
    {
        if (float.TryParse(inputRotationZ.text, out float rotationZ) && selectedTransform != null)
        {
            Vector3 rotation = selectedTransform.eulerAngles;
            rotation.z = rotationZ;
            selectedTransform.eulerAngles = rotation;
        }
    }

    // Scale
    private void UpdateScaleUI()
    {
        Vector3 currScale = selectedTransform.localScale;
        inputScaleX.text = currScale.x.ToString("F3");
        inputScaleY.text = currScale.y.ToString("F3");
        inputScaleZ.text = currScale.z.ToString("F3");
    }

    public void SetScaleX()
    {
        if (float.TryParse(inputScaleX.text, out float scaleX) && selectedTransform != null)
        {
            Vector3 scale = selectedTransform.localScale;
            scale.x = scaleX;
            selectedTransform.localScale = scale;
        }
    }

    public void SetScaleY()
    {
        if (float.TryParse(inputScaleY.text, out float scaleY) && selectedTransform != null)
        {
            Vector3 scale = selectedTransform.localScale;
            scale.y = scaleY;
            selectedTransform.localScale = scale;
        }
    }

    public void SetScaleZ()
    {
        if (float.TryParse(inputScaleZ.text, out float scaleZ) && selectedTransform != null)
        {
            Vector3 scale = selectedTransform.localScale;
            scale.z = scaleZ;
            selectedTransform.localScale = scale;
        }
    }
}

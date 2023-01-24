using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class OptionsScreen : MonoBehaviour
{
    [FormerlySerializedAs("FullscreenToggle")] public Toggle fullscreenToggle;
    [FormerlySerializedAs("VsyncToggle")] public Toggle vsyncToggle;
    [FormerlySerializedAs("ResolutionText")] public TMP_Text resolutionText;
    [FormerlySerializedAs("Resolutions")] public List<ResItem> resolutions = new List<ResItem>();
    private int _selectedRes;
    
    

    private void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        if (QualitySettings.vSyncCount == 0)
        {
            vsyncToggle.isOn = false;
        }
        else
        {
            vsyncToggle.isOn = true;
        }
        bool foundRes = false;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                
                _selectedRes = i;
                foundRes = true;
                UpdateResolutionLabel();
            }
        }
        if (!foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.width = Screen.width;
            newRes.height = Screen.height;
            resolutions.Add(newRes);
            _selectedRes = resolutions.Count - 1;
            UpdateResolutionLabel();
        }
    }
    public void ResLeft()
    {
        _selectedRes--;
        if (_selectedRes < 0)
        {
            _selectedRes = 0;
        }
        UpdateResolutionLabel();
    }
    public void ResRight() 
    {
        _selectedRes++;
        if (_selectedRes > resolutions.Count - 1)
        {
            _selectedRes = resolutions.Count - 1;
        }
        UpdateResolutionLabel();

    }
    public void UpdateResolutionLabel() 
    {
        resolutionText.text = resolutions[_selectedRes].width.ToString() + " x " + resolutions[_selectedRes].height.ToString();
    }
        public void ApplyGraphic()
        {
            if (vsyncToggle.isOn)
            {
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
            }

        Screen.SetResolution(resolutions[_selectedRes].width, resolutions[_selectedRes].height, fullscreenToggle.isOn);
        }

    
}
[System.Serializable]
public class ResItem {

    [FormerlySerializedAs("Width")] public int width;
    [FormerlySerializedAs("Height")] public int height;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsScreen : MonoBehaviour
{
    public Toggle FullscreenToggle;
    public Toggle VsyncToggle;
    public TMP_Text ResolutionText;
    public List<ResItem> Resolutions = new List<ResItem>();
    private int selectedRes;
    
    

    private void Start()
    {
        FullscreenToggle.isOn = Screen.fullScreen;
        if (QualitySettings.vSyncCount == 0)
        {
            VsyncToggle.isOn = false;
        }
        else
        {
            VsyncToggle.isOn = true;
        }
        bool foundRes = false;
        for (int i = 0; i < Resolutions.Count; i++)
        {
            if (Resolutions[i].Width == Screen.width && Resolutions[i].Height == Screen.height)
            {
                
                selectedRes = i;
                foundRes = true;
                UpdateResolutionLabel();
            }
        }
        if (!foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.Width = Screen.width;
            newRes.Height = Screen.height;
            Resolutions.Add(newRes);
            selectedRes = Resolutions.Count - 1;
            UpdateResolutionLabel();
        }
    }
    public void ResLeft()
    {
        selectedRes--;
        if (selectedRes < 0)
        {
            selectedRes = 0;
        }
        UpdateResolutionLabel();
    }
    public void ResRight() 
    {
        selectedRes++;
        if (selectedRes > Resolutions.Count - 1)
        {
            selectedRes = Resolutions.Count - 1;
        }
        UpdateResolutionLabel();

    }
    public void UpdateResolutionLabel() 
    {
        ResolutionText.text = Resolutions[selectedRes].Width.ToString() + " x " + Resolutions[selectedRes].Height.ToString();
    }
        public void ApplyGraphic()
        {
            if (VsyncToggle.isOn)
            {
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
            }

        Screen.SetResolution(Resolutions[selectedRes].Width, Resolutions[selectedRes].Height, FullscreenToggle.isOn);
        }

    
}
[System.Serializable]
public class ResItem {

    public int Width;
    public int Height;
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CameraSystem : MonoBehaviour
{
    [Header("Cameras")]
    public MapNodeVision[] cameraObjects;
    public UnityEvent onCameraChanged;
    public string cameraChangeSound;
    [Space]
    public Sprite activeCameraButton;
    public Sprite inactiveCameraButton;

    private int currentCameraIndex;

    private Image selectedCameraImage;

    void Start()
    {
        ShowCamera(0);

        StartCoroutine(IAnimateCurrentCameraButton());
    }

    public void ShowCamera(string roomName)
    {
        for(int i = 0; i < cameraObjects.Length; i++)
        {
            if (cameraObjects[i].name == roomName)
                currentCameraIndex = i;

            cameraObjects[i].gameObject.SetActive(cameraObjects[i].name == roomName);
        }

        SoundEffects.Play(cameraChangeSound);
        onCameraChanged.Invoke();
    }

    public void ShowCamera(int index)
    {
        if (index >= cameraObjects.Length)
            return;

        for(int i = 0; i < cameraObjects.Length; i++)
        {
            cameraObjects[i].gameObject.SetActive(i == index);
        }

        currentCameraIndex = index;

        SoundEffects.Play(cameraChangeSound);
        onCameraChanged.Invoke();
    }

    public void NextCamera()
    {
        currentCameraIndex++;

        if (currentCameraIndex >= cameraObjects.Length)
            currentCameraIndex = 0;

        ShowCamera(currentCameraIndex);
    }

    public void PreviousCamera()
    {
        currentCameraIndex--;

        if (currentCameraIndex < 0)
            currentCameraIndex = cameraObjects.Length - 1;

        ShowCamera(currentCameraIndex);
    }

    public void CameraButtonPressed(Image img)
    {
        if (selectedCameraImage != null)
        {
            selectedCameraImage.sprite = inactiveCameraButton;
        }

        selectedCameraImage = img;
    }

    private IEnumerator IAnimateCurrentCameraButton()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (selectedCameraImage != null)
            {
                selectedCameraImage.sprite = activeCameraButton;
            }

            yield return new WaitForSeconds(1f);

            if (selectedCameraImage != null)
            {
                selectedCameraImage.sprite = inactiveCameraButton;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    public Slider loadingSlider; // Reference to the slider UI component

    void Start()
    {

        EventHandler.Instance.OnTowersLoadedEvent += SetSlider;
        EventHandler.Instance.OnEnemiesLoadedEvent += SetSlider;


        // Start a coroutine that waits a small amount of time before setting the slider value
        StartCoroutine(SetSliderValueWithDelay(0.34f, 0.1f)); // 0.3 value, 0.1 seconds delay
    }

    IEnumerator SetSliderValueWithDelay(float targetValue, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Set the slider's value after the delay
        loadingSlider.value += targetValue;

    }

    public void SetSlider()
    {
        SetSliderValue(0.32f);
    }

    public void SetSliderValue(float targetValue)
    {
        loadingSlider.value += targetValue;
    }



}

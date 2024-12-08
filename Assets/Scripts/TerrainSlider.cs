using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainSlider : MonoBehaviour
{
    public List<Slider> bucati = new List<Slider>();
    public float duration = 3f;
    void Start()
    {
        for (int i = 0; i < bucati.Count; i++)
        {
            bucati[i].minValue = 0;
            bucati[i].maxValue = 100;
            bucati[i].value = 0;
        }
        StartCoroutine(StartRolling());
    }

    IEnumerator StartRolling()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            for (int i = 0; i < bucati.Count; i++)
            {
                bucati[i].value = Mathf.Lerp(0, 100, elapsed / duration);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < bucati.Count; i++)
        {
            bucati[i].value = 100;
            
            bucati[i].transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

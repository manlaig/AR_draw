using UnityEngine;
using UnityEngine.UI;

public class BrushEditor : MonoBehaviour
{
    [SerializeField] LineRenderer line;

    public void ChangeWidth(Slider slider)
    {
        line.startWidth = slider.value;
        line.endWidth = slider.value;
    }

    public void ChangeMaterial(Material material)
    {
        if (material.name == "rainbow")
        {
            line.startColor = Color.white;
            line.endColor = Color.white;
        }
        else if(line.sharedMaterial.name == "rainbow")
        {
            Color current = GameObject.Find("ColorSelect").GetComponent<ColorSelector>().GetCurrentColor();
            line.startColor = current;
            line.endColor = current;
        }
        line.material = material;
    }

    public void ToggleEditor(GameObject go)
    {
        go.SetActive(!go.activeInHierarchy);
    }
}

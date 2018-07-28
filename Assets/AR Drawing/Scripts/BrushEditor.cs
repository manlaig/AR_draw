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

    public void ToggleEditor(GameObject go)
    {
        go.SetActive(!go.activeInHierarchy);
    }
}

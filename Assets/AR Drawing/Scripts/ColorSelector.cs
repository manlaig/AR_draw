using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    [SerializeField] Image colorIcon;
    //[SerializeField] Material lineColor;

	void Start ()
    {
        colorIcon.color = line.endColor;
	}

    public void ChangeColor(Button button)
    {
        //Debug.Log("Color.r: " + r + " Color.g: " + g + " Color.b: " + b);
        colorIcon.color = button.colors.disabledColor;
        line.startColor = colorIcon.color;
        line.endColor = colorIcon.color;
        //line.material.color = colorIcon.color;
        //line.sharedMaterial.color = colorIcon.color;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    [SerializeField] Image colorIcon, smallWidth, bigWidth;

	void Start ()
    {
        colorIcon.color = line.endColor;
	}

    public void ChangeColor(Button button)
    {
        line.startColor = button.colors.disabledColor;
        line.endColor = button.colors.disabledColor;

        if(colorIcon != null)
            colorIcon.color = button.colors.disabledColor;

        if (smallWidth != null && bigWidth != null)
            smallWidth.color = bigWidth.color = button.colors.disabledColor;
    }

    public void ToggleColorSelector()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}

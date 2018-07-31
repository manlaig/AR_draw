using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    [SerializeField] Image colorIcon, smallWidth, bigWidth,
                     standardBrush, streamBrush, brush;

	void Start ()
    {
        if(line != null)
            SetColors(line.endColor);
	}

    public void ChangeColor(Button button)
    {
        if (line != null)
        {
            line.startColor = button.colors.disabledColor;
            line.endColor = button.colors.disabledColor;
        }
        SetColors(button.colors.disabledColor);
    }

    void SetColors(Color color)
    {
        if (colorIcon != null)
            colorIcon.color = color;

        if (standardBrush != null)
            standardBrush.color = color;

        if (streamBrush != null)
            streamBrush.color = color;

        if (brush != null)
            brush.color = color;

        if (smallWidth != null && bigWidth != null)
            smallWidth.color = bigWidth.color = color;
    }

    public Color GetCurrentColor()
    {
        return colorIcon.color;
    }
}

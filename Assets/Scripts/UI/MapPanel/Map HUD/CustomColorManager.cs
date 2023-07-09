using UnityEngine;
using UnityEngine.UI;

public class CustomColorManager : MonoBehaviour
{
    [SerializeField] Image[] boundaries;

    public void SetBoundaryColor(string hexColor) {

        Color newCol;

        ColorUtility.TryParseHtmlString(hexColor, out newCol);
        if (newCol == null) return;

        foreach (Image im in boundaries) {
            im.color = newCol;
        }
    }


}

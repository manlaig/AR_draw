using UnityEngine;

// stores all material needed to draw a line
public class AllMaterials : MonoBehaviour
{
	public Material[] materials;

	public int GetCorrespondingIndex(Material m)
	{
		if(materials != null)
		{
			for(int i = 0; i < materials.Length; i++)
				if(m.name == materials[i].name)
					return i;
		}
		return 0;
	}
}

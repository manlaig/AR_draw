using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSO : ScriptableObject
{
    public Material[] materials;

    public int GetCorrespondingIndex(Material m)
	{
		if(materials != null)
		{
			for(int i = 0; i < materials.Length; i++)
				if(m.name.Equals(materials[i].name))
					return i;
		}
		return 0;
	}
}

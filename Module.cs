using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
	private ModuleData myData;
	public ModuleData Data { get { return myData; } set { myData = value; } }
}

using UnityEngine;
using UnityEditor;
using System.Collections;

public class ScriptableWizardDisplayWizard : ScriptableWizard {
	
	public GameObject ObjectToCopy = null;
	public int numberOfCopies = 2;
	[MenuItem ("ShadowGame/Show DisplayWizard usage")]
	static void CreateWindow() {
		// Creates the wizard for display
		//创建显示向导
		ScriptableWizard.DisplayWizard("Copy an object.",
		                               typeof(ScriptableWizardDisplayWizard),
		                               "Copy!");
	}
	void OnWizardUpdate() {
		helpString = "Clones an object a number of times";
		if(!ObjectToCopy) {
			errorString = "Please assign an object";
			isValid = false;
		} else {
			errorString = "";
			isValid = true;
		}
	}
	void OnWizardCreate () {
		for(int i = 0; i < numberOfCopies; i++)
			Instantiate(ObjectToCopy, Vector3.zero, Quaternion.identity);
	}
}
using UnityEngine;
using System.Collections;

public class SlidingPuzzleGrid : MonoBehaviour {
	
	// Awake is being called before Start; this makes sure we have a matrix to begin with when we add the blocks
	void Awake() {
		// because of how we wrote the accessor this will also immediately build the matrix of our level
		SlidingPuzzleExample.mainGrid = gameObject.GetComponent<GFRectGrid>();
	}
	
	// visualizes the matrix in text form to let you see what's going on
	void OnGUI(){
		GUI.TextArea (new Rect (10, 10, 250, 150), SlidingPuzzleExample.MatrixToString());
	}
}
using UnityEngine;
using System.Collections;

public enum SceneElementType
{
      MAP,
      NPC,
      BUILDING,
      TREE,
      Floors,
};

public class SceneEditorEnum
{
   public static float[][] colorArray = new float[][]
   {
      new float[]{255,255,255,255},
      new float[]{255,255,255,255},
      new float[]{255,255,255,255},
      new float[]{255,255,255,255},
      new float[]{238,153,26,255},
   };

   public static bool[] isHaveColor = new bool[]{false,false,false,false,true,};

}
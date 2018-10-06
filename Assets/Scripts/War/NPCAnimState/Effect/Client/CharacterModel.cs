using UnityEngine;
using System.Collections;

public enum ModelDef{
    Root,
    Head,
    Left_Hand,
    Right_Hand,
    Chest,
    Fore_Chest,
    Left_Foot,
    Right_Foot,
    None
}


[System.Serializable]
public class CharacterModel {

    /// <summary>
    /// The transform.
    /// </summary>
    private Transform tran;
    /// <summary>
    /// The root.
    /// </summary>
    private Transform Root;
    /// <summary>
    /// The head.
    /// </summary>
    private Transform Head;
    /// <summary>
    /// The left hand.
    /// </summary>
    private Transform Left_Hand;
    /// <summary>
    /// The right hand.
    /// </summary>
    private Transform Right_Hand;
    /// <summary>
    /// The chest.
    /// </summary>
    private Transform Chest;
    /// <summary>
    /// The fore chest.
    /// </summary>
    private Transform Fore_Chest;
    /// <summary>
    /// The left foot.
    /// </summary>
    private Transform Left_Foot;
    /// <summary>
    /// The right foot.
    /// </summary>
    private Transform Right_Foot;
	
    public void Init(Transform _tran)
    { 
        tran = _tran;
        Root = tran;
        Head = Root.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head/Head");
        Left_Hand = Root.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/Left_Hand");
        Right_Hand = Root.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Right_Hand");
        Chest = Root.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Chest");
        Fore_Chest = Root.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Fore_Chest");
        Left_Foot = Root.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 L Thigh/Bip001 L Calf/Bip001 L Foot/Left_Foot");
        Right_Foot = Root.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 R Thigh/Bip001 R Calf/Bip001 R Foot/Right_Foot");
    }

    public Transform GetModelPart(ModelDef def)
    {
        switch(def)
        {
            case ModelDef.Head:
                return Head;
            case ModelDef.Left_Hand:
                return Left_Hand;
            case ModelDef.Right_Hand:
                return Right_Hand;
            case ModelDef.Chest:
                return Chest;
            case ModelDef.Fore_Chest:
                return Fore_Chest;
            case ModelDef.Left_Foot:
                return Left_Foot;
            case ModelDef.Right_Foot:
                return Right_Foot;
            default:
                return tran;
        }
    }

}

using System;

/**
 * All Const vars. should be defined here !!
 */ 
public sealed class Consts {

	public const bool FAILURE = false;
	public const bool OK = true;

    //User Data should be encrpty
    // password for AES
    public const string sharedSecret = "IWLLDOGOD";
	
    public const float oneHundred = 0.01f;
	public const float OneThousand = 0.001f;

	//攻击是3次
	public const short NORMAL_ATTACK_TIMES = 3;

	//技能有3个，特殊的技能就一个
	public const short MAX_SKILL_COUNT = 4;

	//有效时间
	public const float ATTACK_VALIDATE = 1F;
	//读取BUFF配表里的时间
	public const float USE_BUFF_CONFIG_DURATION = -1F;

	//目标超时两秒
	public const float Target_TimeOut = 2F;

    //非建筑的NPC的layers
    public const string LAYER_NPC = "NPC";        
    //建筑的layer
    public const string LAYER_BUILD = "Building"; 
}


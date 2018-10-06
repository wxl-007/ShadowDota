using System;
using AW.Resources;

public class AudioLoader : RefManager<int> {
    #if UNITY_IPHONE
    private const int CAPACITY = 20;
    private const int MAX = 10;

    #elif UNITY_ANDROID
    private const int CAPACITY = 30;
    private const int MAX = 10;

    #elif UNITY_WP8
    private const int CAPACITY = 30;
    private const int MAX = 10;

    #else
    private const int CAPACITY = 50;
    private const int MAX = 30;

    #endif

    public AudioLoader () : base(CAPACITY, MAX) { }

	protected override int RefAsset (string name) {
		return 0;
    }

}

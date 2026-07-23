using UnityEngine;

[System.Serializable]
public struct AppData
{
    public AppType appType;
    public Sprite iconSprite;
    public bool isInstalled;
    public string appName;

    public float priceUSD;
    public float priceBTC;
}

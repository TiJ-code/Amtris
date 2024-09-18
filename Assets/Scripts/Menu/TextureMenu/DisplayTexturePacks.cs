using System.IO;
using UnityEngine;

public class DisplayTexturePacks : MonoBehaviour
{
    public GameObject texturePackPrefab;
    public Transform texturePackParent;

    public static DisplayTexturePacks instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } 
        else
        {
            Destroy(this);
        }
    }

    public void Display()
    {
        ApplyTexturePacks.instance.LoadResourcePack();

        string[] directories = Directory.GetDirectories(Infrastructure.usagePath);

        foreach (string directory in directories)
        {
            GameObject instance = Instantiate(texturePackPrefab, texturePackParent);
            instance.name = Path.GetFileName(directory);
            TexturePack_Element texElement = instance.GetComponent<TexturePack_Element>();
            texElement.SetValues(instance.name, Path.Combine(Infrastructure.usagePath, instance.name));
            texElement.Setup();

            ApplyTexturePacks.instance.elements.Add(texElement);
        }
        ApplyTexturePacks.instance.FixLoadedUi();

        ApplyTexturePacks.instance.ApplyOnClickListener();
    }
}

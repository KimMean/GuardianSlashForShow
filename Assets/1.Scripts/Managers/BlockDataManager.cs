using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockDataManager : MonoBehaviour
{
    private static BlockDataManager instance;
    public static BlockDataManager Instance
    {
        get
        {
            if(instance == null)
                instance = (BlockDataManager)FindObjectOfType(typeof(BlockDataManager));
            return instance;
        }
    }

    string blockFilePath = "Blocks/";
    string blockPrefixName = "Stage";

    [SerializeField] Sprite[] BlockTextures;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        BlockTextures = new Sprite[GameManager.Instance.GetMaxStageCount()];
        LoadBlockTextures();
    }


    public Sprite GetStageBlockSprite(int stage)
    {
        return BlockTextures[stage];
    }

    private void LoadBlockTextures()
    {
        BlockTextures = Resources.LoadAll<Sprite>(blockFilePath);
    }
}

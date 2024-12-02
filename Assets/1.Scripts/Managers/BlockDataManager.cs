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

    string BlockFilePath = "Blocks/";
    string BlockPrefixName = "Stage";

    [SerializeField] Sprite[] BlockTextures;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        BlockTextures = new Sprite[GameManager.Instance.GetMaxStageCount()];
        LoadBlockTextures();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite GetStageBlockSprite(int stage)
    {
        return BlockTextures[stage];
    }

    private void LoadBlockTextures()
    {
        //BlockTextures[0] = Resources.Load<Sprite>(BlockFilePath +  BlockPrefixName + "01");
        //Debug.Log(BlockFilePath + BlockPrefixName + "01");
        BlockTextures = Resources.LoadAll<Sprite>(BlockFilePath);
    }
}

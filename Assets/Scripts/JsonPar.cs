using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InData{
        public int coloum, row;
        public int spldotperct;
}

public class JsonPar : MonoBehaviour
{

    
    public InData inData = new InData();
    public static JsonPar jsonPar;
    // Start is called before the first frame update
    void Start()
    {
        jsonPar = this;
        
        TextAsset asset = Resources.Load("match3jason") as TextAsset;

        if (asset != null)
        {
            inData = JsonUtility.FromJson<InData>(asset.text);
            print(inData.coloum);

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

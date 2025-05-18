using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlyerHp : MonoBehaviour
{
    [SerializeField] GameObject HpImagePrefab;
    [SerializeField] GameObject HpImagePrefab1;
    [SerializeField] GameObject HpImagePrefab2;

    List<GameObject> HpImages;
    public float HPspace = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        HpImages = new List<GameObject>();

        Vector3 vRoot = transform.position;
        for ( int i=0; i<10; i++ ) 
        {
            //GameObject newHpImage = Instantiate(HpImagePrefab,this.transform);
            if(i%2 == 1)
            {
                GameObject newHpImage = Instantiate(HpImagePrefab1, this.transform);
                newHpImage.transform.position = vRoot + new Vector3(i * HPspace, 0.0f, 0.0f);
                HpImages.Add(newHpImage);
            }
            else
            {
                GameObject newHpImage = Instantiate(HpImagePrefab2, this.transform);
                newHpImage.transform.position = vRoot + new Vector3(i * HPspace + 1.0f, 0.0f, 0.0f);
                HpImages.Add(newHpImage);
            }
            //newHpImage.transform.position = vRoot + new Vector3(i*HPspace,0.0f,0.0f);  
            //HpImages.Add(newHpImage);
        }

        SetHp(6);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHp(int hp)
    {
        for(int i=0; i< HpImages.Count; i++)
        {
            HpImages[i].SetActive( i < hp );
        }
    }

}

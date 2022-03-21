using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Wood : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField] Transform wood;
    // Start is called before the first frame update
    void Start()
    {
        wood.DOLocalRotate(new Vector3(-360,0,0),1f,RotateMode.FastBeyond360).SetLoops(-1,LoopType.Restart).SetEase(Ease.Linear);
    }
    public void HitMesh(int index, float damage)
    {
        float newWeight = skinnedMeshRenderer.GetBlendShapeWeight(index)+(damage*(100f/2));
        skinnedMeshRenderer.SetBlendShapeWeight(index,newWeight);
    }
}

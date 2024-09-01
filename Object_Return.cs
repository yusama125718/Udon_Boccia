
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Object_Return : UdonSharpBehaviour
{
    [Header("Target Objects")]
    [SerializeField] private GameObject[] targets = null;

    private Vector3[] positions = null;

    // 初期位置を保存
    void Start()
    {
        positions = new Vector3[targets.Length];
        for (int i = 0; i < targets.Length; i++){
            positions[i] = targets[i].transform.position;
        }
    }

    // ポジションを初期位置に
    void Interact(){
        for (int i = 0; i < targets.Length; i++){
            targets[i].transform.position = positions[i];
        }
    }
}

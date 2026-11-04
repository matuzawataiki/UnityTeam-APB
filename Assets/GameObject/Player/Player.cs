using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rigidbody_;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody_ = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision col)
    {
        // 衝突した相手のオブジェクト名を表示
        Debug.Log("衝突したオブジェクト: " + col.gameObject.name);
    }
}

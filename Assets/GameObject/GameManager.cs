using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static Vector3[] potisionOffsets = new Vector3[] {
        new Vector3(25.0f,3.0f,-18.0f),
        new Vector3(18.0f,3.0f,0.0f),
        new Vector3(25.0f,3.0f,18.0f),
        new Vector3(-25.0f,3.0f,-18.0f),
        new Vector3(-18.0f,3.0f,0.0f),
        new Vector3(-25.0f,3.0f,18.0f)
    };

    [SerializeField] GameObject[] players_ = new GameObject[6];
    [SerializeField] GameObject stage_;

    GameObject selectPlayer_;

    Plane plane_;

    float tensileForce_;
    Vector3 forceDirectoin_;

    enum EnGameManagerState
    {
        enSelectState,
        enPullState,
        enRollingState
    }

    EnGameManagerState state_;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state_ = EnGameManagerState.enSelectState;

        plane_ = new Plane(Vector3.up, new Vector3(0, 2, 0));

        Instantiate(stage_);

        Vector3 position = new Vector3(18.0f, 3.0f, 0.0f);
        Quaternion rotation = Quaternion.identity;

        for (int i = 0; i < players_.Length; i++)
        {
            if (i == players_.Length / 2)
            {
                rotation = Quaternion.AngleAxis(180.0f, Vector3.up);
            }
            Instantiate(players_[i], potisionOffsets[i], rotation);


        }
    }

        // Update is called once per frame
        void Update()
    {
        switch (state_)
        {
            case EnGameManagerState.enSelectState:
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    Debug.DrawRay(ray.origin, ray.direction * 300, Color.green, 3, true);

                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 300.0f, LayerMask.GetMask("Player")))
                    {
                        Debug.Log("キャッチテスト");
                        if (!hit.collider.gameObject.CompareTag("Coin")){
                            Debug.Log("プレイヤーでない");
                            break;
                        }
                        Debug.Log("選択");
                        selectPlayer_ = hit.collider.gameObject;
                        state_ = EnGameManagerState.enPullState;
                    }
                    else
                    {
                        Debug.Log("当たらなかった");
                    }
                }

                break;

            case EnGameManagerState.enPullState:
                if (Input.GetMouseButton(0))
                {
                    Debug.Log("選択中");
                    Vector3 playerPosition = selectPlayer_.transform.position;
                    
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    float distance;
                    plane_.Raycast(ray, out distance);
                    Vector3 mouseWorldPos = ray.GetPoint(distance);
                    Vector3 mouseToPlayer = playerPosition - mouseWorldPos;
                    mouseToPlayer.y = 0.0f;

                    tensileForce_ = Mathf.Min(mouseToPlayer.magnitude, 30);
                    forceDirectoin_ = mouseToPlayer.normalized;

                    //Debug.DrawRay(mouseWorldPos, mouseToPlayer, Color.green, 0.1f, true);
                    Debug.DrawRay(playerPosition, forceDirectoin_ * 7, Color.red, 0.1f, true);
                    Debug.DrawRay(playerPosition, forceDirectoin_ * -1 * tensileForce_, Color.green, 0.1f, true);


                    Debug.Log(tensileForce_);
                }
                else
                {
                    if(tensileForce_ > 5)
                    {
                        state_ = EnGameManagerState.enRollingState;
                        Rigidbody rigidbody = selectPlayer_.transform.parent.GetComponent<Rigidbody>();

                        rigidbody.AddForce(forceDirectoin_ * tensileForce_ * 200);

                        Debug.Log("発射");
                    }
                    else
                    {
                        state_ = EnGameManagerState.enSelectState;
                        selectPlayer_ = null;
                        Debug.Log("選択解除");
                    }
                }

                break;

            case EnGameManagerState.enRollingState:

                state_ = EnGameManagerState.enSelectState;

                break;

        }

    }
}

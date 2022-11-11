using System.Collections;
using UnityEngine;


public class Character : UObject
{
    public Tile cur_Position;

    [Header("캐릭터 그라운드 체크")]
    public float hOffset = -0.5f;
    public bool isGround;
    public float radius = 0.5f;
    public LayerMask groundLayer;

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.position + new Vector3(0, hOffset, 0));
        RaycastHit result;
        if (Physics.Raycast(ray, out result,radius,groundLayer))
        {
            cur_Position = result.collider.GetComponent<Tile>();
            cur_Position.SetAboveObject(this);
            isGround = true;
        }
        else
        {
            cur_Position = null;
            isGround = false;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, hOffset, 0));

    }

}
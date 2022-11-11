using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stack형으로 게임컨트롤 상태를 관리한다.
/// </summary>
public class InGameController : MonoBehaviour
{
    public enum Control_State
    { }
    public Stack<Control_State> Stack_controlState = new Stack<Control_State>();


    public Character cur_Control_Target;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SetToPathFinding();
        }
    }

    private void SetToPathFinding()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Debug.Log("찾았다.");
            //var list = PathFinder.GetSinglePath(cur_Control_Target, hit.collider.GetComponent<Tile>());
            //hit.collider.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
}
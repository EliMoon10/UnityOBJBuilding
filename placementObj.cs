using UnityEngine;
using System.Collections;
using System;

public class placementObj : MonoBehaviour
{

    [Tooltip("이 오브젝트는 설치될겁니다!")]
    public GameObject prefabPlacementObject;
    [Tooltip("오브젝트가 설치될 수 있으면 이 오브젝트로 변합니다!(색상)")]
    public GameObject prefabOK;
    [Tooltip("오브젝트가 설치할 수 없다면 이 오브젝트로 변합니다ㅠㅠ(색상)")]
    public GameObject prefabFail;

    [Tooltip("스냅 그리드의 크기입니다. 이 수치가 적으면 오브젝트 끼리 딱 붙습니다...")]
    public float grid = 2.0f;


    [Tooltip("raycast를 위한 레이어 마스크입니다!")]
    public LayerMask mask = -1;


    int[,] used;

    private GameObject placementObject = null;
    private GameObject areaObject = null;

    private Bounds placementBounds;

    bool mouseClick = false;
    Vector3 lastPos;

    // Use this for initialization
    void Start()
    {
        //터레인 데이터 

        if (GetComponent<Terrain>() != null)
        {
            placementBounds = GetComponent<Terrain>().terrainData.bounds;
        }
        else if (GetComponent<Renderer>() != null)
        {
            placementBounds = GetComponent<Renderer>().bounds;
        }

        Vector3 slots = placementBounds.size / grid;
        used = new int[Mathf.CeilToInt(slots.x), Mathf.CeilToInt(slots.z)];
        for (var x = 0; x < Mathf.CeilToInt(slots.x); x++)
        {
            for (var z = 0; z < Mathf.CeilToInt(slots.z); z++)
            {
                used[x, z] = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 point;


        if (getTargetLocation(out point))
        {

            Vector3 halfSlots = placementBounds.size / 2.0f;
            // 그리드로 포지션 계산
            int x = (int)Math.Round(Math.Round(point.x - transform.position.x + halfSlots.x - grid / 2.0f) / grid);
            int z = (int)Math.Round(Math.Round(point.z - transform.position.z + halfSlots.z - grid / 2.0f) / grid);

            point.x = (float)(x) * grid - halfSlots.x + transform.position.x + grid / 2.0f;
            point.z = (float)(z) * grid - halfSlots.z + transform.position.z + grid / 2.0f;

            if (lastPos.x != x || lastPos.z != z || areaObject == null)
            {
                lastPos.x = x;
                lastPos.z = z;
                if (areaObject != null)
                {
                    Destroy(areaObject);
                }
                areaObject = (GameObject)Instantiate(used[x, z] == 0 ? prefabOK : prefabFail, point, Quaternion.identity);
            }

            if (!placementObject)
            {
                placementObject = (GameObject)Instantiate(prefabPlacementObject, point, Quaternion.identity);
            }
            else
            {
                placementObject.transform.position = point;
            }

            if (Input.GetMouseButtonDown(0) && mouseClick == false)
            {
                mouseClick = true;

                if (used[x, z] == 0)
                {
                    used[x, z] = 1;
                    Instantiate(prefabPlacementObject, point, Quaternion.identity);
                }
            }
            else if (!Input.GetMouseButtonDown(0))
            {
                mouseClick = false;
            }
        }
        else
        {
            if (placementObject)
            {
                Destroy(placementObject);
                placementObject = null;
            }
            if (areaObject)
            {
                Destroy(areaObject);
                areaObject = null;
            }
        }
    }

    bool getTargetLocation(out Vector3 point)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, mask))
        {
            if (hitInfo.collider == GetComponent<Collider>())
            {
                point = hitInfo.point;
                return true;
            }
        }
        point = Vector3.zero;
        return false;
    }
}

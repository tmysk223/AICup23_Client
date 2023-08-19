using ForceDirectedGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class MapMaker : MonoBehaviour
{
    private enum MapMode
    {
        mode2D,
        mode3D,
    }

    [SerializeField] MapMode mode = MapMode.mode3D;

    [SerializeField][TextArea] string testJson;
    [SerializeField] GraphManager3D graphManager;
    [SerializeField] GraphManager graphManager2D;

    // Start is called before the first frame update
    void Start()
    {
        CreateMap();
    }

    void CreateMap()
    {

        string mapJson = GameManager.Instance == null ? testJson : GameManager.Instance.getMap();
        MapUtility.Map map = MapUtility.Deserialize(mapJson);

        switch (mode)
        {
            case MapMode.mode2D:
                graphManager2D.Initialize(MapUtility.ConvertToGraph(map));
                break;
            case MapMode.mode3D:
                graphManager.Initialize(MapUtility.ConvertToGraph(map));
                break;
        }
    }
}
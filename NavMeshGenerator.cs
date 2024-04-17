using NavMeshPlus.Components;
using UnityEngine;

public class NavMeshGenerator : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface navMeshBaker; // Reference to the NavMeshSurface component

    void Start()
    {
        if (navMeshBaker != null)
        {
            navMeshBaker.BuildNavMesh();
            Debug.Log("NavMesh Baking Complete!");
        }
        else
        {
            Debug.LogError("RuntimeNavMeshBaker reference not set in NavMeshGenerator script!");

        }
    }
}

using System.Collections;
using UnityEngine;

public class CityBlock : MonoBehaviour
{
    /// <summary>
    /// This is the reference to the main class
    /// </summary>
    private CityGen _manager;

    /// <summary>
    /// All below are the spawing points
    /// measure the area with Cube default size, 1x1 is = to unity's default cube size
    /// </summary>
    public Transform[] SpawnPoints1x1;
    public Transform[] SpawnPoints2x2;
    public Transform[] SpawnPoints3x3;
    public Transform[] SpawnPoints4x4;

    public void GenerateBlock(CityGen _ref)
    {
        //Assinging the reference to private variable for using ahead
        _manager = _ref;

        //Generating random models on each spawn point available in above arrays with desired size
        foreach (var point in SpawnPoints1x1)
        {
            //Getting a 1x1 random model from main class, and same goes for next loops too
            var model = _manager.models1x1[Random.Range(0, _manager.models1x1.Length)];
            Instantiate(model, point.position, point.rotation, transform);

        }

        foreach (var point in SpawnPoints2x2)
        {
            var model = _manager.models2x2[Random.Range(0, _manager.models2x2.Length)];
            Instantiate(model, point.position, point.rotation, transform);
        }

        foreach (var point in SpawnPoints3x3)
        {
            var model = _manager.models3x3[Random.Range(0, _manager.models3x3.Length)];
            Instantiate(model, point.position, point.rotation, transform);
        }

        foreach (var point in SpawnPoints4x4)
        {
            var model = _manager.models4x4[Random.Range(0, _manager.models4x4.Length)];
            Instantiate(model, point.position, point.rotation, transform);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CityGen : MonoBehaviour
{
    [Header("Streets Infrastructure")]
    /// <summary>
    /// Street blocks these are the blocks contains spawning points of objects as per sizes
    /// </summary>
    public GameObject[] StreetBlocks;
    //Adding space to unity's inspector
    [Space(10)]
    /// <summary>
    /// This is the street crossing block which connects streets to each other
    /// </summary>
    public GameObject StreetBlockCross;
    /// <summary>
    /// The chance of street connections higher value generates many connections
    /// </summary>
    public float ChanceOfCross = 0.2f;
    [Header("Models to Generate")]
    /// <summary>
    /// These objects can be trees and pole or any other standing object
    /// </summary>
    public GameObject[] models1x1;
    /// <summary>
    /// These are props its covered area is 2x2 but make sure the spawn point do not overlaps each other
    /// </summary>
    public GameObject[] models2x2;
    /// <summary>
    /// Same as above but these are like buildings the area of these objects is 3x3 
    /// but make sure there should be a gap, the object should cover only 2x2
    /// </summary>
    public GameObject[] models3x3;
    /// <summary>
    /// Same as above mentioned
    /// </summary>
    public GameObject[] models4x4;

    [Header("Sizes")]
    /// <summary>
    /// This is city grid 
    /// </summary>
    public int CitySizeX = 10;
    public int CitySizeZ = 10;
    //Adding space to unity's inspector
    [Space(10)]
    /// <summary>
    /// This is block size
    /// </summary>
    public int BlockSizeX = 10;
    public int BlockSizeZ = 10;
    /// <summary>
    /// The grid of city, contains games objects(blocks) which are generated,
    /// </summary>
    private GameObject[,] BlocksGrid;

    public void GenerateCity()
    {
        //if there are previously generated objects
        //first we will remove them to re-instantiate new city
        if(transform.childCount > 0)
        {
            int count = transform.childCount;
            for(int i = 0; i < count; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        //Creating new grid for city
        BlocksGrid = new GameObject[CitySizeX, CitySizeZ];
        //Main loop1
        for (int x = 0; x < CitySizeX; x++)
        {
            for (int z = 0; z < CitySizeZ; z++)
            {
                //Converting the block size for Transformation of Blocks
                float _x = x * BlockSizeX;
                float _z = z * BlockSizeZ;
                //Creating final position for block
                var _pos = new Vector3(_x, 0, _z);
                //Empty game object it is used to carry the newly spawned block for other functionality
                GameObject _SpawnedBlock = null;
                //Checking the chance of street crosses in this loop, it will run on each z Axis loop
                if (Random.value < ChanceOfCross)
                {
                    //if this time the loop is creating the crossing block, instantiating cross block and assingin to the var
                    _SpawnedBlock = Instantiate(StreetBlockCross, _pos, Quaternion.identity, transform);
                    //Turning the direction of crossing block to the right side by default
                    //it can be on left side but we will go as we made the block prefab containing road on the left side
                    //so it must be turn to the opposite direction on initial spawning
                    //we will see the needed direction in the next main loop to change it accordingly
                    _SpawnedBlock.transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    //if this loop does not creating the street crossing we will add a random default block
                    _SpawnedBlock = Instantiate(StreetBlocks[Random.Range(0, StreetBlocks.Length)], _pos, Quaternion.identity, transform);
                }
                //checking in case of null reference we will stop the process
                if (_SpawnedBlock != null)
                {
                    //Getting the Block class from the generated block and generating its objects
                    var _block = _SpawnedBlock.GetComponent<CityBlock>();
                    //Check if we missed to add the script on block object
                    if (_block != null)
                    {
                        //Sending the call to current generated block for instantiating of objects
                        _block.GenerateBlock(this);
                    }
                    else
                    {
                        //print the reference of loop to see what going on here
                        print("The spawned block does not contains any class - in the loop of X=" + x + " and Z=" + z);
                    }
                }
                else
                {
                    //print the reference of loop to see what going on here
                    print("The spawned block is null in the loop of X=" + x + " and Z=" + z);
                }

                //Assingin the generated block GameObject to the BlocksGrid as per the indexes
                //it will be re-used in next main loop to check the street crosses and their directions
                BlocksGrid[x, z] = _SpawnedBlock;
            }
        }
        //Main loop2
        //This loop is for checking the street crosses
        //We will add more crosses and also we are about to deleting the wrong placed default blocks
        //Wrong placed is like there is a cross in the previous street
        //we will remove the default block from opposite side
        //And then place a new crossing block with the opposite direction to connect with each other
        for (int x = 0; x < CitySizeX; x++)
        {
            for (int z = 0; z < CitySizeZ; z++)
            {
                //Converting the block size for Transformation of Blocks
                float _x = x * BlockSizeX;
                float _z = z * BlockSizeZ;
                var _pos = new Vector3(_x, 0, _z);
                //First of all we will skip the first Street because it is already in the right direction
                if(x > 0)
                {
                    //Creating another x-Variable and setting it to the previous index of the current street
                    var x2 = x - 1;
                    //Checking if the previous block on same index is a street crossing block
                    //the most important part of this logic is a renaming of default blocks and crossing block
                    //The crossing block's name must contains a word (cross)
                    if (BlocksGrid[x2, z].name.ToLower().Contains("cross"))
                    {
                        //if we get in by checking the previous index
                        //now its time to verify that current street is having non-crossing block on the same index
                        //we are going to delete this and we will instantiate new crossing block
                        if (BlocksGrid[x, z].name.ToLower().Contains("cross") == false)
                        {
                            //Destroying the old default block
                            Destroy(BlocksGrid[x, z]);
                            //Creating the new Block which is crossing block to connect with the previous street
                            BlocksGrid[x, z] = Instantiate(StreetBlockCross, _pos, Quaternion.identity, transform);
                            //Getting the CityBlock script to generate objects in newly instantiated block
                            BlocksGrid[x, z].GetComponent<CityBlock>().GenerateBlock(this);
                            //renaming this block to the modified because in the next street
                            //it will make collision between other crossing object
                            //and that is resulting in a new crossing block which will be extra and that is not needed too
                            BlocksGrid[x, z].name = "modified";
                        }
                        else
                        {
                            //if we get the already created block is a crossing block on the same index as in the previous street
                            //we will just change the direction
                            BlocksGrid[x, z].transform.eulerAngles = Vector3.zero;
                            //again modifying the name to avoid further errors in infrastructure
                            BlocksGrid[x, z].name = "modified";
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// it will be added to the Unity'ui Slider for auto value
    /// </summary>
    public void OnStreetCrossingChance(float value)
    {
        ChanceOfCross = value;
    }
    
}

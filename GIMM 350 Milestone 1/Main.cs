﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour {
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies; //array of enemy prefabs
    public float enemySpawnPerSecond = 0.5f; //# Enemies/second
    public float enemyDefaultPadding = 1.5f; //Padding for position
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[]
    {
        WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield, WeaponType.laser
    };
    public Text scoreText;  //Make a public Text game object named score text and link it in the inspector
    public int totalScore = 0; //Create a public int that keeps track of the total score

    private BoundsCheck bndCheck;

    public void ShipDestroyed(Enemy e)
    {
        //Potentially generate a PowerUp
        if (Random.value <= e.powerUpDropChance)
        {
            //Choose which PowerUp to pick
            //Pick one from the possibilities in powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            //Spawn a PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            //Set it to the proper WeaponType
            pu.SetType(puType);

            //Set it to the position of the destroyed ship
            pu.transform.position = e.transform.position;
        }
    }

    //New method that will display the score on the score text
    public void ScoreUp()
    {
        scoreText.text = "Score: " + totalScore;
    }

    void Awake()
    {
        S = this;
        //Set bndCheck to reference the BoundsCheck componenet on this GameObject
        bndCheck = GetComponent<BoundsCheck>();

        //Invoke SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        //A generic Dictionary with WeaponType as the key
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();

        foreach(WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        //Pick a random enemy prefab to instantiate
        int ndx = Random.Range(0,prefabEnemies.Length);
        GameObject go = Instantiate(prefabEnemies[ ndx ]) as GameObject;

        //Position the Enemy above the screen with a random x position
        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        //Set the initial position for the spawned enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        //Invoke SpawnEnemy() again
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart(float delay)
    {
        //Invoke the Restart() method in delay seconds
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        //Reload _Scene_0 to restart the game
        SceneManager.LoadScene("_Scene_0");
    }

    ///<summary>
    ///Static function that gets a WeaponDefinition from the WEAP_DICT static
    ///protected field fo the Main class.
    /// </summary>
    /// <returns>
    /// The WeaponDefinition or, if there is no WeaponDefinition with
    /// the WeaponType passed in, returns a new WeaponDefinition with a
    /// WeaponType of none.
    /// </returns>
    /// <param name="wt">The WeaponType of the desired WeaponDefinition</param>
    
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        //Check to make sure that the key exist in the dictionary
        //Attempting to retrieve a key that didn't exist, would throw an error,
        //so the following if statement is important.
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        //This returns a new WeaponDefinition with a type of WeaponType.name,
        //which means it has failed to find the right WeaponDefinition
        return (new WeaponDefinition());
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : GenericSingletonClass<GameManager>
{
    [SerializeField]
    UIController _UIController;

    int _score = 0;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScore(int score)
    {
        _score += score;
        _UIController.SetScore(_score);
    }

}

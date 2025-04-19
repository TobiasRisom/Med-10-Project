using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScreenNavigation : MonoBehaviour
{
	private FirestoreHandler fish;

	private int pos = -1080;

	private float tweenSpeed = 0.5f;
	private Ease tweenEase = Ease.OutQuad;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();
	    
	    //fish.ScheduleManager();
	    //fish.spawnTasks(fish.username);
	    fish.spawnTasks("Leif");
    }
    
    public void changeWindow(int windowIndex)
    {
	    float targetX = pos * windowIndex;
	    transform.DOLocalMoveX(targetX, tweenSpeed).SetEase(tweenEase);
    }
}

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
	    
	    if (isItUpdateTime())
	    {
		    UpdateTasks();
	    }
    }
    
    public void changeWindow(int windowIndex)
    {
	    float targetX = pos * windowIndex;
	    transform.DOLocalMoveX(targetX, tweenSpeed).SetEase(tweenEase);
    }
    
    private Boolean isItUpdateTime()
    {
	    DateTime now = DateTime.Now;
	    string updateTime = PlayerPrefs.GetString("UpdateTime");
	    
	    if (DateTime.TryParse(updateTime, out DateTime storedTime))
	    {
		    if (now >= storedTime)
		    {
			    // Update Time!
			    return true;
		    }
	    }
	    return false;
    }
    
    private void UpdateTasks()
    {
	    fish.UpdateDailyAndWeeklyTasks((((int)DateTime.Now.DayOfWeek + 6) % 7) + 2); // Monday = 2, Tuesday = 3, Wednesday = 4...
	        
	    // Change next update date to tomorrow
	    DateTime tomorrow = DateTime.Now.Date.AddDays(1);
	    DateTime nextUpdate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 3, 0, 0);
	    PlayerPrefs.SetString("UpdateTime", nextUpdate.ToString("yyyy-MM-dd"));
    }
}

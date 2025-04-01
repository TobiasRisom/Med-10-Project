using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Swipe : MonoBehaviour, IDragHandler, IEndDragHandler
{
	private Vector3 panelLocation;
	public float percentThreshold = 0.2f;
	public float easing = 0.5f;
	public int currentPage = 0; // -1 = Left (Week), 0 = Middle (tasks), 1 = Right (Pet)
    void Start()
    {
	    panelLocation = transform.position;
    }

    public void OnDrag(PointerEventData data)
    {
	    float difference = Mathf.Clamp(data.pressPosition.x - data.position.x, -1080, 1080);
	    
	    transform.position = panelLocation - new Vector3(difference, 0, 0);
    }

    public void OnEndDrag(PointerEventData data)
    {
	    float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
	    if (Mathf.Abs(percentage) >= percentThreshold)
	    {
		    Vector3 newLocation = panelLocation;
		    if (percentage > 0 && currentPage < 1)
		    {
			    newLocation += new Vector3(-Screen.width, 0, 0);
			    currentPage++;

		    } else if (percentage < 0 && currentPage > -1)
		    {
			    newLocation += new Vector3(Screen.width, 0, 0);
			    currentPage--;
		    }
		    StartCoroutine(Smoothing(transform.position, newLocation, easing));

		    transform.position = newLocation;
		    panelLocation = newLocation;
	    }
	    else
	    {
		    StartCoroutine(Smoothing(transform.position, panelLocation, easing));
	    }
    }

    IEnumerator Smoothing(Vector3 startpos, Vector3 endpos, float seconds)
    {
	    float t = 0f;
	    while (t <= 1.0)
	    {
		    t += Time.deltaTime / seconds;
		    transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0, 1f, t));
		    yield return null;
	    }
    }
}

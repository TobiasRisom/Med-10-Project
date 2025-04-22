using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoomAnswer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private string taskTitle;
    private string userName;
    
    private string writtenAnswer;
    private Sprite imageAnswer;

    [SerializeField]
    private GameObject panel;

    private GameObject TaskImageAnswer;
    private GameObject TaskWrittenAnswer;

    private bool answerType = true; // True = Image, False = Writing
    void Start()
    {
	    panel = GameObject.FindWithTag("ScreenContentHolder")
	                      .transform.GetChild(2)
	                      .gameObject;
	    
	    taskTitle = gameObject.transform.GetChild(0)
	                    .GetComponent<TextMeshProUGUI>()
	                    .text;
	    
	    userName = gameObject.transform.GetChild(1)
	                          .GetComponent<TextMeshProUGUI>()
	                          .text;

	    TaskImageAnswer = gameObject.transform.GetChild(2)
	                                .gameObject;

	    TaskWrittenAnswer = gameObject.transform.GetChild(4)
	                                  .gameObject;

	    if (TaskImageAnswer.activeSelf)
	    {
		    if (TaskImageAnswer.GetComponent<Image>()
		                       .sprite != null)
		    {
			    imageAnswer = TaskImageAnswer.GetComponent<Image>()
			                                        .sprite;
			    answerType = true;
		    }
	    }
	    
	    if (TaskWrittenAnswer.activeSelf)
	    {
		    if (TaskWrittenAnswer.GetComponent<TextMeshProUGUI>()
		                         .text != null)
		    {
			    writtenAnswer = TaskWrittenAnswer.GetComponent<TextMeshProUGUI>()
			                                     .text;
			    answerType = false;
		    }
	    }
    }

    public void SetZoomIn()
    {
	    panel.SetActive(true);

	    GameObject title = panel.transform.GetChild(1)
	                            .gameObject;
	    GameObject username = panel.transform.GetChild(2)
	                               .gameObject;
	    GameObject image = panel.transform.GetChild(3)
	                            .gameObject;
	    GameObject writing = panel.transform.GetChild(4)
	                              .gameObject;

	    title.GetComponent<TextMeshProUGUI>()
	         .text = taskTitle;

	    username.GetComponent<TextMeshProUGUI>()
	            .text = userName;

	    if (answerType)
	    {
		    image.GetComponent<Image>()
		         .sprite = imageAnswer;

		    image.GetComponent<Image>()
		         .color = new Color(255, 255, 255, 255);
		    writing.GetComponent<TextMeshProUGUI>()
		           .text = "";
	    }

	    if (!answerType)
	    {
		    writing.GetComponent<TextMeshProUGUI>()
		           .text = writtenAnswer;

		    image.GetComponent<Image>()
		         .color = new Color(255, 255, 255, 0);
	    }
	    
    }
}

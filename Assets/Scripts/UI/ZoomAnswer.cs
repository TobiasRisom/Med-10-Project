using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoomAnswer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private string taskTitle;
    private string userName;
    private string desc;
    private string emoji;
    private string timeStamp;
    
    private string writtenAnswer;
    private Sprite imageAnswer;

    [SerializeField]
    private GameObject panel;

    private GameObject TaskImageAnswer;
    private GameObject TaskWrittenAnswer;

    private bool answerType = true; // True = Image, False = Writing
    void Start()
    {
	    panel = GameObject.FindWithTag("MainCanvas")
	                      .transform.GetChild(3)
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
	    
	    desc = gameObject.transform.GetChild(9)
	                          .GetComponent<TextMeshProUGUI>()
	                          .text;
	    
	    emoji = gameObject.transform.GetChild(10)
	                         .GetComponent<TextMeshProUGUI>()
	                         .text;
	    timeStamp = gameObject.transform.GetChild(11)
	                          .GetComponent<TextMeshProUGUI>()
	                          .text;
	    

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
	    GameObject usernameObject = panel.transform.GetChild(2)
	                               .gameObject;
	    GameObject image = panel.transform.GetChild(3)
	                            .gameObject;
	    GameObject writing = panel.transform.GetChild(4)
	                              .gameObject;
	    GameObject descObject = panel.transform.GetChild(6)
	                              .gameObject;

	    GameObject emojiObject = panel.transform.GetChild(7)
	                              .gameObject;
	    
	    GameObject timeStampObject = panel.transform.GetChild(8)
	                                      .gameObject;


	    title.GetComponent<TextMeshProUGUI>()
	         .text = taskTitle;

	    usernameObject.GetComponent<TextMeshProUGUI>()
	            .text = userName;

	    descObject.GetComponent<TextMeshProUGUI>()
	              .text = desc;
	    emojiObject.GetComponent<TextMeshProUGUI>()
	               .text = emoji;
	    timeStampObject.GetComponent<TextMeshProUGUI>()
	                   .text = timeStamp;

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

using UnityEngine;

public class MainMenu : MonoBehaviour
{
	Animator anim;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void SwitchTo(string where) {
		anim.SetTrigger(where);
	}
}

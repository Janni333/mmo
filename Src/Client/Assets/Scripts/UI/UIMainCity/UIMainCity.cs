using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainCity : MonoBehaviour {
	public UIAvatar uiavartar;

	// Use this for initialization
	void Start () {
		UpdateAvatar();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void UpdateAvatar()
	{
		this.uiavartar.curChar = User.Instance.currentCharacter;
		this.uiavartar.UpdateInfo();
	}
}

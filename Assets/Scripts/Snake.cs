using System.Collections;
using UnityEngine;
using System;	// to be used to send message/events

public class Snake : MonoBehaviour {

	public Snake nextPart;

	static public Action<String> hit;

	public void SetNext(Snake newPart) {
		nextPart = newPart;
	}

	public Snake GetNext() {
		return nextPart;
	}

	public void RemoveTail() {
		Destroy (this.gameObject);
	}

	void OnTriggerEnter(Collider other) {
		if (hit != null) {
			// send tag to gameController script
			hit (other.tag);
		}
		// if its a fruit, destroy the fruit
		if (other.tag == "Fruit") {
			Destroy (other.gameObject);
		}


	}
}

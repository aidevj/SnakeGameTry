using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour {

	public int maxSize = 5;
	public int currentSize;

	public int xBound;				// boundaries
	public int yBound;

	public GameObject snakePrefab;	// prefab of snake body part
	public GameObject foodPrefab;

	public GameObject currentFruit;

	public Snake head;				// obj acting as the head of the snake in heirarchy, this is what is being controlled
	public Snake tail;

	public int direction;			// 0: up | 1: right | 2: down | 3: left
	public Vector2 nextPos;

	public int score;				// player score

	void OnEnable() {
		// subscribe to hit action
		Snake.hit += hit;
	}

	void Start () {
		InvokeRepeating ("TimerInvoke", 0, .5f);

		// spawn a fruit right off the bat
		FruitHandler ();
	}

	void OnDisable(){
		// unsubscribe from hit action
		Snake.hit -= hit;
	}

	void Update () {
		HandleInput ();
	}

	void TimerInvoke()
	{
		Move ();	// instantiates a new tail piece constantly while always moving

		if (currentSize >= maxSize) {
			TailFunction ();
		} else {	
			currentSize++;	// increment the current size 
		}
	}

	void Move () {
		GameObject temp;
		nextPos = head.transform.position;	// make the next posiiton the positon of the head

		// depending on direction value, automatically move in that direction
		switch (direction) {
			case 0:	//up
				nextPos = new Vector2 (nextPos.x, nextPos.y + 1);
				break;
			case 1: //right
				nextPos = new Vector2 (nextPos.x + 1, nextPos.y);
				break;
			case 2: //down
				nextPos = new Vector2 (nextPos.x, nextPos.y - 1);
				break;
			case 3: // left
				nextPos = new Vector2 (nextPos.x -1, nextPos.y);
				break;
		}

		// instantiate a new head from the head essentially (spawns it in the next place in front)
		temp = (GameObject)Instantiate (snakePrefab, nextPos, transform.rotation);

		head.SetNext (temp.GetComponent<Snake> ());
		head = temp.GetComponent<Snake> ();

		return;
	}

	void SpawnBodyPart() {
	
	}

	void HandleInput() {
		// makes sure cant reverse direction and collide with self
		// this doesnt matter if the controls are rotation based
		if (Input.GetKeyDown (KeyCode.W) && direction != 2) {
			direction = 0;	// up
		}
		if (Input.GetKeyDown (KeyCode.D) && direction != 3) {
			direction = 1;	// right
		}
		if (Input.GetKeyDown (KeyCode.S) && direction != 0) {
			direction = 2; // down
		}
		if (Input.GetKeyDown (KeyCode.A) && direction != 1 ) {
			direction = 3; // left
		}
	}

	// called when the capacity of body part is at max
	// prevents from growing any larger if at capacity
	void TailFunction() {
		Snake oldSnakeTailPart = tail;		// save the current tail unit 
		tail = tail.GetNext ();				// get the next component down the line and assign that to the new tail
		oldSnakeTailPart.RemoveTail ();		// remove the old one
	}

	// handles the food spawning
	void FruitHandler() {
		// get a random pos within bounds
		int xPos = Random.Range  (-xBound, xBound);
		int yPos = Random.Range  (-yBound, yBound);

		currentFruit = (GameObject)Instantiate (foodPrefab, new Vector2 (xPos, yPos), Quaternion.identity);

		StartCoroutine (CheckRender (currentFruit));
	}

	// checks to make sure things are visible in the camera
	IEnumerator CheckRender(GameObject IN) {
		// when first spawning an object, make sure it is visible
		yield return new WaitForEndOfFrame(); // will wait until the end of frame then execute any lines of code after it

		if (IN.GetComponent<Renderer> ().isVisible == false) {
			//  despite having x and y bounds, just in case if not visible or off camera
			if (IN.tag == "Fruit") {
				Destroy (IN);
				FruitHandler (); // respawn a new one so it will hopefully be on camera
			}
		}
	}

	/// <summary>
	/// Catches the snake's body part colliding with an object.
	/// </summary>
	/// <param name="sent">This should be the string name of the tag of what was hit.</param>
	void hit(string sent) {
		if (sent == "Fruit") {
			// since in Snake.cs we destroyed the fruit that was collided with, make a new one
			FruitHandler();
			maxSize++;
			score++;
		}
		if (sent == "Snake") {
			// hits self, gameover
			CancelInvoke("TimerInvoke");
		}
	}
}

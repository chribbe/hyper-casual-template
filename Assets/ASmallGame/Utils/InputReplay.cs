using System;
using System.IO;    // for stream
using System.Linq;  // for List compare
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReplay : MonoBehaviour
{

	// public types
	public enum Mode { Record, PlayBack };
	public enum UpdateFunction { FixedUpdate, Update, Both };
	// public types for delegates
	public delegate bool AnyKeyType();
	public delegate bool GetKeyType(KeyCode code);
	public delegate bool GetMouseButtonType(int button);
	public delegate bool GetButtonType(string name);
	public delegate float GetAxisType(string name);
	public delegate Vector3 Vector3Type();
	public delegate Vector2 Vector2Type();

	// public properties for Management via Editor UI
	public bool active = false;
	public Mode mode = Mode.Record;
	public UpdateFunction UpdateCycle = UpdateFunction.Update;
	public string FilePath = "Temp/input.json";
	public bool manualStart = false;            // if true, only configure at start and wait for start
												// virtual button and axis support, list the InputManager's inputs you want to track
	public List<string> AxisList = new List<string>();
	public List<string> ButtonList = new List<string>();

	// public methods and properties for input access
	public GetKeyType GetKey;
	public GetKeyType GetKeyDown;
	public GetKeyType GetKeyUp;
	public GetMouseButtonType GetMouseButton;
	public GetMouseButtonType GetMouseButtonDown;
	public GetMouseButtonType GetMouseButtonUp;
	public GetButtonType GetButton;
	public GetButtonType GetButtonDown;
	public GetButtonType GetButtonUp;
	public GetAxisType GetAxis;

	private Vector3Type _mousePosition;
	private Vector3Type _mouseWorldPosition;
	private Vector2Type _mouseScrollDelta;
	public Vector3 mousePosition { get { return _mousePosition(); } }
	public Vector3 mouseWorldPosition { get { return _mouseWorldPosition(); } }
	public Vector2 mouseScrollDelta { get { return _mouseScrollDelta(); } }

	private AnyKeyType _anyKey;
	private AnyKeyType _anyKeyDown;
	public bool anyKey { get { return _anyKey(); } }
	public bool anyKeyDown { get { return _anyKeyDown(); } }

	// private types
	[Serializable]
	private struct InputSequence        // var names are reduced for smaller json
	{
		public float t;         // time
		public List<KeyCode> gK;    // getKey
		public List<KeyCode> gKD;   // getKeyDown
		public List<KeyCode> gKU;   // getKeyUp
		public Vector3 mP;      // mousePosition
		public Vector3 mWP;     // mouseWorldPosition
		public Vector2 mSD;     // mouseScrollDelta
		public List<string> vB;     // virtual Button
		public List<string> vBD;    // virtual Button Down
		public List<string> vBU;    // virtual Button Up
		public List<float> vA;      // virtual Axis

		public void init()
		{
			gK = new List<KeyCode>();
			gKD = new List<KeyCode>();
			gKU = new List<KeyCode>();
			mP = new Vector3();
			mWP = new Vector3();
			mSD = new Vector2();
			vB = new List<string>();
			vBD = new List<string>();
			vBU = new List<string>();
			vA = new List<float>();
		}
	};

	// Streams
	private StreamReader inputPlaybackStream;
	private StreamWriter inputRecordStream;

	// Input sequences
	private InputSequence oldSequence;
	private InputSequence currentSequence;
	private InputSequence nextSequence;

	// delegate to switch from record to play activity
	private delegate void Work(float time);
	private Work work;

	// start time of the record or playback
	private float startTime = 0.0f;



	// Use this for initialization
	void Start()
	{
		if (!active)    // if not active, act as UnityEngine.Input
		{
			SetInputStd();
			work = pause;
			return;
		}
		else        // else configure streams
		{
			work = pause;
			configure(mode, UpdateCycle, FilePath);

			if (!manualStart)   // start now 
			{
				switch (mode)
				{
					case Mode.Record:
						StartRecord();
						break;
					case Mode.PlayBack:
						StartPlayBack();
						break;
				}
			}
		}
	}

	~InputReplay()
	{
		Stop();
	}

	// Update is called once per frame
	void Update()
	{
		if (UpdateCycle == UpdateFunction.Update || UpdateCycle == UpdateFunction.Both)
			work(Time.time - startTime);
	}

	// FixedUpdate is called every physics update
	void FixedUpdate()
	{
		if (UpdateCycle == UpdateFunction.FixedUpdate || UpdateCycle == UpdateFunction.Both)
			work(Time.fixedTime - startTime);
	}

	/*
	 * PUBLIC METHODS
	 * */
	public bool configure(Mode m, UpdateFunction c, string p)
	{
		UpdateCycle = c;
		FilePath = p;
		mode = m;

		switch (mode)
		{
			case Mode.Record:
				oldSequence.init();
				currentSequence.init();

				inputRecordStream = new StreamWriter(FilePath, false);  // will overwrite new file Stream
				if (inputRecordStream.ToString() == "")
				{
					Stop();
					Debug.Log("InputReplay: StreamWriter(" + FilePath + "), file not found ?");
					return false;
				}
				else
				{
					inputRecordStream.AutoFlush = true;
					SetInputStd();
				}
				break;

			case Mode.PlayBack:
				oldSequence.init();
				currentSequence.init();
				nextSequence.init();

				inputPlaybackStream = new StreamReader(FilePath, false);
				if (inputPlaybackStream.ToString() == "")
				{
					Stop();
					Debug.Log("InputReplay: StreamReader(" + FilePath + "), file not found ?");
					return false;
				}
				else if (!ReadLine()) // read the first line to check
				{
					Stop();
					Debug.Log("InputReplay: empty file");
					return false;
				}
				break;
		}

		return true;
	}

	public void Stop()
	{
		SetInputStd();  // switch back to direct inputs

		active = false;

		switch (mode)
		{   // close streams
			case Mode.Record:
				inputRecordStream.Close();
				break;
			case Mode.PlayBack:
				inputPlaybackStream.Close();
				break;
		}

		work = pause;
	}

	public void StartRecord()
	{
		active = true;
		setStartTime();
		SetInputStd();
		work = Record;
	}

	public void StartPlayBack()
	{
		active = true;
		setStartTime();
		SetInputFake();
		work = Play;
	}

	/*
	 * PRIVATE METHODS
	 * */
	private void setStartTime()
	{
		if (UpdateCycle == UpdateFunction.FixedUpdate)
			startTime = Time.fixedTime;
		else
			startTime = Time.time;
	}

	private void SetInputStd()  // redirect public methods and properties to actual UnityEngine.Input
	{
		GetKey = Input.GetKey;
		GetKeyDown = Input.GetKeyDown;
		GetKeyUp = Input.GetKeyUp;
		GetMouseButton = Input.GetMouseButton;
		GetMouseButtonDown = Input.GetMouseButtonDown;
		GetMouseButtonUp = Input.GetMouseButtonUp;

		GetButton = Input.GetButton;
		GetButtonDown = Input.GetButtonDown;
		GetButtonUp = Input.GetButtonUp;
		GetAxis = Input.GetAxis;

		_mousePosition = delegate { return Input.mousePosition; };
		_mouseWorldPosition = delegate { return Camera.main.ScreenToWorldPoint(Input.mousePosition); };
		_mouseScrollDelta = delegate { return Input.mouseScrollDelta; };

		_anyKey = delegate { return Input.anyKey; };
		_anyKeyDown = delegate { return Input.anyKeyDown; };
	}

	private void SetInputFake() // redirect public methods and properties to our replay system
	{
		GetKey = FakeGetKey;
		GetKeyDown = FakeGetKeyDown;
		GetKeyUp = FakeGetKeyUp;
		GetMouseButton = FakeGetMouseButton;
		GetMouseButtonDown = FakeGetMouseButtonDown;
		GetMouseButtonUp = FakeGetMouseButtonUp;

		GetButton = delegate (string name) { return currentSequence.vB.Contains(name); };
		GetButtonDown = delegate (string name) { return currentSequence.vBD.Contains(name); };
		GetButtonUp = delegate (string name) { return currentSequence.vBU.Contains(name); };
		GetAxis = delegate (string name) { return currentSequence.vA.ElementAt(AxisList.FindIndex(str => str == name)); };

		_mousePosition = delegate { return currentSequence.mP; };
		_mouseWorldPosition = delegate { return currentSequence.mWP; };
		_mouseScrollDelta = delegate { return currentSequence.mSD; };

		_anyKey = delegate { return currentSequence.gK.Any<KeyCode>(); };
		_anyKeyDown = delegate { return currentSequence.gKD.Any<KeyCode>(); };
	}

	private void pause(float time)
	{
		// nothing to do
	}

	/* RECORD
	 * */
	private void Record(float time)
	{
		currentSequence.init();
		currentSequence.t = time;

		// store only true boolean
		foreach (KeyCode vkey in System.Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKey(vkey))
				currentSequence.gK.Add(vkey);
			if (Input.GetKeyDown(vkey))
				currentSequence.gKD.Add(vkey);
			if (Input.GetKeyUp(vkey))
				currentSequence.gKU.Add(vkey);
		}

		currentSequence.mP = Input.mousePosition;

		currentSequence.mWP = Camera.main.ScreenToWorldPoint(currentSequence.mP);

		currentSequence.mSD = Input.mouseScrollDelta;

		foreach (string virtualAxis in AxisList)
			currentSequence.vA.Add(Input.GetAxis(virtualAxis));

		foreach (string ButtonName in ButtonList)
		{
			if (Input.GetButton(ButtonName))
				currentSequence.vB.Add(ButtonName);
			if (Input.GetButtonDown(ButtonName))
				currentSequence.vBD.Add(ButtonName);
			if (Input.GetButtonUp(ButtonName))
				currentSequence.vBU.Add(ButtonName);
		}

		// only write if something changed
		if (AnyChange(oldSequence, currentSequence))
		{
			//Debug.Log (JsonUtility.ToJson (newSequence));
			inputRecordStream.WriteLine(JsonUtility.ToJson(currentSequence));
			oldSequence = currentSequence;
		}
	}

	// check if 
	private bool AnyChange(InputSequence seqA, InputSequence seqB)
	{
		if (!Enumerable.SequenceEqual(seqA.gK, seqB.gK)) return true;
		else if (!Enumerable.SequenceEqual(seqA.vB, seqB.vB)) return true;
		else if (!Enumerable.SequenceEqual(seqA.vA, seqB.vA)) return true;
		else if (seqA.mP != seqB.mP) return true;
		else if (seqA.mWP != seqB.mWP) return true;
		else if (seqA.mSD != seqB.mSD) return true;
		else return false;
	}

	/* PLAYBACK
	 * */
	private void Play(float time)
	{
		if (time >= nextSequence.t)
		{
			oldSequence = currentSequence;
			currentSequence = nextSequence;
			//Debug.Log (time);

			nextSequence.init();
			if (!ReadLine())
			{
				Stop();
				Debug.Log("InputPlayback: EndOfFile");
			}
		}
	}

	private bool ReadLine() // read a new line in file for the next sequence to play
	{
		string newline = inputPlaybackStream.ReadLine();

		if (newline == null)
			return false;

		nextSequence = JsonUtility.FromJson<InputSequence>(newline);
		return true;
	}

	private bool GetKeyCodeInList(KeyCode code, List<KeyCode> list)
	{
		foreach (KeyCode vkey in list)
		{
			if (vkey == code)
				return true;
		}
		return false;
	}

	/*
	 * PRIVATE FAKE INPUT
	 * */
	private bool FakeGetKey(KeyCode code) { return GetKeyCodeInList(code, currentSequence.gK); }
	private bool FakeGetKeyDown(KeyCode code) { return GetKeyCodeInList(code, currentSequence.gKD); }
	private bool FakeGetKeyUp(KeyCode code) { return GetKeyCodeInList(code, currentSequence.gKU); }
	private bool FakeGetMouseButton(int button) { return GetKeyCodeInList(KeyCode.Mouse0 + button, currentSequence.gK); }
	private bool FakeGetMouseButtonDown(int button) { return GetKeyCodeInList(KeyCode.Mouse0 + button, currentSequence.gKD); }
	private bool FakeGetMouseButtonUp(int button) { return GetKeyCodeInList(KeyCode.Mouse0 + button, currentSequence.gKU); }
	/* in case of removed up and down lists, compare old and current sequence
	public bool FakeGetKeyDown(KeyCode code) { return !GetKeyCodeInList (code, oldSequence.getKey) & GetKeyCodeInList (code, currentSequence.getKey); }
	public bool FakeGetKeyUp(KeyCode code) { return GetKeyCodeInList (code, oldSequence.getKey) & !GetKeyCodeInList (code, currentSequence.getKey); }
	public bool FakeGetMouseButtonDown(int button) { return !GetKeyCodeInList (KeyCode.Mouse0+button, oldSequence.getKey) & GetKeyCodeInList (KeyCode.Mouse0+button, currentSequence.getKey); }
	public bool FakeGetMouseButtonUp(int button) { return GetKeyCodeInList (KeyCode.Mouse0+button, oldSequence.getKey) & !GetKeyCodeInList (KeyCode.Mouse0+button, currentSequence.getKey); }
	*/
}

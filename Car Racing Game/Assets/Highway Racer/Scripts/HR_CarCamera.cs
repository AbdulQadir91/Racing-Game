//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class HR_CarCamera : MonoBehaviour{

	public CameraMode cameraMode;
	public enum CameraMode{Top, TPS, FPS}

	private GameObject audioListener;

	internal int cameraSwitchCount = 0;

	private RCC_HoodCamera hoodCam;
	private RCC_TPSCamera tpsCam;

	private float targetFieldOfView = 50f;
	public float topFOV = 48f;
	public float tpsFOV = 55f;
	public float fpsFOV = 65f;

	// The target we are following
	public Transform playerCar;
	private Rigidbody playerRigid;
	public bool gameover = false;

	private Camera cam;
	private Vector3 targetPosition = new Vector3(0, 0, 50);
	private Vector3 pastFollowerPosition, pastTargetPosition;

	// The distance in the x-z plane to the target
	public float distance = 8f;
	
	// the height we want the camera to be above the target
	public float height = 8.5f;

	private float rotation = 30f;

	private float currentT;
	private float oldT;

	private float speed = 0f;
	private float maxShakeAmount = 0.00025f;

	public GameObject mirrors;
	
	void Start(){

		cam = GetComponent<Camera>();

		transform.position = new Vector3(2f, 1f, 55f);
		transform.rotation = Quaternion.Euler(new Vector3(0f, -40f, 0f));

		if(GetComponent<AudioListener>())
			Destroy(GetComponent<AudioListener>());

		audioListener = new GameObject("Audio Listener");
		audioListener.transform.SetParent(transform, false);
		audioListener.AddComponent<AudioListener>();

		if(HR_GamePlayHandler.Instance.mode == HR_GamePlayHandler.Mode.Bomb)
			distance = 10f;
		
	}

	void OnEnable(){

		HR_PlayerHandler.OnPlayerSpawned += OnPlayerSpawned;
		HR_PlayerHandler.OnPlayerDied += OnPlayerCrashed;

	}

	void OnPlayerSpawned (HR_PlayerHandler player){

		playerCar = player.transform;
		playerRigid = player.GetComponent<Rigidbody>();
		hoodCam = player.GetComponentInChildren<RCC_HoodCamera>();
		tpsCam = player.GetComponentInChildren<RCC_TPSCamera>();

		if(GameObject.Find ("Mirrors"))
			mirrors = GameObject.Find ("Mirrors").gameObject;

	}

	void OnPlayerCrashed (HR_PlayerHandler player){

		gameover = true;

	}
	
	void LateUpdate(){

		if (!playerCar){
			playerCar = GameObject.FindObjectOfType<RCC_CarControllerV3>().transform;
			playerRigid = playerCar.GetComponent<Rigidbody>();
			return;
		}
		
		if(playerRigid != playerCar.GetComponent<Rigidbody>())
			playerRigid = playerCar.GetComponent<Rigidbody>();

		if(!cam)
			cam = GetComponent<Camera>();

		if (!playerCar || !playerRigid || Time.timeSinceLevelLoad < 1.5f) {
			transform.position += Quaternion.identity * Vector3.forward * (Time.deltaTime * 3f);
		} else if (playerCar && playerRigid) {

			//targetPosition = playerCar.position;
			cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, targetFieldOfView, Time.deltaTime * 3f);

			if (!gameover) {

				switch (cameraMode) {

				case CameraMode.Top:
					TOP ();
					if(mirrors)
						mirrors.SetActive (false);
					break;

				case CameraMode.TPS:
					if (tpsCam) {
						TPS ();
					} else {
						cameraSwitchCount++;
						ChangeCamera ();
					}
					break;

				case CameraMode.FPS:
					if (hoodCam) {
						FPS ();
						if(mirrors)
							mirrors.SetActive (true);
					} else {
						cameraSwitchCount++;
						ChangeCamera ();
					}
					break;

				}

			} else {

				if (Time.timeScale >= 1)
					transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z + Mathf.Clamp (currentT, 0f, Mathf.Infinity));

			}

			switch (cameraSwitchCount) {

			case 0:
				cameraMode = CameraMode.Top;
				break;
			case 1:
				cameraMode = CameraMode.TPS;
				break;
			case 2:
				cameraMode = CameraMode.FPS;
				break;

			}

		}

		audioListener.transform.position = new Vector3(playerCar.position.x, transform.position.y, transform.position.z);

		pastFollowerPosition = transform.position;
		pastTargetPosition = targetPosition;

		currentT = (transform.position.z - oldT);
		oldT = transform.position.z;

	}
		
	void Update(){

		if (Input.GetKeyDown (RCC_Settings.Instance.changeCameraKB))
			ChangeCamera ();

	}

	public void ChangeCamera(){

		cameraSwitchCount ++;

		if(cameraSwitchCount >= 3)
			cameraSwitchCount = 0;

	}

	void ChaseTarget (){

			if(!gameover){

//				if(cameraMode == CameraMode.Top)
//					targetPosition = new Vector3(0f, playerCar.position.y, playerCar.position.z);
//				if(cameraMode == CameraMode.TPS)
//					targetPosition = new Vector3(tpsCam.transform.position.x, tpsCam.transform.position.y, tpsCam.transform.position.z);

//				if(cameraMode == CameraMode.Top)
//					targetPosition -= transform.rotation * Vector3.forward * distance;
//				if(cameraMode == CameraMode.TPS && HR_GamePlayHandler.Instance.mode != HR_GamePlayHandler.Mode.Bomb)
//					targetPosition -= transform.rotation * Vector3.forward * (distance / 3f);
//				if(cameraMode == CameraMode.TPS && HR_GamePlayHandler.Instance.mode == HR_GamePlayHandler.Mode.Bomb)
//					targetPosition -= transform.rotation * Vector3.forward * (distance);

//				if(cameraMode == CameraMode.Top)
//					targetPosition = new Vector3(targetPosition.x, height, targetPosition.z);
//				if(cameraMode == CameraMode.TPS && HR_GamePlayHandler.Instance.mode != HR_GamePlayHandler.Mode.Bomb)
//					targetPosition = new Vector3(targetPosition.x, height / 4f, targetPosition.z);
//				if(cameraMode == CameraMode.TPS && HR_GamePlayHandler.Instance.mode == HR_GamePlayHandler.Mode.Bomb)
//					targetPosition = new Vector3(targetPosition.x, height / 2f, targetPosition.z);

				if(HR_HighwayRacerProperties.Instance._shakeCamera)
					targetPosition += (Random.insideUnitSphere * speed * maxShakeAmount);

//				if(cameraMode != CameraMode.FPS)
//					transform.position = SmoothApproach( pastFollowerPosition, pastTargetPosition, targetPosition, (speed / 10f) * Mathf.Clamp01(Time.timeSinceLevelLoad - 1.5f) );
//				else
//					transform.position = targetPosition;

			}else{

//				if(Time.timeScale >= 1)
//					transform.position =  new Vector3(transform.position.x, transform.position.y, transform.position.z + Mathf.Clamp(currentT, 0f, Mathf.Infinity));

			}
			
	}

	void TOP(){

		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation, 0f, 0f), Time.deltaTime * 2f);
		targetPosition = new Vector3(0f, playerCar.position.y, playerCar.position.z);
		targetPosition -= transform.rotation * Vector3.forward * distance;
		targetPosition = new Vector3(targetPosition.x, height, targetPosition.z);
		transform.position = SmoothApproach( pastFollowerPosition, pastTargetPosition, targetPosition, (speed / 10f) * Mathf.Clamp(Time.timeSinceLevelLoad - 1.5f, 0f, .85f) );
		targetFieldOfView = topFOV;

	}

	void TPS(){

		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation / 4f, 0f, 0f), Time.deltaTime * 2f);
		targetPosition = new Vector3(playerCar.position.x, tpsCam.transform.position.y, tpsCam.transform.position.z);
		transform.position = SmoothApproach( pastFollowerPosition, pastTargetPosition, targetPosition, (speed / 3f) * Mathf.Clamp(Time.timeSinceLevelLoad - 1.5f, 0f, .85f) );
		targetFieldOfView = tpsFOV;

	}

	void FPS(){

		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 2f);

		if(HR_HighwayRacerProperties.Instance._tiltCamera)
			transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, transform.localEulerAngles.y, -transform.InverseTransformDirection(playerRigid.velocity).x / 2f);
		
		targetPosition = hoodCam.transform.position;
		transform.position = targetPosition;
		targetFieldOfView = fpsFOV;

	}

	private Vector3 SmoothApproach( Vector3 pastPosition, Vector3 pastTargetPosition, Vector3 targetPosition, float speed ){

		if(Mathf.Approximately(Time.deltaTime, 0f))
			return transform.position;

		float t = Time.deltaTime * speed;
		Vector3 v = ( targetPosition - pastTargetPosition ) / t;
		Vector3 f = pastPosition - pastTargetPosition + v;
		return targetPosition - v + f * Mathf.Exp( -t );

	}

	void FixedUpdate(){

		if(!playerRigid)
			return;

		speed = Mathf.Lerp(speed, (playerRigid.velocity.magnitude * 3.6f), Time.deltaTime * 1.5f);

	}

	void OnDisable(){

		HR_PlayerHandler.OnPlayerSpawned -= OnPlayerSpawned;
		HR_PlayerHandler.OnPlayerDied -= OnPlayerCrashed;

	}

}
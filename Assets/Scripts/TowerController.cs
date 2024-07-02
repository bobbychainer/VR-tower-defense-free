using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

public class TowerController: MonoBehaviour {
	public LayerMask enemyLayer;
	protected string towerName = "DEFAULT";
	protected int level = 1;
	protected float lastAttackTime = 0f;
	protected int damage = 1;
	protected int towerPrice = 100;
	protected int totalTowerHealth = 100;
	protected int currentTowerHealth = 100;
	protected float attackCooldown = 1f;
	protected bool placed = false;
	public enum SelectState { NONE , DRAGGABLE , SELECTED , OPTION_UPGRADE , OPTION_DELETE }
	protected SelectState state;
	protected GameObject healthObject;
	
	//public Material blockedMaterial;
	private GenerateCubes generateCubes;
	private BuildController buildController;
	
	protected virtual void Start() {
		healthObject = gameObject.transform.Find("TowerPanel/HealthBar/Background/Anchor/Health").gameObject;		
		
		generateCubes = FindObjectOfType<GenerateCubes>();
		buildController = FindObjectOfType<BuildController>();
		
		state = SelectState.DRAGGABLE;
		ToggleTowerOptionList(false);
		ToggleTowerOptionControlls(false);
	}

	protected virtual void Update() {
		// no target detection or attack without being placed
		if (placed) {
			// attack if not on cooldown and if target near by
			if (TargetDetected() && AttackReady()) {
				Attack();
				lastAttackTime = Time.time;
			}
		}
	}
	
	// initialize tower
	protected virtual void Initialize() {
		return;
	}
	
	// target detected = true ; no target near by = false;
	protected virtual bool TargetDetected() {
		return false;
	}
	
	// attack cooldown
	// returns true if attack ready
	protected virtual bool AttackReady() {
		return (Time.time - lastAttackTime >= attackCooldown);
	}

	// attack target (depends on tower type)
	protected virtual void Attack() {
		return;
	}
	
	public void AcceptPressed() {
		// check if tower can be placed there
		Vector3 basePosition = gameObject.transform.Find("Base").transform.position;
		bool groundBlocked = generateCubes.TryCubeGroundAtPosition(basePosition.x, basePosition.z);
		
		if (groundBlocked) {
			Debug.Log("Place Tower");
			Initialize();
			placed = true;
			ClearTowerInfoElements();
			buildController.TowerAcceptButtonPressed();
		} else {
			Debug.Log("GROUND BLOCKED");
		}
	}
	
	public void CancelPressed() {
		Debug.Log("Placement Canceled");
		Destroy(gameObject);
		ClearTowerInfoElements();
		buildController.TowerCancelButtonPressed();
	}
	
	public void UpgradePressed() {
		Debug.Log("UPGRADE TOWER Pressed");
		if (placed) {
			state = SelectState.OPTION_UPGRADE;
			Debug.Log("UPGRADE TOWER - State = "+state);
			
			ToggleTowerOptionList(false);
			ToggleTowerOptionControlls(true);
		}
	}
	
	public void DeletePressed() {
		Debug.Log("DELETE TOWER Pressed");
		if (placed) {
			state = SelectState.OPTION_DELETE;
			Debug.Log("DELETE TOWER");
			
			ToggleTowerOptionList(false);
			ToggleTowerOptionControlls(true);
		}
	}
	
	public void OptionAcceptPressed() {
		
		Debug.Log("PRESSED OptionAcceptPressed - State = "+state);
		
		if (state == SelectState.OPTION_UPGRADE) {
			if (towerName != "DEFAULT") {
				bool upgradeCoinCheckSuccessful = buildController.TowerUpgradeButtonPressed(towerName, level);
				if (upgradeCoinCheckSuccessful) {
					// TODO UPGRADES on tower
					Debug.Log("UPGRADE SUCCESSFUL");
				}
			}
		} else if (state == SelectState.OPTION_DELETE) {
			Debug.Log("DELETE TOWER");
			Vector3 basePosition = gameObject.transform.Find("Base").transform.position;
			generateCubes.ResetCubeGroundAtPosition(basePosition.x, basePosition.z);
			Destroy(gameObject);
			//TODO get money back TODO
		}
		
		ToggleTowerOptionList(true);
		ToggleTowerOptionControlls(false);
		
		state = SelectState.SELECTED;
	}
	
	public void OptionCancelPressed() {
		ToggleTowerOptionControlls(false);
		ToggleTowerOptionList(true);
		state = SelectState.SELECTED;
	}
	
	private void ToggleTowerOptionList(bool isVisible) {
		GameObject optionListObject = gameObject.transform.Find("TowerPanel/OptionMenu/List").gameObject;
		if (optionListObject != null) optionListObject.SetActive(isVisible);
	}
	
	private void ToggleTowerOptionControlls(bool isVisible) {
		GameObject optionControllObject = gameObject.transform.Find("TowerPanel/OptionMenu/Controlls").gameObject;
		if (optionControllObject != null) optionControllObject.SetActive(isVisible);
	}
	
	private void ClearTowerInfoElements() {
		GameObject acceptObject = gameObject.transform.Find("TowerPanel/Accept").gameObject;
		GameObject cancelObject = gameObject.transform.Find("TowerPanel/Cancel").gameObject;
		acceptObject.SetActive(false);
		cancelObject.SetActive(false);
	}
	
	// activate tower
	public void PlaceTower() {
		Initialize();
		placed = true;
	}

    private void OnTriggerEnter(Collider other) {
		//Collision EnemyBullet -> Tower
        if (other.gameObject.tag == "EnemyBullet") {
			
            BulletController bulletController = other.gameObject.GetComponent<BulletController>();
			
            if (bulletController != null) {
				
                int damage = bulletController.GetDamage();
                bulletController.TargetHit();
                TakeDamage(damage);
				Destroy(other.gameObject);
            }
        }
    }
	
	private void UpdateHealthBar() {
		// Calculate the new scale for the health object based on the current health percentage
		Vector3 newScale = healthObject.transform.localScale;
		float healthPercentage = (float)currentTowerHealth / (float)totalTowerHealth;
		newScale.x = healthPercentage;

		// Apply the new scale
		healthObject.transform.localScale = newScale;

		// Adjust the position to keep the left side anchored
		Vector3 newPos = healthObject.transform.localPosition;
		float originalWidth = 1f; // Assuming the original width of the health bar is 1 unit
		newPos.x = (healthPercentage - 1) * (originalWidth / 2);

		// Apply the new position
		healthObject.transform.localPosition = newPos;
	}

    public void TakeDamage(int damage) {
        currentTowerHealth -= damage;
		// update healthObject
		UpdateHealthBar();
        if (currentTowerHealth <= 0) {
            Destroy(gameObject);
            Debug.Log("Tower destroyed.");
            //GameManager.instance.UpdatePlayerScore(enemyValue); Minuspunkte hinzuf�gen
        }
    }
	
	public void CloseInformationMenu() {
		state = SelectState.NONE;
		ToggleTowerOptionList(false);
		ToggleTowerOptionControlls(false);
	}
	
	public void ShowInformationMenu() {
		state = SelectState.SELECTED;
		ToggleTowerOptionList(true);
	}
	
	public bool hasBeenPlaced() { return placed; }
	
	public SelectState GetSelectState() { return state;	}

}

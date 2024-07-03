using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Debug = UnityEngine.Debug;

public class TowerController: MonoBehaviour {
	public LayerMask enemyLayer;
	protected string towerName = "DEFAULT";
	protected int level = 1;
	protected int maxLevel = 1;
	protected float lastAttackTime = 0f;
	protected int damage = 1;
	protected float attackRadius = 2f;
	protected int towerPrice = 100;
	protected int totalTowerHealth = 100;
	protected int currentTowerHealth = 100;
	protected float attackCooldown = 1f;
	protected bool placed = false;
	public enum SelectState { NONE , DRAGGABLE , SELECTED , OPTION_UPGRADE , OPTION_DELETE }
	protected SelectState state;
	protected GameObject healthObject;
	protected GameObject baseObject;
	
	//public Material blockedMaterial;
	private GenerateCubes generateCubes;
	protected BuildController buildController;
	
	protected virtual void Start() {
		healthObject = gameObject.transform.Find("TowerPanel/HealthBar/Background/Anchor/Health").gameObject;
		baseObject = gameObject.transform.Find("Base").gameObject;
		
		generateCubes = FindObjectOfType<GenerateCubes>();
		buildController = FindObjectOfType<BuildController>();
		
		state = SelectState.DRAGGABLE;
		ToggleTowerOptionList(false);
		ToggleTowerOptionControlls(false);
		
		SetInformationText(towerName+" TOWER\n\nMove Tower by Dragging.\nPress Accept to Place.");
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
	
	protected virtual void UpgradeTower() {
		level += 1;
		UpdateLevel();
	}
	
	public void AcceptPressed() {
		// check if tower can be placed there
		Vector3 basePosition = baseObject.transform.position;
		bool groundBlocked = generateCubes.TryCubeGroundAtPosition(basePosition.x, basePosition.z);
		
		if (groundBlocked) {
			Debug.Log("Place Tower");
			Initialize();
			placed = true;
			ClearTowerInfoElements();
			buildController.TowerAcceptButtonPressed();
		} else {
			Debug.Log("GROUND BLOCKED");
			SetInformationText("Can't be placed here.\n\nMove Tower by Dragging.\nPress Accept to Place.");
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
			if (level < maxLevel) {
				state = SelectState.OPTION_UPGRADE;
				Debug.Log("UPGRADE TOWER - State = "+state);
				
				string upgradeInformationText = CreateUpgradeInformationText();
				SetInformationText(upgradeInformationText);
				ToggleTowerOptionList(false);
				ToggleTowerOptionControlls(true);
			} else {
				// TODO Show max level reached
				SetInformationText("LEVEL "+level+" / "+maxLevel+"\n\nMax Level reached.");
			}
		} else {
			// WTF HOW 
			Debug.Log("WTF?! HOW DID YOU DO THAT. TOWER NOT PLACED AND UPGRADE PRESSED!");
		}
	}
	
	public void DeletePressed() {
		Debug.Log("DELETE TOWER Pressed");
		if (placed) {
			state = SelectState.OPTION_DELETE;
			Debug.Log("DELETE TOWER");
			
			int moneyBack = (int)(towerPrice*0.8);
			SetInformationText("DELETE\n\nRemove Tower for a Refund\nRefund: "+moneyBack);
			
			ToggleTowerOptionList(false);
			ToggleTowerOptionControlls(true);
		} else {
			// WTF HOW 
			Debug.Log("WTF?! HOW DID YOU DO THAT. TOWER NOT PLACED AND UPGRADE PRESSED!");
		}
	}
	
	public void OptionAcceptPressed() {
		
		Debug.Log("PRESSED OptionAcceptPressed - State = "+state);
		
		if (state == SelectState.OPTION_UPGRADE) {
			if (towerName != "DEFAULT") {
				bool upgradeCoinCheckSuccessful = buildController.TowerUpgradeButtonPressed(towerName, level);
				if (upgradeCoinCheckSuccessful) {
					// TODO UPGRADES on tower
					UpgradeTower();
					Debug.Log("UPGRADE SUCCESSFUL");
					string towerStatText = CreateStatInformationText();
					SetInformationText(towerStatText);
				}
			}
		} else if (state == SelectState.OPTION_DELETE) {
			Debug.Log("DELETE TOWER");
			Vector3 basePosition = baseObject.transform.position;
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
		string towerStatText = CreateStatInformationText();
		SetInformationText(towerStatText);
	}
	
	private void ToggleTowerOptionList(bool isVisible) {
		GameObject optionListObject = transform.Find("TowerPanel/OptionMenu/List").gameObject;
		if (optionListObject != null) optionListObject.SetActive(isVisible);
	}
	
	private void ToggleTowerOptionControlls(bool isVisible) {
		GameObject optionControllObject = transform.Find("TowerPanel/OptionMenu/Controlls").gameObject;
		if (optionControllObject != null) optionControllObject.SetActive(isVisible);
	}
	
	private void ToggleTowerInformation(bool isVisible) {
		GameObject informationTextObject = transform.Find("TowerPanel/Information").gameObject;
		if (informationTextObject != null) informationTextObject.SetActive(isVisible);
	}
	
	private void UpdateLevel() {
		TextMeshProUGUI levelText = transform.Find("TowerPanel/Level/Canvas/Image/LevelText").GetComponent<TextMeshProUGUI>();		
		if (levelText != null) {
            levelText.text = level.ToString();
        }
	}
	
	private void SetInformationText(string text) {
		TextMeshProUGUI informationText = transform.Find("TowerPanel/Information/Canvas/Image/InformationText").GetComponent<TextMeshProUGUI>();		
		if (informationText != null) {
            informationText.text = text;
        }
	}
	
	private string CreateStatInformationText() {
		string text = towerName+" TOWER\n\n";
		text += "Damage "+damage+"\n";
		text += "Attack Speed "+attackCooldown+"\n";
		text += "Attack Radius "+attackRadius+"\n";
		return text;
	}
	
	private string CreateUpgradeInformationText() {
		string text = "UPGRADES\n\n";
		var upgrades = buildController.GetAllUpgrades(towerName,level);
		Debug.Log(upgrades);
		if (upgrades.damage != 0) text += "Damage "+damage+" -> "+upgrades.damage+"\n";
		if (upgrades.attackCooldown != 0f) text += "Attack Speed "+attackCooldown+" -> "+upgrades.attackCooldown+"\n";
		if (upgrades.attackRadius != 0f) text += "Attack Radius "+(int)attackRadius+" -> "+(int)upgrades.attackRadius+"\n";
		return text;
	}
	
	private void ClearTowerInfoElements() {
		GameObject acceptObject = gameObject.transform.Find("TowerPanel/Accept").gameObject;
		GameObject cancelObject = gameObject.transform.Find("TowerPanel/Cancel").gameObject;
		acceptObject.SetActive(false);
		cancelObject.SetActive(false);
		CloseInformationMenu();
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
		ToggleTowerInformation(false);
	}
	
	public void ShowInformationMenu() {
		state = SelectState.SELECTED;
		ToggleTowerOptionList(true);
		ToggleTowerInformation(true);
		string towerStatText = CreateStatInformationText();
		SetInformationText(towerStatText);
	}
	
	public bool hasBeenPlaced() { return placed; }
	
	public SelectState GetSelectState() { return state;	}

}

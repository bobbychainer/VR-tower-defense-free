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
	protected int towerPrice = 0;
	protected int totalTowerHealth = 100;
	protected int currentTowerHealth = 100;
	protected float attackCooldown = 1f;
	protected bool placed = false;
	public enum SelectState { NONE , DRAGGABLE , SELECTED , OPTION_UPGRADE , OPTION_DELETE }
	protected SelectState state;
	protected GameObject healthObject;
	protected GameObject baseObject;
	
	private GenerateCubes generateCubes;
	protected BuildController buildController;
	
	// on start
	protected virtual void Start() {
		// start initializations
		healthObject = gameObject.transform.Find("TowerPanel/HealthBar/Background/Anchor/Health").gameObject;
		baseObject = gameObject.transform.Find("Base").gameObject;
		generateCubes = FindObjectOfType<GenerateCubes>();
		buildController = FindObjectOfType<BuildController>();
		// on spawn in drag mode
		state = SelectState.DRAGGABLE;
		// deactivate select options
		ToggleTowerOptionList(false);
		ToggleTowerOptionControlls(false);
		// set info text
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
	
	// upgrade tower (custom upgrades on tower types possible)
	protected virtual void UpgradeTower() {
		level += 1;
		UpdateLevel();
	}
	
	// tower not placed
	// start placement try and initialize/activate if successfull
	public void AcceptPressed() {
		// check if tower can be placed on tower base position
		Vector3 basePosition = baseObject.transform.position;
		bool groundBlocked = generateCubes.TryCubeGroundAtPosition(basePosition.x, basePosition.z);
		
		if (groundBlocked) {
			// activate tower 
			Initialize();
			placed = true;
			// remove buttons
			ClearTowerInfoElements();
			// reset build controller state and handle press
			buildController.TowerAcceptButtonPressed();
		} else {
			// set info text
			SetInformationText("Can't be placed here.\n\nMove Tower by Dragging.\nPress Accept to Place.");
		}
	}
	
	// destroy tower if placement cancled
	public void CancelPressed() {
		Destroy(gameObject);
		// remove buttons
		ClearTowerInfoElements();
		// reset build controller state
		buildController.TowerCancelButtonPressed();
	}
	
	// upgrade can be pressed if placed and not max level reached
	public void UpgradePressed() {
		if (placed) {
			if (level < maxLevel) {
				// update state
				state = SelectState.OPTION_UPGRADE;
				// set info text				
				string upgradeInformationText = CreateUpgradeInformationText();
				SetInformationText(upgradeInformationText);
				// show accept and cancel button (menu controlls)
				ToggleTowerOptionList(false);
				ToggleTowerOptionControlls(true);
			} else {
				// set info text
				SetInformationText("LEVEL "+level+" / "+maxLevel+"\n\nMax Level reached.");
			}
		} else {
			// WTF HOW 
			Debug.Log("WTF?! HOW DID YOU DO THAT. TOWER NOT PLACED AND UPGRADE PRESSED!");
		}
	}
	
	// if tower placed show refund options and toggle menu controlls
	public void DeletePressed() {
		if (placed) {
			// update state
			state = SelectState.OPTION_DELETE;		
			// refund coins
			int coinsBack = (int)(towerPrice*0.8);
			// set info text
			SetInformationText("DELETE\n\nRemove Tower for a Refund\nRefund: "+coinsBack);
			// show accept and cancel button (menu controlls)
			ToggleTowerOptionList(false);
			ToggleTowerOptionControlls(true);
		} else {
			// WTF HOW 
			Debug.Log("WTF?! HOW DID YOU DO THAT. TOWER NOT PLACED AND UPGRADE PRESSED!");
		}
	}
	
	// option menu controlls accept has been pressed
	// upgrade or delete action will be executed
	public void OptionAcceptPressed() {
		if (state == SelectState.OPTION_UPGRADE) {
			// upgrade
			// check if upgrade payment was successfull
			bool upgradeCoinCheckSuccessful = buildController.TowerUpgradeButtonPressed(towerName, level);
			if (upgradeCoinCheckSuccessful) {
				// start tower upgrade
				UpgradeTower();
				// set info text to stat
				string towerStatText = CreateStatInformationText();
				SetInformationText(towerStatText);
				// show upgrade and delete button
				ToggleTowerOptionList(true);
				ToggleTowerOptionControlls(false);
				// update state
				state = SelectState.SELECTED;
			} else {
				SetInformationText("UPGRADE\n\nNot Enough Coins.");
			}
		} else if (state == SelectState.OPTION_DELETE) {
			// delete
			// reset ground cube tag and material
			Vector3 basePosition = baseObject.transform.position;
			generateCubes.ResetCubeGroundAtPosition(basePosition.x, basePosition.z);
			// get money refund
			int towerRefund = (int)(towerPrice*0.8);
			buildController.TowerDeleteButtonPressed(towerRefund);
			Destroy(gameObject);
		}
	}
	
	// cancel tower option and reset state
	public void OptionCancelPressed() {
		// show upgrade and delete button
		ToggleTowerOptionControlls(false);
		ToggleTowerOptionList(true);
		// update state
		state = SelectState.SELECTED;
		// set info text to stat
		string towerStatText = CreateStatInformationText();
		SetInformationText(towerStatText);
	}
	
	// toggle information panel upgrade and delete button
	private void ToggleTowerOptionList(bool isVisible) {
		GameObject optionListObject = transform.Find("TowerPanel/OptionMenu/List").gameObject;
		if (optionListObject != null) optionListObject.SetActive(isVisible);
	}
	
	// toggle information panel accept and cancel button
	private void ToggleTowerOptionControlls(bool isVisible) {
		GameObject optionControllObject = transform.Find("TowerPanel/OptionMenu/Controlls").gameObject;
		if (optionControllObject != null) optionControllObject.SetActive(isVisible);
	}
	
	// toggle information panel information text box
	private void ToggleTowerInformation(bool isVisible) {
		GameObject informationTextObject = transform.Find("TowerPanel/Information").gameObject;
		if (informationTextObject != null) informationTextObject.SetActive(isVisible);
	}
	
	// update information panel level text
	private void UpdateLevel() {
		TextMeshProUGUI levelText = transform.Find("TowerPanel/Level/Canvas/Image/LevelText").GetComponent<TextMeshProUGUI>();		
		if (levelText != null) levelText.text = level.ToString();
	}
	
	// set information text in information panel
	private void SetInformationText(string text) {
		TextMeshProUGUI informationText = transform.Find("TowerPanel/Information/Canvas/Image/InformationText").GetComponent<TextMeshProUGUI>();		
		if (informationText != null) informationText.text = text;
	}
	
	// create tower stats information text
	private string CreateStatInformationText() {
		string text = towerName+" TOWER\n\n";
		text += "Damage "+damage+"\n";
		text += "Attack Speed "+attackCooldown+"\n";
		text += "Attack Radius "+attackRadius+"\n";
		return text;
	}
	
	// create tower state upgrade information text
	private string CreateUpgradeInformationText() {
		string text = "UPGRADES\n\n";
		// get upgrades for level
		var upgrades = buildController.GetAllUpgrades(towerName,level);
		text += "Costs  "+upgrades.price+" Coins\n";
		if (upgrades.damage != 0) text += "Damage "+damage+" -> "+upgrades.damage+"\n";
		if (upgrades.attackCooldown != 0f) text += "Attack Speed "+attackCooldown+" -> "+upgrades.attackCooldown+"\n";
		if (upgrades.attackRadius != 0f) text += "Attack Radius "+(int)attackRadius+" -> "+(int)upgrades.attackRadius+"\n";
		return text;
	}
	
	// remove tower informations
	private void ClearTowerInfoElements() {
		// hide accept and delete button
		GameObject acceptObject = gameObject.transform.Find("TowerPanel/Accept").gameObject;
		GameObject cancelObject = gameObject.transform.Find("TowerPanel/Cancel").gameObject;
		acceptObject.SetActive(false);
		cancelObject.SetActive(false);
		// hide tower options
		CloseInformationMenu();
	}
	
	// collision triggered by EnemyBullet to take damage
    private void OnTriggerEnter(Collider other) {
		//Collision EnemyBullet -> Tower
        if (other.gameObject.tag == "EnemyBullet") {
            BulletController bulletController = other.gameObject.GetComponent<BulletController>();
            if (bulletController != null) {
				// get bullet damage
                int damage = bulletController.GetDamage();
				// take damage
                bulletController.TargetHit();
                TakeDamage(damage);
				Destroy(other.gameObject);
            }
        }
    }
	
	// update health bar after taking damage
	private void UpdateHealthBar() {
		// calculate new scale for the health object based on the current health percentage
		Vector3 newScale = healthObject.transform.localScale;
		float healthPercentage = (float)currentTowerHealth / (float)totalTowerHealth;
		newScale.x = healthPercentage;
		// apply new scale to health object
		healthObject.transform.localScale = newScale;
		// adjust  position to keep the left side anchored
		Vector3 newPos = healthObject.transform.localPosition;
		float originalWidth = 1f;
		newPos.x = (healthPercentage - 1) * (originalWidth / 2);
		// apply new position to health object
		healthObject.transform.localPosition = newPos;
	}

	// take damge from enemy bullet
    public void TakeDamage(int damage) {
        currentTowerHealth -= damage;
		// update healthObject
		UpdateHealthBar();
        if (currentTowerHealth <= 0) {
			// destroy tower
            Destroy(gameObject);
            Debug.Log("Tower destroyed.");
        }
    }
	
	// hide informations on not selected
	public void CloseInformationMenu() {
		// update state
		state = SelectState.NONE;
		ToggleTowerOptionList(false);
		ToggleTowerOptionControlls(false);
		ToggleTowerInformation(false);
	}
	
	// show informations on tower selected
	public void ShowInformationMenu() {
		// update state
		state = SelectState.SELECTED;
		ToggleTowerOptionList(true);
		ToggleTowerInformation(true);
		// set info text to stat
		string towerStatText = CreateStatInformationText();
		SetInformationText(towerStatText);
	}
	
	public bool hasBeenPlaced() { return placed; }
	
	public SelectState GetSelectState() { return state;	}
	
	public void IncreaseTowerPrice(int extraPrice) { towerPrice += extraPrice; }
}

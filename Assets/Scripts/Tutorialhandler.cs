using UnityEngine;
using TMPro;

public class DropdownHandlerTMP : MonoBehaviour
{
    public TMP_Dropdown towerDropdown;
    public TMP_Text infoText;

    void Start()
    {
        // Add a listener to call the method OnDropdownValueChanged when an option is selected
        towerDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Initialize the text to the first option's information
        OnDropdownValueChanged(towerDropdown.value);
    }

    void OnDropdownValueChanged(int index)
    {
        // Change the text based on the selected option
        switch (index)
        {
            case 0: //Controls
                infoText.text = @"
Welcome to our TowerDef VR Game

Preparation Phase:
Build and upgrade your defense.

Attack Phase:
Enemies will start spawning and moving towards your base.
Your towers will automatically attack the enemies as they come into range.
You cannot build or upgrade towers during this phase, so make sure your setup is ready!

Game Layout:
Enemies run from spawn (green) to base (blue) on a given path (red).
Each round, the base moves further back, increasing the challenge.
Enemy stats and timer increases over the time.

Objective:
Prevent the enemies from reaching your base. If too many enemies get through, you will lose the game.
                ";
                break;
            case 1://Controls
                infoText.text = @"
                Controls:
                
                Right Hand:
                - Trigger: UI Click / Player Shoot Ability
                - Touchpad Up: Press to Teleport on a Location
                - Touchpad right/left: Touch to do a 90° turn, Press to keep spinning
                
                Left Hand:
                - Trigger: Teleport to Overview
                - Touchpad: Movement and Strafe
                - Grab: Pause the Game
                
                Tip: You can look at your controllers, while ingame to see tooltips!";
                break;
            case 2://Towers
                infoText.text = @"All Towers can be upgraded.
Small Tower
Damage: 1
Attack Cooldown: 1 second
Attack Radius: 8 units
Max Level: 7
Description: The Small Tower has a slow firing rate but compensates with an extensive attack range, making it ideal for covering large areas of the battlefield.

Rapid Tower
Damage: 1
Attack Cooldown: 0.7 seconds
Attack Radius: 5 units
Max Level: 6
Description: The Rapid Tower fires quickly, allowing it to hit enemies frequently. However, it has a shorter attack range, best suited for close encounters.

Laser Tower
Damage: 1
Attack Cooldown: 5 seconds
Attack Radius: 8 units
Max Level: 5
Description: The Laser Tower attacks less frequently but can strike in all four directions when enemies are within range, making it powerful against multiple foes at once.
                ";
                break;
            
            case 3: //Player
                infoText.text = @"

As the commander of the defense forces, you have several crucial abilities at your disposal in this tower defense game:

Tower Placement: Strategically position towers to defend key points along the enemy's path.
Tower Management: Upgrade and fine-tune your towers to optimize their effectiveness against increasingly formidable enemies.
Direct Combat Support: Engage directly by shooting to assist towers, especially during critical moments when enemies threaten to breach defenses.
If you are defeated in battle, you will be temporarily frozen until you respawn, preventing you from participating actively until then.

Your objective is clear: protect the base by skillfully managing your towers and engaging enemies with strategic precision.

                ";
                break;

            default://Enemies
                infoText.text = @"
Slimy Slime
Health: 1
Damage to Base: 1
Speed: 4 units/second
Description: Slimy Slimes are basic enemies with low health and moderate speed. They pose a minimal threat individually but can overwhelm defenses in large numbers.

Jumpy Shroom
Health: 2
Damage to Base: 2
Speed: 3 units/second
Description: Jumpy Shrooms are agile enemies that hop towards the base, making them harder to hit. They have a moderate threat level due to their increased mobility.

Armored Shell
Health: 5
Damage to Base: 5
Speed: 2 units/second
Description: Armored Shells are heavily protected enemies with high health and slow speed. They are tough to destroy and can withstand substantial damage before reaching the base.

Prickly Cactus (Special)
Health: 3 
Damage to Base: 3 
Speed: 1 unit/second 
Attack Cooldown: 5 seconds
Damage: 1
Description: Prickly Cacti are unique enemies that not only advance towards the base but also attack towers and the player if in range. They require strategic handling due to their ability to damage defenses directly.";
                break;
        }
    }
}

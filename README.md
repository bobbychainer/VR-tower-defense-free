# TowerDefenseVR
In an informtik lab, three other fellow students and I implemented a Tower-Defense based game in unity. The game is implemented in 3D and for VR (it can also be played without VR setup via XOrigin emulator). A new aspect of the game is that you can intervene as a player in the game.

## Table of Contents
1. [Game Prinicple](#game-prinicple)
2. [Implemented Features](#implemented-features)
3. [Installation](#installation)
4. [Contributing](#contributing)
5. [License](#license)
6. [Credits](#credits)
7. [Acknowledgments](#acknowledgments)

## Game Prinicple
The playing field is a 50x50 blockgrid with a spawn and a base. Each round consists of the phases **Preparation** and **Attack**. In the Preparation phase you can build or upgrade towers via the UI menu, as soon as you click on the ready-button, the phase and the timer starts. The enemies start to spawn and run along the marked path to the base. In each round the base changes its position further back on the path and the enemies change or become stronger.
The base has lives and loses them when enemies reach the base. You can control the player in both phases and attack the opponents in the Attack phase. The player also has lives, if he loses them all, he will be freed until the end of the round.

## Implemented Features
- Path with spawn and base
- Player movement and shooting
- Four different enemy types
- Four different tower types and upgrades
- Score and health system
- Menus and UIs

### Project status
- Done

### Future implementations
- 

## Installation
### Prerequisites and important info
- Unity Version: This project was developed using Unity LTS version 2022.3.26f1.
- Compatibility: The game is designed for 3D VR gameplay and is compatible with desktop platforms. You can also play without a VR-setup with the XOrigin Emulator in the unity project. You have to activate it before playing.

### Installation Steps
Clone the repository and add it to Unity Hub
```bash
git clone https://github.com/sp8cky/TowerDefenseVR
```

### Customization Options:
- Adjust player movement and attack settings
- Modify enemy/tower behavior: Users can customize various aspects of the enemy/tower behavior, such as movement speed, attack speed, spawn rate...
- Change game visuals: Users can replace the default sprites with their own artwork to customize the game's appearance.

## Contributing
- Feedback and Contributions: Feedback and contributions are welcome! If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request on GitHub.

### How to Contribute
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m 'Add some feature'`
4. Push to the branch: `git push origin feature/your-feature`
5. Open a pull request

## License:
- This project is licensed under the MIT-License. See the LICENSE file for details.

## Credits:
- This project was created by sp8cky and three other people.
  
## Acknowledgments
- We used a few packages and assets, all listed in the credits menu, thanks for those!

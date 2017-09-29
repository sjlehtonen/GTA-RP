# GTA Roleplaying script

This project is a roleplaying game mode for GTA-MP multiplayer modification for GTA V. 

## Features
- Account system in a way that one account can have multiple characters
- House ownership system
- Vehicle ownership and parking system
- Vehicle shop system with easy way to create new shops
- Dynamic weather system
- Profession system with the possibility of making new professions easily
- Faction system for factions like police and military
- Phone system with address book and the possibility of sending text messages and making phone calls



## Requirements

- GTA-MP Server
- MySql

## Installation

1. Put all the content inside a folder called GTA-RP and place it in the resources folder of your GTA-MP Server installation

2. Set GTA-RP as the current game mode inside the **settings.xml** which should be found inside the GTA-MP Server root

3. Create a file called **Config.ini** inside the Config folder (if the folder doesn't exist, create it at the game mode root) and add the following:

```
[database]
; database configuration
server="server here, usually localhost"
dbname="database name here"
username="username here"
password="password here"
```

4. Import the database file **gta_rp.sql** (Found at the root of this repository) provided here to initialize the database
	- It contains a test account with name **Test** and password **Test123**
		- **Note:** Your GTA-MP name must match the account name currently. 
			- In the future, social club ID will be used instead.
	- It also contains a few characters on the account, their property ownerships and vehicle ownerships

5. Have **MySql.Data.dll**, **INIFileParser.dll** and **System.Data.dll** in the GTA-MP Server directory root

## Pictures of the game mode

### General pictures:
![UI10](Images/1.bmp)
![UI11](Images/3.bmp)
![UI12](Images/4.bmp)

### Buying a vehicle:
![UI1](Images/12.bmp)

### Login screen:
![UI2](Images/5.bmp)
![UI3](Images/6.bmp)

### Character selection:
![UI4](Images/2.bmp)

### Working as a garbage truck driver:
![UI5](Images/10.bmp)
![UI6](Images/11.bmp)

### Housing pictures:
![UI7](Images/7.bmp)
![UI8](Images/8.bmp)
![UI9](Images/9.bmp)

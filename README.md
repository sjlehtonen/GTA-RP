# GTA Roleplaying Script

This project is a roleplaying game mode for GTA-MP multiplayer modification for GTA V.

## NOTE
I am working on this game mode alone so many things are not implemented yet. However, a lot of the base systems such as having the ability to own houses and vehicles is done. Also it's possible to create new jobs easily. What needs to be done are the systems to support these, such as housing market, ability to share car keys with friends etc.

There might be many bugs as not all of the systems are properly tested yet and many of them are being implemented. Try looking at the code if you wonder something.

When I have time I will try to document things better but for now I will focus on implementing the underlying systems that can be built upon.

Because of all things, the progress might sometimes be slow.

## Current features
- Account system in a way that one account can have multiple characters
- House ownership system
- Vehicle ownership and parking system
- Vehicle shop system with easy way to create new shops
- Dynamic weather system
- Profession system with the possibility of making new professions easily
- Faction system for factions like police and military
- Phone system with address book and the possibility of sending text messages and making phone calls (Not properly tested yet)

## Upcoming things
- Housing market (Ability to buy houses and sell them, currently needs to be added to database manually)
- Ability to put houses for rent
- Proper faction system for police, fire department etc.
- Insurance for vehicles, handling vehicle destruction... (Things like vehicle destruction is not really handled at all now)

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
	- It contains a test account with name **Test** and password **test123**
		- **Note:** Your GTA-MP name must match the account name currently. 
			- In the future, social club ID will be used instead.
	- It also contains a few characters on the account, their property ownerships and vehicle ownerships

5. Have **MySql.Data.dll**, **INIFileParser.dll** and **System.Data.dll** in the GTA-MP Server directory root

## Pictures of the game mode

### General pictures:
![UI10](Images/1.png)
![UI11](Images/3.png)
![UI12](Images/4.png)

### Buying a vehicle:
![UI1](Images/12.png)

### Login screen:
![UI2](Images/5.png)
![UI3](Images/6.png)

### Character selection:
![UI4](Images/2.png)

### Working as a garbage truck driver:
![UI5](Images/10.png)
![UI6](Images/11.png)

### Housing pictures:
![UI7](Images/7.png)
![UI8](Images/8.png)
![UI9](Images/9.png)

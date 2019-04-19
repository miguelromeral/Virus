# Virus! (by [Tranjis Games](https://www.tranjisgames.com/))

***Work in progress***

![](https://img.shields.io/badge/.NET%20Standard-2.0-blue.svg)
![](https://img.shields.io/badge/.NET%20Framework-4.6.1-blue.svg)
![](https://img.shields.io/badge/Windows%2010-16299-lightgrey.svg)

## :warning: Legal warning
This project is full free and it's only for personal purpose and in any case not for commercial uses. It's developed in my free time and a way to be enterteined between one project and another.

The main idea is by [Tranjis Games](https://www.tranjisgames.com/juegos-de-mesa/juego-de-cartas-virus/), in association with [Domingo Cabrero](https://www.linkedin.com/in/domingocabrero/), Carlos López & [Santi Santisteban](https://twitter.com/sanzellos), the creators. The design is from Santi Santisteban and the artwork is made by [David GJ.](www.davidgj.com)

You can buy the game which is based this project on in the official shop: [official shop](https://www.tranjisgames.com/tienda/juego-de-mesa-virus/)


## :black_joker: Game rules

You can download the official rules in this [link](https://www.tranjisgames.com/wp-content/uploads/2017/02/VIRUS-RULES-eng.pdf).

You can also watch [this video](https://www.youtube.com/watch?v=paYfRh3r7qw) in which rules are explained (it's in spanish, so you'll have to enable subtitles).

## :computer: Project description

**Virus.Core** is the main project and the other ones depends on it. It contains the main classes, procedures and structure to make the game functuonality.
It's a class library in .NET Standard Framework 2.0, so you could download and use it on your own projects.

**Virus.Automatic** is a Console App project created to make the commputer plays by itself. You only will have to press a key between each move to watch the progress of the game.

**Virus.Manual is another** Console App project in which you can play against the computer by typing your commands on each turn you move.

**Virus.Universal** will be the project for Universal Windows Platform app. *Not implemented for the moment.*

### :camera: Screenshots

    Deck (20) | Discards Stack (32) | Total in game: (74)
    
    Turn # 31
    [            Player 0 | IA:     Medium]
    1.  (     Red Organ:         )
    2.  (    Blue Organ:     (B@))
    3.  (   Green Organ:         )
    
    [            Player 1 | IA:     Medium]
    1.  (    Blue Organ: (^*)(^*))
    
    ---> [            Player 2 | IA:     Medium]
    1.  (     Red Organ:         )
    
    [            Player 3 | IA:      First]
    1.  (    Blue Organ:         )
    2.  (   Green Organ:         )
    
    1.- Yellow Organ
    2.- ^ Organ
    3.- Green Virus
    
    Turn #31 (Player 2).
    Press any key to continue.

*Taken in Virus.Automatic, 4/19/2019*

## :pencil: TODO

* :arrow_forward: Documented
* :heavy_check_mark: Be able to read configuration of the game from a file (like Linux .conf files)
* :heavy_check_mark: AI to be able to play any card on the deck.
* :arrow_forward: Refactor code to have the main actions unified (i.e.: not have three different snippets that discard cards. Better have one and its appropiate log call).
* :o: Create string resources to allow more than one language.
* :arrow_forward: Finish to fill Logger calls to file.
* :arrow_forward: Implement AI in all levels (easy, medium, hard) and its strategies.
* :o: Create statistics from each move played.
* :arrow_forward: Create an ConsoleApp to allow human interaction.
* :o: GUI in UWP with XAML.
* :o: MVC Application with HTML and Javascript.
* :o: GUI in Unity?

## :envelope: Contact

Tranjis Games: <info@tranjisgames.com>

Developed by MiguelRomeral: [Twitter](https://twitter.com/miguelromeral), [LinkedIn](https://www.linkedin.com/in/miguelromeral/)

### License

© All rights reserved to Tranjis Games. 

This project has a GNU GPLv3 License
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
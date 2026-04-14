UPDATE Locations SET NorthID = -1, SouthID = -1, EastID = -1, WestID = -1;

-- 2. Insert the rooms if they don't exist yet (Ensures IDs 1 through 12 exist)
IF NOT EXISTS (SELECT 1 FROM Locations WHERE ID = 1) INSERT INTO Locations (ID, Name, ImagePath) VALUES (1, 'Cell', '/Assets/Images/Cell.png');
IF NOT EXISTS (SELECT 1 FROM Locations WHERE ID = 2) INSERT INTO Locations (ID, Name, ImagePath) VALUES (2, 'Hallway', '/Assets/Images/hallway.png');
IF NOT EXISTS (SELECT 1 FROM Locations WHERE ID = 3) INSERT INTO Locations (ID, Name, ImagePath) VALUES (3, 'Room2', '/Assets/Images/room2.png');
IF NOT EXISTS (SELECT 1 FROM Locations WHERE ID = 4) INSERT INTO Locations (ID, Name, ImagePath) VALUES (4, 'Stairs', '/Assets/Images/Stairs.png');
IF NOT EXISTS (SELECT 1 FROM Locations WHERE ID = 5) INSERT INTO Locations (ID, Name, ImagePath) VALUES (5, 'FellaOnTheStairs', '/Assets/Images/fellaOnTheStairs.png');
IF NOT EXISTS (SELECT 1 FROM Locations WHERE ID = 6) INSERT INTO Locations (ID, Name, ImagePath) VALUES (6, 'DarkSpookyHallway', '/Assets/Images/darkSpookyHallway.png');
IF NOT EXISTS (SELECT 1 FROM Locations WHERE ID = 7) INSERT INTO Locations (ID, Name, ImagePath) VALUES (7, 'Library', '/Assets/Images/Library.png');
IF NOT EXISTS (SELECT 1 FROM Locations WHERE ID = 8) INSERT INTO Locations (ID, Name, ImagePath) VALUES (8, 'AnotherBedroom', '/Assets/Images/AnotherBedroom.png');
IF NOT EXISTS (SELECT 1 FROM Locations WHERE ID = 9) INSERT INTO Locations (ID, Name, ImagePath) VALUES (9, 'JumpScareFrame1', '/Assets/Images/JumpScareFrame1.png');
IF NOT EXISTS (SELECT 1 FROM Locations WHERE ID = 10) INSERT INTO Locations (ID, Name, ImagePath) VALUES (10, 'JumpScareFrame2', '/Assets/Images/jumpScareFrame2.png');
IF NOT EXISTS (SELECT 1 FROM Locations WHERE ID = 11) INSERT INTO Locations (ID, Name, ImagePath) VALUES (11, 'YellowHallwayShadow', '/Assets/Images/yellowHallwayShadow.png');
IF NOT EXISTS (SELECT 1 FROM Locations WHERE ID = 12) INSERT INTO Locations (ID, Name, ImagePath) VALUES (12, 'ScaryMirror', '/Assets/Images/scaryMirror.png');

UPDATE Locations SET NorthID = 2, SouthID = -1, EastID = -1, WestID = -1 WHERE ID = 1;
UPDATE Locations SET NorthID = 3, SouthID = 1, EastID = 4, WestID = -1 WHERE ID = 2;
UPDATE Locations SET NorthID = -1, SouthID = 2, EastID = -1, WestID = -1 WHERE ID = 3;
UPDATE Locations SET NorthID = 5, SouthID = -1, EastID = -1, WestID = 2 WHERE ID = 4;
UPDATE Locations SET NorthID = -1, SouthID = 4, EastID = 6, WestID = -1 WHERE ID = 5;
UPDATE Locations SET NorthID = 9, SouthID = 5, EastID = 8, WestID = 7 WHERE ID = 6;
UPDATE Locations SET NorthID = -1, SouthID = -1, EastID = 6, WestID = -1 WHERE ID = 7;
UPDATE Locations SET NorthID = -1, SouthID = -1, EastID = -1, WestID = 6 WHERE ID = 8;
UPDATE Locations SET NorthID = 10, SouthID = 10, EastID = 10, WestID = 10 WHERE ID = 9;
UPDATE Locations SET NorthID = 11, SouthID = -1, EastID = -1, WestID = -1 WHERE ID = 10;
UPDATE Locations SET NorthID = 12, SouthID = 10, EastID = -1, WestID = -1 WHERE ID = 11;
UPDATE Locations SET NorthID = -1, SouthID = 11, EastID = -1, WestID = -1 WHERE ID = 12;
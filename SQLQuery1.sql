-- Insert the Starting Cell
INSERT INTO Locations (ID, Name, ImagePath, NorthID)
VALUES (1, 'The Starting Cell', '/Assets/Images/Cell.png', 2);

-- Insert the Hallway (Note: It connects to 1, 3, 4, 5)
INSERT INTO Locations (ID, Name, ImagePath, NorthID, SouthID, EastID)
VALUES (2, 'The Dark Hallway', '/Assets/Images/hallway.png', 3, 1, 4); 
-- Note: We initially block East (ID 4) in logic, but let's store it here.

-- Insert the Ritual Room
INSERT INTO Locations (ID, Name, ImagePath, SouthID)
VALUES (3, 'The Ritual Room', '/Assets/Images/room2.png', 2);

-- Insert the Library
INSERT INTO Locations (ID, Name, ImagePath, WestID)
VALUES (4, 'Abandoned Library', '/Assets/Images/Library.png', 2);

-- Insert the Stairs
INSERT INTO Locations (ID, Name, ImagePath, WestID, NorthID, EastID)
VALUES (5, 'The Grand Staircase', '/Assets/Images/Stairs.png', 4, 6, 7);

-- Insert the Bedroom
INSERT INTO Locations (ID, Name, ImagePath, SouthID)
VALUES (6, 'Master Bedroom', '/Assets/Images/AnotherBedroom.png', 5);

-- Insert the Mirror Room
INSERT INTO Locations (ID, Name, ImagePath, WestID)
VALUES (7, 'The Cracked Mirror', '/Assets/Images/scaryMirror.png', 5);
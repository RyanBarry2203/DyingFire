-- 1. CLEAR EXISTING DATA (Start with a clean slate)
DELETE FROM GameItems;
DELETE FROM Interactables;
DELETE FROM Locations;

-- Reset identity counters so IDs start fresh at 1
DBCC CHECKIDENT ('GameItems', RESEED, 0);
DBCC CHECKIDENT ('Interactables', RESEED, 0);

-- 2. INSERT LOCATIONS (The Map)
-- Columns: ID, Name, ImagePath, NorthID, EastID, SouthID, WestID
INSERT INTO Locations (ID, Name, ImagePath, NorthID, EastID, SouthID, WestID) VALUES
(1, 'Bed Room', '/Assets/Images/Cell.png', 2, -1, -1, -1),
(2, 'Dark Hall', '/Assets/Images/hallway.png', 3, -1, 1, -1), -- East is -1 because the Door handles it
(3, 'Ritual Room', '/Assets/Images/room2.png', -1, -1, 2, -1),
(4, 'Stairs', '/Assets/Images/Stairs.png', 5, -1, -1, 2), -- West goes back to Dark Hall
(5, 'Fella on Stairs', '/Assets/Images/fellaOnTheStairs.png', -1, -1, 4, 6),
(6, 'Dark Spooky Hallway', '/Assets/Images/darkSpookyHallway.png', 7, 5, -1, -1),
(7, 'Another Bedroom', '/Assets/Images/AnotherBedroom.png', -1, -1, 8, -1), -- South triggers the Jumpscare!
(8, 'JumpScare F1', '/Assets/Images/JumpScareFrame1.png', 9, 9, 9, 9), -- Any direction goes to F2
(9, 'JumpScare F2', '/Assets/Images/jumpScareFrame2.png', -1, -1, 10, -1),
(10, 'Yellow Hallway Shadow', '/Assets/Images/yellowHallwayShadow.png', -1, -1, 11, -1),
(11, 'Scary Mirror', '/Assets/Images/scaryMirror.png', 10, -1, -1, -1);

-- 3. INSERT THE LOCKED DOOR
-- Placed in the Dark Hall (ParentLocationID = 2). Teleports to Stairs (TargetLocationID = 4)
INSERT INTO Interactables (Name, X, Y, Width, Height, IsLocked, RequiredItem, LockedMessage, TargetLocationID, ParentLocationID)
VALUES ('Heavy Wooden Door', 700, 150, 150, 300, 1, 'Rusty Key', 'The door is locked tight. There is a rusty keyhole.', 4, 2);

-- 4. INSERT THE KEY
-- Placed on the floor of the Ritual Room (ParentLocationID = 3) so the player has to explore North first.
-- Type 0 = Key (Based on your ItemType Enum)
INSERT INTO GameItems (Name, Description, Type, ImagePath, ParentLocationID, ParentInteractableID)
VALUES ('Rusty Key', 'An old, blood-stained key.', 0, NULL, 3, NULL);
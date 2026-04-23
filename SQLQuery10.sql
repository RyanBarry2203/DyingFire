DELETE FROM [dbo].[GameItems];
DELETE FROM [dbo].[Interactables];
DELETE FROM [dbo].[Locations];
DBCC CHECKIDENT ('[dbo].[Interactables]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[GameItems]', RESEED, 0);

-- 2. Build the Map (Exactly matching your notebook drawing)
INSERT INTO [dbo].[Locations] ([ID], [Name], [ImagePath], [Description], [IsDark], [NorthID], [EastID], [SouthID], [WestID])
VALUES 
(1, 'Cell', '/Assets/Images/Cell.png', 'A cold, damp cell. This is where you woke up.', 0, 2, -1, -1, -1),
(2, 'Hallway', '/Assets/Images/hallway.png', 'A long, dreary hallway.', 0, 4, -1, 1, -1), -- East is -1 because the DOOR handles moving East
(3, 'Stairs', '/Assets/Images/Stairs.png', 'A set of stairs leading up into the darkness.', 0, 5, -1, -1, 2),
(4, 'Room 2', '/Assets/Images/room2.png', 'An abandoned room.', 0, -1, -1, 2, -1),
(5, 'Fella on the Stairs', '/Assets/Images/fellaOnTheStairs.png', 'There is someone... or something... here.', 0, -1, -1, 6, -1),
(6, 'Dark Spooky Hallway', '/Assets/Images/darkSpookyHallway.png', 'It is pitch black. You feel your sanity draining.', 1, 9, 8, -1, 7),
(7, 'Another Bedroom', '/Assets/Images/AnotherBedroom.png', 'A ruined bedroom.', 0, -1, 6, -1, -1),
(8, 'Library', '/Assets/Images/Library.png', 'Books are scattered everywhere. You hear breathing.', 0, -1, -1, -1, 6),
(9, 'JumpScare Frame 1', '/Assets/Images/JumpScareFrame1.png', 'AHHH!', 0, 10, -1, 6, -1),
(10, 'JumpScare Frame 2', '/Assets/Images/jumpScareFrame2.png', 'AHHH!', 0, 11, -1, 9, -1),
(11, 'Yellow Hallway Shadow', '/Assets/Images/yellowHallwayShadow.png', 'A sickly yellow light casts long shadows.', 0, 12, 8, 10, 8),
(12, 'Scary Mirror', '/Assets/Images/scaryMirror.png', 'Your reflection doesn''t look quite right.', 0, -1, -1, 11, -1);

-- 3. Create Interactables (A Chest and a Door)
-- ID 1: A Chest in the Cell (ParentLocationID = 1)
INSERT INTO [dbo].[Interactables] ([Name], [X], [Y], [Width], [Height], [IsLocked], [TargetLocationID], [ParentLocationID])
VALUES ('Old Chest', 600, 400, 150, 100, 0, 0, 1);

-- ID 2: The Door in the Hallway (ParentLocationID = 2). TargetLocationID = 3 means it teleports you to the Stairs!
INSERT INTO [dbo].[Interactables] ([Name], [X], [Y], [Width], [Height], [IsLocked], [RequiredItem], [LockedMessage], [TargetLocationID], [ParentLocationID])
VALUES ('Stairwell Door', 380, 150, 150, 350, 1, 'Rusty Key', 'The door is locked tight. It needs a key.', 3, 2);

-- 4. Put the Key INSIDE the Chest
-- ParentInteractableID = 1 means it goes inside the 'Old Chest' we just made.
INSERT INTO [dbo].[GameItems] ([Name], [Description], [Type], [ParentInteractableID])
VALUES ('Rusty Key', 'An old iron key. Might open a door nearby.', 0, 1);
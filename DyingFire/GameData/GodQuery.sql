-- 1. Create the Tables (Matching your C# Models)
CREATE TABLE [dbo].[Locations] (
    [ID]          INT            NOT NULL PRIMARY KEY,
    [Name]        NVARCHAR (100) NULL,
    [ImagePath]   NVARCHAR (255) NULL,
    [Description] NVARCHAR (MAX) NULL,
    [NoiseLevel]  INT            DEFAULT 0 NOT NULL,
    [IsDark]      BIT            DEFAULT 0 NOT NULL,
    [NorthID]     INT            DEFAULT (-1) NOT NULL,
    [EastID]      INT            DEFAULT (-1) NOT NULL,
    [SouthID]     INT            DEFAULT (-1) NOT NULL,
    [WestID]      INT            DEFAULT (-1) NOT NULL
);

CREATE TABLE [dbo].[Interactables] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [Name]             NVARCHAR (100) NULL,
    [X]                FLOAT (53)     NOT NULL,
    [Y]                FLOAT (53)     NOT NULL,
    [Width]            FLOAT (53)     NOT NULL,
    [Height]           FLOAT (53)     NOT NULL,
    [IsLocked]         BIT            DEFAULT 0 NOT NULL,
    [RequiredItem]     NVARCHAR (100) NULL,
    [LockedMessage]    NVARCHAR (255) NULL,
    [TargetLocationID] INT            DEFAULT 0 NOT NULL,
    [ParentLocationID] INT            NULL -- Links to Locations.ID
);

CREATE TABLE [dbo].[GameItems] (
    [ID]                   INT            IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [Name]                 NVARCHAR (100) NULL,
    [Description]          NVARCHAR (MAX) NULL,
    [ImagePath]            NVARCHAR (255) NULL,
    [Type]                 INT            NOT NULL, -- 0:Key, 1:Weapon, 2:Consumable, 3:Clue
    [ParentLocationID]     INT            NULL, -- If it's on the floor
    [ParentInteractableID] INT            NULL  -- If it's inside a chest
);

-- 2. Insert the Map Data (Based on your notebook drawing!)
-- Note: ID 6 is DarkSpookyHallway (your C# code looks for this)
-- Note: ID 8 is Library (your C# code spawns the monster here)

INSERT INTO [dbo].[Locations] ([ID], [Name], [ImagePath], [Description], [IsDark], [NorthID], [EastID], [SouthID], [WestID])
VALUES 
(1, 'Cell', '/Assets/Images/Cell.png', 'A cold, damp cell. This is where you woke up.', 0, 2, -1, -1, -1),
(2, 'Hallway', '/Assets/Images/hallway.png', 'A long, dreary hallway.', 0, 4, 3, 1, -1),
(3, 'Stairs', '/Assets/Images/Stairs.png', 'A set of stairs leading up into the darkness.', 0, 5, -1, -1, 2),
(4, 'Room 2', '/Assets/Images/room2.png', 'An abandoned room.', 0, -1, -1, 2, -1),
(5, 'Fella on the Stairs', '/Assets/Images/fellaOnTheStairs.png', 'There is someone... or something... here.', 0, -1, -1, 6, 3),
(6, 'Dark Spooky Hallway', '/Assets/Images/darkSpookyHallway.png', 'It is pitch black. You feel your sanity draining.', 1, 9, 8, 11, 7),
(7, 'Another Bedroom', '/Assets/Images/AnotherBedroom.png', 'A ruined bedroom.', 0, -1, 6, -1, -1),
(8, 'Library', '/Assets/Images/Library.png', 'Books are scattered everywhere. You hear breathing.', 0, 10, 10, 10, 10),
(9, 'JumpScare Frame 1', '/Assets/Images/JumpScareFrame1.png', 'AHHH!', 0, -1, -1, 6, -1),
(10, 'JumpScare Frame 2', '/Assets/Images/jumpScareFrame2.png', 'AHHH!', 0, 11, -1, -1, -1),
(11, 'Yellow Hallway Shadow', '/Assets/Images/yellowHallwayShadow.png', 'A sickly yellow light casts long shadows.', 0, 6, -1, 12, -1),
(12, 'Scary Mirror', '/Assets/Images/scaryMirror.png', 'Your reflection doesn''t look quite right.', 0, 11, -1, -1, -1);

-- 3. Insert a test item and interactable (Door from Hallway to Stairs)
INSERT INTO [dbo].[Interactables] ([Name], [X], [Y], [Width], [Height], [IsLocked], [RequiredItem], [LockedMessage], [ParentLocationID])
VALUES ('Stairwell Door', 400, 200, 100, 200, 1, 'Rusty Key', 'The door to the stairs is locked tight.', 2);

INSERT INTO [dbo].[GameItems] ([Name], [Description], [Type], [ParentLocationID])
VALUES ('Rusty Key', 'An old iron key. Might open a door nearby.', 0, 1);
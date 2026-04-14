-- 1. Add the Lore column to GameItems if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GameItems') AND name = 'Lore')
BEGIN
    ALTER TABLE GameItems ADD Lore NVARCHAR(MAX);
END
GO -- <--- THIS IS THE FIX. It forces SQL to create the column before reading the next lines.

-- 2. Clear old test data so we don't get duplicates
DELETE FROM GameItems;
DELETE FROM Interactables;
GO

-- 3. Create the Chest in the Bedroom (Location ID 8)
INSERT INTO Interactables (ID, Name, X, Y, Width, Height, IsLocked, ParentLocationID) 
VALUES (1, 'Old Trunk', 480, 350, 220, 150, 0, 8);
GO

-- 4. Put items INSIDE the chest (ParentInteractableID = 1)
INSERT INTO GameItems (Name, Description, Lore, Type, ParentInteractableID) 
VALUES (
    'Torn Journal Page', 
    'A crumpled piece of paper with frantic handwriting.', 
    '"September 4th. The noises in the walls are getting louder. I locked the door, but I don''t think it matters anymore. The shadows are moving on their own."', 
    3, 
    1
);

INSERT INTO GameItems (Name, Description, Lore, Type, ParentInteractableID) 
VALUES (
    'Heavy Iron Key', 
    'A cold, rusted key.', 
    'Forged decades ago. It bears the crest of the original estate owner. It feels unnaturally heavy in your hand.', 
    0, 
    1
);
GO
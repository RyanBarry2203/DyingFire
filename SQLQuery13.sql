UPDATE [dbo].[Interactables] 
SET X = 520, Y = 420, Width = 220, Height = 150 
WHERE Name = 'Old Chest';

-- Move the Door hitbox off the center path and onto the actual wooden door on the right wall
UPDATE [dbo].[Interactables] 
SET X = 720, Y = 150, Width = 160, Height = 450 
WHERE Name = 'Stairwell Door';
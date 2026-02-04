-- 3. INSERT DATA: The Locked Door (In Hallway/ID 2, Leads to Stairs/ID 5)
INSERT INTO Interactables (Name, X, Y, Width, Height, IsLocked, RequiredItem, LockedMessage, TargetLocationID, ParentLocationID)
VALUES ('Reinforced Oak Door', 100, 150, 180, 350, 1, 'Rusty Key', 'The keyhole looks jagged.', 5, 2);

-- 4. INSERT DATA: The Chest (In Ritual Room/ID 3)
INSERT INTO Interactables (Name, X, Y, Width, Height, IsLocked, RequiredItem, LockedMessage, TargetLocationID, ParentLocationID)
VALUES ('Old Chest', 425, 400, 200, 100, 0, NULL, NULL, 0, 3);

-- 5. INSERT DATA: The Key (Inside the Chest - Note ParentInteractableID = 2 because the chest is the 2nd item we made)
INSERT INTO GameItems (Name, Description, Type, ImagePath, ParentInteractableID)
VALUES ('Rusty Key', 'An old iron key.', 0, '/Assets/Images/gold_frame.png', 2);

-- 6. INSERT DATA: The Note (In Library/ID 4)
INSERT INTO GameItems (Name, Description, Type, ImagePath, ParentLocationID)
VALUES ('Torn Journal Page', 'Day 4: I can hear them in the walls...', 3, '/Assets/Images/scroll_icon.png', 4);

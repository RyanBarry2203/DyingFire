ALTER TABLE [dbo].[GameItems] ADD [Lore] NVARCHAR(MAX) NULL;
GO

-- 2. Insert the items into the hitboxes we made earlier
DECLARE @MattressID INT = (SELECT ID FROM Interactables WHERE Name = 'Cell Mattress');
INSERT INTO [dbo].[GameItems] ([Name], [Description], [Type], [ParentInteractableID], [Lore])
VALUES ('Crumpled Note', 'A hastily written note.', 3, @MattressID, '"If anyone finds this, don''t go into the dark hallway. It hears you when you step in the dark."');

DECLARE @TableID INT = (SELECT ID FROM Interactables WHERE Name = 'Dining Table');
INSERT INTO [dbo].[GameItems] ([Name], [Description], [Type], [ParentInteractableID], [Lore])
VALUES ('Investigator Journal', 'Torn page from a leather journal.', 3, @TableID, '"The entity feeds on fear and noise. I must stay quiet. I left my Spirit Box in the nursery upstairs..."');

DECLARE @BenchID INT = (SELECT ID FROM Interactables WHERE Name = 'Workbench');
INSERT INTO [dbo].[GameItems] ([Name], [Description], [Type], [ParentInteractableID], [Lore])
VALUES ('Heavy Wrench', 'A solid iron wrench.', 1, @BenchID, 'Covered in old, dried grease. Could be used as a weapon or to turn a rusted valve.');

DECLARE @DresserID INT = (SELECT ID FROM Interactables WHERE Name = 'Nursery Dresser');
INSERT INTO [dbo].[GameItems] ([Name], [Description], [Type], [ParentInteractableID], [Lore])
VALUES ('Spirit Box', 'A modified radio used to communicate with the dead.', 3, @DresserID, 'It emits a low hum even when turned off. Use it to listen to the other side.');
-- Fix DarkSpookyHallway (ID 6)
-- North -> 9 (Frame 1), West -> 7 (Library), East -> 8 (AnotherBedroom)
UPDATE Locations SET NorthID = 9, SouthID = 5, EastID = 8, WestID = 7 WHERE ID = 6;

-- Fix JumpScareFrame1 (ID 9)
-- ANY direction -> 10 (Frame 2)
UPDATE Locations SET NorthID = 10, SouthID = 10, EastID = 10, WestID = 10 WHERE ID = 9;
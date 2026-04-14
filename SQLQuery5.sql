UPDATE Locations SET NorthID = -1, SouthID = -1, EastID = -1, WestID = -1;

-- 2. Apply the corrected directional mapping
UPDATE Locations SET NorthID = 2 WHERE ID = 1;
UPDATE Locations SET NorthID = 3, SouthID = 1, EastID = 4 WHERE ID = 2;
UPDATE Locations SET SouthID = 2 WHERE ID = 3;
UPDATE Locations SET NorthID = 5, WestID = 2 WHERE ID = 4;
UPDATE Locations SET SouthID = 4, EastID = 6 WHERE ID = 5;

-- 6: DarkSpookyHallway (East -> 7 Library, West -> 8 AnotherBedroom, North -> 9 Frame1, South -> 5 Fella)
UPDATE Locations SET NorthID = 9, SouthID = 5, EastID = 7, WestID = 8 WHERE ID = 6;

-- 7: Library (West -> 6 DarkSpooky)
UPDATE Locations SET WestID = 6 WHERE ID = 7;

-- 8: AnotherBedroom (East -> 6 DarkSpooky)
UPDATE Locations SET EastID = 6 WHERE ID = 8;

-- 9: JumpScareFrame1 (North/East/West -> 10 Frame2, South -> 6 DarkSpooky)
UPDATE Locations SET NorthID = 10, SouthID = 6, EastID = 10, WestID = 10 WHERE ID = 9;

-- 10: JumpScareFrame2 (North -> 11 Yellow, South -> 9 Frame1)
UPDATE Locations SET NorthID = 11, SouthID = 9 WHERE ID = 10;

-- 11: YellowHallwayShadow (North -> 12 Mirror, South -> 10 Frame2)
UPDATE Locations SET NorthID = 12, SouthID = 10 WHERE ID = 11;

-- 12: ScaryMirror (South -> 11 Yellow)
UPDATE Locations SET SouthID = 11 WHERE ID = 12;
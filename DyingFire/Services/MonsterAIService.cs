using System;
using System.Collections.Generic;
using System.Linq;
using DyingFire.Models;

namespace DyingFire.Services
{
    public class MonsterAIService
    {
        public int MonsterLocationID { get; private set; } = 8;

        // Events to tell the ViewModel what happened
        public event Action OnJumpscareTriggered;
        public event Action<int> OnMonsterMoved;

        public void MoveMonster(List<Location> allLocations, int playerLocationID, GameState currentState)
        {
            var monsterRoom = allLocations.FirstOrDefault(x => x.ID == MonsterLocationID);
            if (monsterRoom == null) return;

            // If player is hiding, monster loses track and wanders randomly
            if (currentState == GameState.Hiding)
            {
                WanderRandomly(allLocations, monsterRoom);
                return;
            }

            var adjacentIds = new List<int> { monsterRoom.LocationToNorth, monsterRoom.LocationToSouth, monsterRoom.LocationToEast, monsterRoom.LocationToWest };
            adjacentIds.RemoveAll(id => id == -1);
            var adjacentRooms = allLocations.Where(x => adjacentIds.Contains(x.ID)).ToList();

            // Track noise
            var targetRoom = adjacentRooms.OrderByDescending(x => x.NoiseLevel).FirstOrDefault();

            if (targetRoom != null && targetRoom.NoiseLevel > 0)
            {
                MonsterLocationID = targetRoom.ID;
            }
            else if (adjacentRooms.Count > 0)
            {
                WanderRandomly(allLocations, monsterRoom);
            }

            OnMonsterMoved?.Invoke(MonsterLocationID);

            if (MonsterLocationID == playerLocationID)
            {
                OnJumpscareTriggered?.Invoke();
            }
        }

        private void WanderRandomly(List<Location> allLocations, Location currentRoom)
        {
            var adjacentIds = new List<int> { currentRoom.LocationToNorth, currentRoom.LocationToSouth, currentRoom.LocationToEast, currentRoom.LocationToWest };
            adjacentIds.RemoveAll(id => id == -1);
            var adjacentRooms = allLocations.Where(x => adjacentIds.Contains(x.ID)).ToList();

            if (adjacentRooms.Count > 0)
            {
                Random rnd = new Random();
                MonsterLocationID = adjacentRooms[rnd.Next(adjacentRooms.Count)].ID;
            }
        }

        public void ResetMonster()
        {
            MonsterLocationID = 1; // Reset to start or a specific room
        }
    }
}
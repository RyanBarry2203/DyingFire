using System;
using System.Collections.Generic;
using System.Linq;
using DyingFire.Models;

namespace DyingFire.Services
{
    public class MonsterAIService
    {
        public int MonsterLocationID { get; private set; } = 8; // Starts in Library

        private int _searchWaitTime = 0;
        private Random _rnd = new Random();

        public event Action OnJumpscareTriggered;
        public event Action<int> OnMonsterMoved;

        public void MoveMonster(List<Location> allLocations, int playerLocationID, GameState currentState)
        {
            var monsterRoom = allLocations.FirstOrDefault(x => x.ID == MonsterLocationID);
            if (monsterRoom == null) return;

            if (_searchWaitTime > 0)
            {
                _searchWaitTime--;
                return;
            }

            if (currentState == GameState.Hiding)
            {
                WanderRandomly(allLocations, monsterRoom);
            }
            else
            {
                var adjacentIds = new List<int> { monsterRoom.LocationToNorth, monsterRoom.LocationToSouth, monsterRoom.LocationToEast, monsterRoom.LocationToWest };
                adjacentIds.RemoveAll(id => id == -1);
                var adjacentRooms = allLocations.Where(x => adjacentIds.Contains(x.ID)).ToList();

                var targetRoom = adjacentRooms.OrderByDescending(x => x.NoiseLevel).FirstOrDefault();

                if (targetRoom != null && targetRoom.NoiseLevel > 0) MonsterLocationID = targetRoom.ID;
                else if (adjacentRooms.Count > 0) WanderRandomly(allLocations, monsterRoom);
            }

            _searchWaitTime = _rnd.Next(5, 15);
            OnMonsterMoved?.Invoke(MonsterLocationID);

            if (MonsterLocationID == playerLocationID && currentState != GameState.Hiding)
            {
                TriggerJumpscare();
            }
        }

        private void WanderRandomly(List<Location> allLocations, Location currentRoom)
        {
            var adjacentIds = new List<int> { currentRoom.LocationToNorth, currentRoom.LocationToSouth, currentRoom.LocationToEast, currentRoom.LocationToWest };
            adjacentIds.RemoveAll(id => id == -1);
            var adjacentRooms = allLocations.Where(x => adjacentIds.Contains(x.ID)).ToList();

            if (adjacentRooms.Count > 0)
            {
                MonsterLocationID = adjacentRooms[_rnd.Next(adjacentRooms.Count)].ID;
            }
        }

        // Calculates exactly how many rooms away the monster is using Breadth-First Search
        public int GetDistance(List<Location> allLocations, int startId, int targetId)
        {
            if (startId == targetId) return 0;

            var queue = new Queue<(int id, int dist)>();
            var visited = new HashSet<int>();

            queue.Enqueue((startId, 0));
            visited.Add(startId);

            while (queue.Count > 0)
            {
                var curr = queue.Dequeue();
                if (curr.id == targetId) return curr.dist;

                var loc = allLocations.FirstOrDefault(l => l.ID == curr.id);
                if (loc != null)
                {
                    int[] neighbors = { loc.LocationToNorth, loc.LocationToEast, loc.LocationToSouth, loc.LocationToWest };
                    foreach (var n in neighbors)
                    {
                        if (n != -1 && !visited.Contains(n))
                        {
                            visited.Add(n);
                            queue.Enqueue((n, curr.dist + 1));
                        }
                    }
                }
            }
            return 999;
        }

        public void TriggerJumpscare()
        {
            OnJumpscareTriggered?.Invoke();
        }

        public void ResetMonster()
        {
            MonsterLocationID = 8;
            _searchWaitTime = 0;
        }
    }
}
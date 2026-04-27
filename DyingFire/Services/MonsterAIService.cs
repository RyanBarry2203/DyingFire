using System;
using System.Collections.Generic;
using System.Linq;
using DyingFire.Models;

namespace DyingFire.Services
{
    public class MonsterAIService
    {
        // The current room id where the monster is.
        // Other systems (for example GameLoop or MainViewModel) can read this or subscribe to OnMonsterMoved.
        // It starts in the Library (ID 8) by default.
        public int MonsterLocationID { get; private set; } = 8; // Starts in Library

        // Internal counter used to make the monster wait some turns before moving again.
        private int _searchWaitTime = 0;
        // Random instance used for wander behavior and timing randomness.
        private Random _rnd = new Random();

        // Event fired when a jumpscare should run.
        // The UI or game state manager subscribes to this to show the jumpscare screen.
        public event Action OnJumpscareTriggered;
        // Event fired when the monster moves. The int parameter is the new location id.
        // Subscribers use this to update UI or run additional logic when the monster changes rooms.
        public event Action<int> OnMonsterMoved;

        // Called by the GameLoop on a tick to let the monster decide where to go next.
        // allLocations is the world graph the monster can navigate.
        // playerLocationID is used to determine if the monster reached the player.
        // currentState is the player's current state (for example Hiding). Behavior changes when the player is hiding.
        public void MoveMonster(List<Location> allLocations, int playerLocationID, GameState currentState)
        {
            var monsterRoom = allLocations.FirstOrDefault(x => x.ID == MonsterLocationID);
            if (monsterRoom == null) return;

            // If we are in a wait period, decrement and skip movement this tick.
            if (_searchWaitTime > 0)
            {
                _searchWaitTime--;
                return;
            }

            // If the player is hiding, the monster wanders randomly instead of tracking sound.
            if (currentState == GameState.Hiding)
            {
                WanderRandomly(allLocations, monsterRoom);
            }
            else
            {
                // Collect adjacent room ids and filter out -1 (no-room).
                var adjacentIds = new List<int> { monsterRoom.LocationToNorth, monsterRoom.LocationToSouth, monsterRoom.LocationToEast, monsterRoom.LocationToWest };
                adjacentIds.RemoveAll(id => id == -1);
                var adjacentRooms = allLocations.Where(x => adjacentIds.Contains(x.ID)).ToList();

                // Prefer the adjacent room with the highest NoiseLevel.
                // NoiseLevel is set by other systems (for example player actions or room config).
                var targetRoom = adjacentRooms.OrderByDescending(x => x.NoiseLevel).FirstOrDefault();

                // Move into the noisy room if one exists and has noise, otherwise wander.
                if (targetRoom != null && targetRoom.NoiseLevel > 0) MonsterLocationID = targetRoom.ID;
                else if (adjacentRooms.Count > 0) WanderRandomly(allLocations, monsterRoom);
            }

            // After moving, pick a random wait time so the monster does not move every single tick.
                _searchWaitTime = _rnd.Next(5, 15);
            // Notify listeners that the monster moved.
            OnMonsterMoved?.Invoke(MonsterLocationID);

            // If the monster ended up in the player's room while the player is not hiding, trigger a jumpscare.
            if (MonsterLocationID == playerLocationID && currentState != GameState.Hiding)
            {
                TriggerJumpscare();
            }
        }

        // Move to a random adjacent room.
        // This helper reads the current room's neighbors and picks one at random.
        // It only changes MonsterLocationID. It does not notify listeners itself.
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

        // Calculates exactly how many rooms away the monster is using Breadth-First Search.
        // This is used by other systems to show distance feedback or make decisions based on how close the monster is.
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
            // Return a large number if no path exists.
            return 999;
        }

        // Public method to trigger the jumpscare event. Subscribers handle the visual and state changes.
        public void TriggerJumpscare()
        {
            OnJumpscareTriggered?.Invoke();
        }

        // Reset the monster back to its initial state.
        // Called when the game restarts or when you want to teleport the monster back to the starting room.
        public void ResetMonster()
        {
            MonsterLocationID = 8;
            _searchWaitTime = 0;
        }
    }
}
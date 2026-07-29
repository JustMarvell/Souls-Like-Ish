using System.Collections.Generic;
using UnityEngine;

namespace SoulsLikeIsh.Input
{
    public class InputBuffer
    {
        private readonly Dictionary<PlayerAction, float> _timestamps = new();

        public void Buffer(PlayerAction action) => _timestamps[action] = Time.time;

        public bool TryConsume(PlayerAction action, float window)
        {
            if (_timestamps.TryGetValue(action, out float t) && Time.time - t <= window)
            {
                _timestamps.Remove(action);
                return true;
            }
            return false;
        }
    }
}
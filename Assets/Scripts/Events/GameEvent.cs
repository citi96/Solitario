using System.Collections.Generic;
using UnityEngine;

namespace Events {
    [CreateAssetMenu(menuName = "Game Event")]
    public class GameEvent : ScriptableObject {
        private readonly List<GameEventListener> _listeners = new List<GameEventListener>();

        public void Register(GameEventListener l) {
            _listeners.Add(l);
        }

        public void UnRegister(GameEventListener l) {
            _listeners.Remove(l);
        }

        public void Raise() {
            foreach (var listener in _listeners) {
                listener.Response();
            }
        }
    }
}
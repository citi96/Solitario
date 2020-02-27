using UnityEngine;
using UnityEngine.Events;

namespace Events {
    public class GameEventListener : MonoBehaviour {
        public GameEvent gameEvent;
        public UnityEvent response;

        public virtual void OnEnableLogic() {
            if (gameEvent != null)
                gameEvent.Register(this);
        }

        private void OnEnable() {
            OnEnableLogic();
        }

        public virtual void OnDisableLogic() {
            if (gameEvent != null)
                gameEvent.UnRegister(this);
        }

        private void OnDisable() {
            OnDisableLogic();
        }

        public virtual void Response() {
            response.Invoke();
        }
    }
}
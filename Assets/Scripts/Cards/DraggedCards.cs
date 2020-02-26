using System.Collections;
using Columns;
using UnityEngine;

namespace Cards {
    public class DraggedCards : MonoBehaviour {
        private Vector3 _startingPosition;
        [SerializeField] private Canvas canvas;
        [SerializeField] private float spacing;

        public Canvas Canvas => canvas;

        public float Spacing => spacing;

        private void Awake() {
            _startingPosition = transform.position;
        }

        private void Update() {
            var position = transform.position;
            if (!position.z.Equals(canvas.transform.position.z)) {
                transform.position = new Vector3(position.x, position.y, canvas.transform.position.z);
            }

            if (transform.childCount == 0 && _startingPosition != transform.position) {
                transform.position = _startingPosition;
            }
        }

        private Vector2 GetTouchPositionInCanvas(Vector2 position) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas.transform as RectTransform, position, Canvas.worldCamera,
                out var pos);
            return Canvas.transform.TransformPoint(pos);
        }

        public void MoveCardsToColumn(Vector2 destination, float waiting, Column column) {
            for (int i = 0; i < transform.childCount; i++) {
                StartCoroutine(Move(destination, waiting, column, transform.GetChild(i)));
            }
        }

        private IEnumerator Move(Vector2 destination, float waiting, Column column, Transform card) {
            while (Vector2.Distance(transform.position, destination) >= 1f) {
                transform.position = Vector2.MoveTowards(transform.position, destination, Time.deltaTime * 1000);
                yield return new WaitForSeconds(waiting);
            }

            card.SetParent(column.transform);

            if (transform.childCount == 0) {
                column.UpdateColumn();
            }
        }

        public void MoveCards() {
            transform.position = Input.touchCount > 0
                ? GetTouchPositionInCanvas(Input.GetTouch(0).position)
                : GetTouchPositionInCanvas(Input.mousePosition);
        }

        public void AddCardToColumn(Column column) {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++) {
                transform.GetChild(0).SetParent(column.transform);
            }

            column.UpdateColumn();
        }
    }
}
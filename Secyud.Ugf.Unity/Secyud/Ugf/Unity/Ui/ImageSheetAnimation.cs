using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.Ui
{
    [RequireComponent(typeof(RawImage))]
    public class ImageSheetAnimation : MonoBehaviour
    {
        [SerializeField] private float _playTime = 1;
        [SerializeField] private int _columns = 1;
        [SerializeField] private int _rows = 1;

        private RawImage _image;
        private int _index;
        private float _time;

        private void Awake()
        {
            _image = GetComponent<RawImage>();
            _playTime /= _columns * _rows;
            Rect rect = _image.uvRect;
            rect.width = 1f / _columns;
            rect.height = 1f / _rows;
            _image.uvRect = rect;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            if (_time > _playTime)
            {
                _time %= _playTime;
                _index = (_index + 1) % (_columns * _rows);
                int col = _index % _columns;
                int row = _index / _columns;
                Rect rect = _image.uvRect;
                rect.x = rect.width * col;
                rect.y = rect.height * row;
                _image.uvRect = rect;
            }
        }
    }
}
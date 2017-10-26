
namespace UnityMVC
{
    public class Model
    {
        public delegate void EventHandler();
        public event EventHandler OnPositionUpdated;

        private Position _position = Point.Create();

        public Position position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position.Equals(value))
                    return;

                _position = value;

                if (OnPositionUpdated != null)
                    OnPositionUpdated();
            }
        }
    }
}
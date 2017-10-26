using UnityEngine;

public class MouseInputPorter : InputPorter
{
    private bool _oldClick = false;
    private Controller _currentTarget = null;

    public override void Tick(float delta)
    {
        base.Tick(delta);


        bool currentClick = Input.GetMouseButton(0);
        var position = Input.mousePosition;

        if (currentClick && !_oldClick)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(position);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                _currentTarget = hit.collider.gameObject.GetComponent<Controller>();
                _currentTarget.BeginTouching(position);
            }
        }
        else if (currentClick && _oldClick)
        {
            if (_currentTarget != null)
                _currentTarget.InTouching(position, delta);
        }
        else if (!currentClick && _oldClick)
        {
            if (_currentTarget != null)
                _currentTarget.FinishTouching(position, delta);

            _currentTarget = null;
        }

        _oldClick = currentClick;
    }
}
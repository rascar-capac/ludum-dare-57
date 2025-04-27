using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Level> _levels;
    [SerializeField] private Level _topTeleportationLevel;
    [SerializeField] private Level _bottomTeleportationLevel;
    [SerializeField] private Transform _elevatorTopTeleportationAnchor;
    [SerializeField] private Transform _elevatorBottomTeleportationAnchor;
    [SerializeField] private int _startingPlayerLevelIndex;
    [SerializeField] private int _startingPlayerSection;
    [SerializeField] private bool _positiveLevelsAreAllowed;

    private int _currentPlayerMetaLevel;
    private int _currentPlayerSection;

    public int CurrentPlayerMetaLevel => _currentPlayerMetaLevel;

    public void SetCurrentToUpperLevel()
    {
        RefreshCurrentPlayerLevelAndSection(-1);
    }

    public void SetCurrentToLowerLevel()
    {
        RefreshCurrentPlayerLevelAndSection(1);
    }

    public Level GetLevel(int index)
    {
        return _levels[index];
    }

    public bool CanGoUp()
    {
        return _positiveLevelsAreAllowed || _currentPlayerMetaLevel != 0 || _currentPlayerSection != 0;
    }

    public bool CanGoDown()
    {
        return true;
    }

    private void RefreshCurrentPlayerLevelAndSection(int movement)
    {
        _currentPlayerMetaLevel += movement;

        if (_currentPlayerMetaLevel > 9)
        {
            _currentPlayerMetaLevel -= _levels.Count;
            _currentPlayerSection++;
            RefreshLevelsInfos();
            TeleportElevatorAndPlayer(topToBottom: false);
            _topTeleportationLevel.RefreshInfos(_currentPlayerSection - 1);
        }
        else if (_currentPlayerMetaLevel < 0)
        {
            _currentPlayerMetaLevel += _levels.Count;
            _currentPlayerSection--;
            RefreshLevelsInfos();
            TeleportElevatorAndPlayer(topToBottom: true);
            _bottomTeleportationLevel.RefreshInfos(_currentPlayerSection + 1);
        }
    }

    private void RefreshLevelsInfos()
    {
        foreach (Level level in _levels)
        {
            level.RefreshInfos(_currentPlayerSection);
        }
    }

    private void TeleportElevatorAndPlayer(bool topToBottom)
    {
        Vector3 elevatorToPlayerOffset = Game.Player.transform.position - Game.Elevator.transform.position;
        Game.Elevator.transform.position = topToBottom ? _elevatorBottomTeleportationAnchor.position : _elevatorTopTeleportationAnchor.position;
        Game.Player.transform.position = Game.Elevator.transform.position + elevatorToPlayerOffset;
    }

    private void Awake()
    {
        _currentPlayerMetaLevel = _startingPlayerLevelIndex;
        _currentPlayerSection = _startingPlayerSection;
        RefreshLevelsInfos();
    }
}

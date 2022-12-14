using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

namespace PvP3DAction
{
    public class LevelManager : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI[] _scoreListText;
        [SerializeField] private TextMeshProUGUI _winnerText;
        [SerializeField] private float _restartTime;
        [SerializeField] private int _hitsToWin;

        private List<Player> _players = new();
        private Player _winner;

        private bool _isScoreReached;
        private float _restartTimer;

        private void Start()
        {
            _restartTimer = _restartTime;
        }

        private void Update()
        {
            if (_players.Count == 0) return;

            RpcViewScoreList();

            if (_isScoreReached)
            {
                _winnerText.transform.parent.gameObject.SetActive(true);

                _winnerText.text = $"{_winner.PlayerNameText}\nis winner!";

                _restartTimer -= Time.deltaTime;

                if (_restartTimer < 0)
                {
                    _winnerText.transform.parent.gameObject.SetActive(false);

                    RestartLevel();
                }

                return;
            }

            CheckScore();
        }

        [ClientRpc]
        public void RpcViewScoreList()
        {
            for (int i = 0; i < _players.Count; i++)
            {
                _scoreListText[i].text = $"{_players[i].PlayerNameText}:   {_players[i].DashHitsAmount}\n";
            }
        }

        private void CheckScore()
        {
            if (_isScoreReached == true) return;            

            foreach (Player player in _players)
            {
                if (player.DashHitsAmount >= _hitsToWin)
                {
                    _winner = player;

                    _isScoreReached = true;

                    return;
                }
            }
        }

        private void RestartLevel()
        {
            _isScoreReached = false;

            _restartTimer = _restartTime;

            foreach (Player player in _players)
            {
                player.transform.position = player.StartPosition;

                player.DashHitsReset();
            }
        }

        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }
    }
}
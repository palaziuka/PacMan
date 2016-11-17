﻿using NLog;
using PacMan.Logic.Concrete;
using PacMan.Logic.Model;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace PacMan.Logic.Logic
{
    public class GameLogic
    {

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private MoveMan _moveMan;
        private MoveBadBoy[] _moveBadBoy;
        private const int BadBoyQuality = 3;
        private readonly Grid _canvasHost;
        private bool _gridIsUsed;
        private Board _board;
        private const int BoardSize = 15;
        private Thread _thread;
        public GameLogic(Grid canvasHost)
        {
            _canvasHost = canvasHost;
            InicializeComponents();
            RunParallelThread();

        }
        public int Score => _moveMan.CurrentScore;

        public int Lifes => _moveMan.Lifes;

        public int Level { get; set; } = 1;

        private void InicializeComponents()
        {

            _moveBadBoy = new MoveBadBoy[BadBoyQuality];
            _board = new Board(BoardSize);
            _moveMan = new MoveMan(_board);
            for (int i = 0; i < BadBoyQuality; i++)
            {
                _moveBadBoy[i] = new MoveBadBoy(_board, _moveMan.Man);
            }


            _moveBadBoy[0].SetCoordinates(0, 0);
            _moveBadBoy[1].SetCoordinates(BoardSize - 1, BoardSize - 1);
            _moveBadBoy[2].SetCoordinates(0, BoardSize - 1);

            for (var i = 0; i < BadBoyQuality; i++)
            {
                _board.AddComponents(_moveBadBoy[i].GetCurrentX(), _moveBadBoy[i].GetCurrentY(), BoardElements.BadBoy);

            }
        }
        private void RunParallelThread()
        {
            _thread = new Thread(GamesLoop) {Name = "Move Bad Boys"};
            _log.Debug("Запуск потока {0}", _thread.Name);
            _thread.Start();

        }
        public void StartGame()
        {

            SetBoardComponents();



        }
        public bool IsPlay()
        {
            return Lifes > 0;
        }
        public bool ChangeLevel()
        {
            if (_board.QualityBonus > 0)
                return false;
            else
            {
                _log.Trace("Изменения свойства Level");
                return true;
            }
        }

        private void GamesLoop()
        {
            while (true)
            {
                while (_board.QualityBonus > 0)
                {
                    Thread.Sleep(800);
                    for (var i = 0; i < BadBoyQuality; i++)
                    {
                        _moveBadBoy[i].Stepping(i);
                        CheckCollision();
                    }
                }
                StartGame();
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void SetBoardComponents()
        {

            _canvasHost.Dispatcher.Invoke(() =>
            {
                _log.Trace("Старт добавления элементов на игральное поле");
                if (_gridIsUsed)
                {
                    ClearGrid();
                    _board.UpdateBoard();
                    _moveBadBoy[0].SetCoordinates(0, 0);
                    _moveBadBoy[1].SetCoordinates(BoardSize - 1, BoardSize - 1);
                    _moveBadBoy[2].SetCoordinates(0, BoardSize - 1);
                    for (var i = 0; i < BadBoyQuality; i++)
                    {
                        _board.AddComponents(_moveBadBoy[0].GetCurrentX(), _moveBadBoy[0].GetCurrentY(), BoardElements.BadBoy);
                    }
                    _moveMan.SetCoordinates();
                    _board.AddComponents(_moveMan.GetCurrentX(), _moveMan.GetCurrentY(), BoardElements.Man);
                }
                else
                {
                    for (var i = 0; i < BoardSize; i++)
                    {
                        _canvasHost.ColumnDefinitions.Add(new ColumnDefinition());
                        _canvasHost.RowDefinitions.Add(new RowDefinition());
                        _gridIsUsed = true;
                    }

                }

                for (var j = 0; j < BoardSize; j++)
                {
                    for (var i = 0; i < BoardSize; i++)
                    {
                        var c = new Canvas {Background = ColorManager.ChangeColor(_board.BoardElement[i, j])};
                        Grid.SetColumn(c, i);
                        Grid.SetRow(c, j);
                        _canvasHost.Children.Add(c);
                    }

                }
                _log.Trace("Элементы добавлены");

            });
        }
        private void ClearGrid()
        {

            _log.Trace("Старт очистки игрального поля");

            for (var i = 0; i < BoardSize; i++)
            {
                for (var j = 0; j < BoardSize; j++)
                {
                    _canvasHost.Children.Remove(_canvasHost.Children
                        .Cast<UIElement>()
                        .FirstOrDefault(item => Grid.GetColumn(item) == j && Grid.GetRow(item) == i));
                }
            }
            _log.Trace("игральное поле очищенно");

        }
        private void CheckCollision()
        {
            for (var i = 0; i < BadBoyQuality; i++)
            {
                if (_moveBadBoy[i].GetCurrentX() != _moveMan.GetCurrentX() ||
                    _moveBadBoy[i].GetCurrentY() != _moveMan.GetCurrentY()) continue;
                if (_moveBadBoy[i].LastStep == BoardElements.Bonus)
                {
                    _moveMan.AddBonusValue();
                    _moveBadBoy[i].LastStep = BoardElements.Way;
                }
                if (_moveBadBoy[i].LastStep == BoardElements.Man) _moveBadBoy[i].LastStep = BoardElements.Way;
                _log.Trace("Изменение свойства Lifes");

                _moveMan.Lifes--;
                _moveMan.SetCoordinates();
                _board.AddComponents(_moveMan.GetCurrentX(), _moveMan.GetCurrentY(), BoardElements.Man);
                ColorManager.ChangeElementColor(_moveMan.GetCurrentX(), _moveMan.GetCurrentY(), BoardElements.Man);
            }
        }

        public void MoveAction(Side key)
        {
            CheckCollision();
            _moveMan.MoveAction(key);
        }
        public void KillThread()
        {
            _log.Debug("Убит поток {0}", _thread.Name);
            _thread.Abort(); 
        }
    }
}

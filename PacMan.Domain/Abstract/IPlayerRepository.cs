﻿using PacMan.Domain.Entities;
using System.Collections.Generic;

namespace PacMan.Domain.Abstract
{
    public interface IPlayerRepository
    {
        IEnumerable<Player> Players { get; }
        void AddPlayer(Player player);
    }
}

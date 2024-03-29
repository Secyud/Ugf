﻿using System.Collections;

namespace Secyud.Ugf.Modularity
{
    public interface IGameModule
    {
        IEnumerator OnGameNewing();
        IEnumerator OnGameSaving();
        IEnumerator OnGameLoading();
    }
}
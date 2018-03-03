﻿namespace IngameScript.Core.ComponentModel
{
    public interface IComponent
    {
        string ComponentId { get; }

        void OnAttached(App app);
    }
}
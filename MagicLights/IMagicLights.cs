﻿using System.Threading.Tasks;

namespace MagicLights
{
    public interface IMagicLights
    {
        public Task Start();

        public Task Stop();
    }
}

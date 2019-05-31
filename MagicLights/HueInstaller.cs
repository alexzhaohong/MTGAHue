﻿using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using MagicLights.LightClients;

namespace MagicLights
{
    public class HueInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ILightClientProvider>()
                    .ImplementedBy<HueLightClientProvider>());
        }
    }
}
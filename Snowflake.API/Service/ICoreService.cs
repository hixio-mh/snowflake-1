﻿using System;
using System.Collections.Generic;
using Snowflake.Service.Manager;
using Snowflake.Service.HttpServer;
using Snowflake.Service.JSWebSocketServer;
using Snowflake.Emulator.Configuration;
using Snowflake.Emulator.Input.InputManager;
using Snowflake.Controller;
using Snowflake.Game;
using Snowflake.Platform;
namespace Snowflake.Service
{
    /// <summary>
    /// The core frontend service that handles all the functions of the frontend core.
    /// </summary>
    public interface ICoreService
    {
        /// <summary>
        /// The Ajax endpoint manager
        /// </summary>
        IAjaxManager AjaxManager { get; }
        /// <summary>
        /// The Emulator assemblies manager
        /// </summary>
        IEmulatorAssembliesManager EmulatorManager { get; }
        /// <summary>
        /// The directory from which application data is stored
        /// </summary>
        string AppDataDirectory { get; }
        /// <summary>
        /// The database that holds controller ports information
        /// </summary>
        IControllerPortsDatabase ControllerPortsDatabase { get; }
        /// <summary>
        /// The database that contains games information
        /// </summary>
        IGameDatabase GameDatabase { get; }
        /// <summary>
        /// The database that contains platform preferences
        /// </summary>
        IPlatformPreferenceDatabase PlatformPreferenceDatabase { get; }
        /// <summary>
        /// The database that contains the gamepad abstraction profiles
        /// </summary>
        IGamepadAbstractionDatabase GamepadAbstractionDatabase { get; }
        /// <summary>
        /// All currently loaded platforms
        /// </summary>
        IDictionary<string, IPlatformInfo> LoadedPlatforms { get; }
        /// <summary>
        /// All currently loaded controllers
        /// </summary>
        IDictionary<string, IControllerDefinition> LoadedControllers { get; }
        /// <summary>
        /// The plugin manager
        /// </summary>
        IPluginManager PluginManager { get; }
        /// <summary>
        /// The webserver manager
        /// </summary>
        IServerManager ServerManager { get; }
        /// <summary>
        /// The input device manager
        /// </summary>
        IInputManager InputManager { get; }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

// Simple script which creates a persistent singleton allowing us to keep any object attached between scenes
public class KeepBetweenScenes : PersistentSingleton<KeepBetweenScenes> {}

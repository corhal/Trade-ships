﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable {

	List<Action> Actions { get; }
	string Name { get; }
	int Level { get; }
}

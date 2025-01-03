﻿using Shiron.Manila.Ext;

namespace Shiron.Manila.ManilaCPP;

public class ManilaCPP : ManilaPlugin {
	public ManilaCPP() : base("shiron.manila", "manilacpp", "1.0.0") { }

	public override void init() {
		info(this);

		info("Registering components...");
		register(typeof(ConsoleApp));
	}

	public override void release() {
		Console.WriteLine("Releasing CPP...");
	}
}

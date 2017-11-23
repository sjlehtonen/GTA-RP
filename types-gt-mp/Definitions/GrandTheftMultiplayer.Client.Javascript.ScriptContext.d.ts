declare namespace GrandTheftMultiplayer.Client.Javascript.ScriptContext {

	enum Anchor {
		TopLeft = 0,
		TopCenter = 1,
		TopRight = 2,
		MiddleLeft = 3,
		MiddleCenter = 4,
		MiddleRight = 6,
		BottomLeft = 7,
		BottomCenter = 8,
		BottomRight = 9
	}

	class ChatEvent {
		constructor(object: any, method: any);
		Invoke(msg: string): void;
		BeginInvoke(msg: string, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class CustomDataReceived {
		constructor(object: any, method: any);
		Invoke(data: string, dataID: number): void;
		BeginInvoke(data: string, dataID: number, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class DataChangedEvent {
		constructor(object: any, method: any);
		Invoke(entity: GrandTheftMultiplayer.Client.Models.LocalHandle, key: string, oldValue: any): void;
		BeginInvoke(entity: GrandTheftMultiplayer.Client.Models.LocalHandle, key: string, oldValue: any, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class EmptyEvent {
		constructor(object: any, method: any);
		Invoke(): void;
		BeginInvoke(callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class EntityEvent {
		constructor(object: any, method: any);
		Invoke(entity: GrandTheftMultiplayer.Client.Models.LocalHandle): void;
		BeginInvoke(entity: GrandTheftMultiplayer.Client.Models.LocalHandle, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class EntityMoveEvent {
		constructor(object: any, method: any);
		Invoke(obj: GrandTheftMultiplayer.Client.Models.LocalHandle): void;
		BeginInvoke(obj: GrandTheftMultiplayer.Client.Models.LocalHandle, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class fArg {
		constructor(f: number);
	}

	class HudVisibilityChangeEvent {
		constructor(object: any, method: any);
		Invoke(visible: boolean): void;
		BeginInvoke(visible: boolean, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class IntChangeEvent {
		constructor(object: any, method: any);
		Invoke(oldValue: number): void;
		BeginInvoke(oldValue: number, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class PlayerDamageEvent {
		constructor(object: any, method: any);
		Invoke(attacker: GrandTheftMultiplayer.Client.Models.LocalHandle, weaponUsed: number, boneHit: number): void;
		BeginInvoke(attacker: GrandTheftMultiplayer.Client.Models.LocalHandle, weaponUsed: number, boneHit: number, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class PlayerKilledEvent {
		constructor(object: any, method: any);
		Invoke(killer: GrandTheftMultiplayer.Client.Models.LocalHandle, weapon: number): void;
		BeginInvoke(killer: GrandTheftMultiplayer.Client.Models.LocalHandle, weapon: number, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class PlayerMeleeDamageEvent {
		constructor(object: any, method: any);
		Invoke(attacker: GrandTheftMultiplayer.Client.Models.LocalHandle, weaponUsed: number): void;
		BeginInvoke(attacker: GrandTheftMultiplayer.Client.Models.LocalHandle, weaponUsed: number, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class Raycast {
		readonly didHitAnything: boolean;
		readonly didHitEntity: boolean;
		readonly hitEntity: GrandTheftMultiplayer.Client.Models.LocalHandle;
		readonly hitCoords: GrandTheftMultiplayer.Shared.Math.Vector3;
	}

	class ServerEventTrigger {
		constructor(object: any, method: any);
		Invoke(eventName: string, _arguments: any[]): void;
		BeginInvoke(eventName: string, _arguments: any[], callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class StreamEvent {
		constructor(object: any, method: any);
		Invoke(item: GrandTheftMultiplayer.Client.Models.LocalHandle, entityType: number): void;
		BeginInvoke(item: GrandTheftMultiplayer.Client.Models.LocalHandle, entityType: number, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class VehicleChangeEvent {
		constructor(object: any, method: any);
		Invoke(entity: GrandTheftMultiplayer.Client.Models.LocalHandle, seat: number): void;
		BeginInvoke(entity: GrandTheftMultiplayer.Client.Models.LocalHandle, seat: number, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class VehicleSeatChangeEvent {
		constructor(object: any, method: any);
		Invoke(entity: GrandTheftMultiplayer.Client.Models.LocalHandle, oldSeat: number, newSeat: number): void;
		BeginInvoke(entity: GrandTheftMultiplayer.Client.Models.LocalHandle, oldSeat: number, newSeat: number, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

	class WeaponShootEvent {
		constructor(object: any, method: any);
		Invoke(weaponUsed: number, aimCoords: GrandTheftMultiplayer.Shared.Math.Vector3): void;
		BeginInvoke(weaponUsed: number, aimCoords: GrandTheftMultiplayer.Shared.Math.Vector3, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): void;
	}

}

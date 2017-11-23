declare namespace GrandTheftMultiplayer.Client.Models {

	enum HandleType {
		GameHandle = 0,
		LocalHandle = 1,
		NetHandle = 2
	}

	class LocalHandle {
		readonly IsNull: boolean;
		readonly Raw: number;
		readonly Value: number;
		constructor(handle: number);
		constructor(handle: number, localId: GrandTheftMultiplayer.Client.Models.HandleType);
		Equals(obj: any): boolean;
		GetHashCode(): number;
		Properties<T>(): any;
		ToString(): string;
	}

	class ModelDimensions {
		Maximum: GrandTheftMultiplayer.Shared.Math.Vector3;
		Minimum: GrandTheftMultiplayer.Shared.Math.Vector3;
		constructor();
	}

}

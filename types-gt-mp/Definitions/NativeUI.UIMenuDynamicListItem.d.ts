declare namespace NativeUI.UIMenuDynamicListItem {

	enum ChangeDirection {
		Left = 0,
		Right = 1
	}

	class DynamicListItemChangeCallback {
		constructor(object: any, method: any);
		Invoke(sender: NativeUI.UIMenuDynamicListItem, direction: NativeUI.UIMenuDynamicListItem.ChangeDirection): string;
		BeginInvoke(sender: NativeUI.UIMenuDynamicListItem, direction: NativeUI.UIMenuDynamicListItem.ChangeDirection, callback: System.AsyncCallback, object: any): System.IAsyncResult;
		EndInvoke(result: System.IAsyncResult): string;
	}

}

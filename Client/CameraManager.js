/**
 * Class for managing cameras.
 * Currently used for the login camera.
 */
class CameraManager {
    constructor() {

    }

    handleCameraEvent(eventName, args) {
        if (eventName == "EVENT_SET_LOGIN_SCREEN_CAMERA") {
            let newCamera = API.createCamera(args[0], args[1]);
            API.setActiveCamera(newCamera);
        }
        else if (eventName == "EVENT_REMOVE_CAMERA") {
            API.setActiveCamera(null);
        }
    }
}

let cameraManager = new CameraManager();

API.onServerEventTrigger.connect(function (eventName, args) {
    cameraManager.handleCameraEvent(eventName, args);
});
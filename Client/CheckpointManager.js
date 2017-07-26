class CheckpointManager {
    constructor() {
        this.checkpoints = [];
        this.blips = [];
        this.blipsDict = new Map();
        this.checkpointsDict = new Map();
    }

    handleCreateCheckpointEvent(args) {
        this.createCheckpoint(args[0], args[1], args[2], args[3], args[4], args[5]);
    }

    handleDeleteCheckpointEvent(args) {
        this.deleteCheckpoint(args[0]);
    }

    deleteCheckpoint(id) {
        var entityBlip = this.blipsDict.get(id);
        var entityMarker = this.checkpointsDict.get(id);

        API.deleteEntity(entityBlip);
        API.deleteEntity(entityMarker);

        this.blipsDict.delete(id);
        this.checkpointsDict.delete(id);
    }

    createCheckpoint(id, position, type, c1, c2, c3) {
        var marker = API.createMarker(type, position, new Vector3(), new Vector3(), new Vector3(3, 3, 3), c1, c2, c3, 255);
        var blip = API.createBlip(position);

        this.blipsDict.set(id, blip);
        this.checkpointsDict.set(id, marker);
    }

    handleCheckpointEvent(eventName, args) {
        switch (eventName) {
            case 'EVENT_CREATE_CHECKPOINT':
                this.handleCreateCheckpointEvent(args);
                break;

            case 'EVENT_DELETE_CHECKPOINT':
                this.handleDeleteCheckpointEvent(args);
                break;

            default:
                break;
        }
    }
}

let checkpointManager = new CheckpointManager();

API.onServerEventTrigger.connect(function (eventName, args) {
    checkpointManager.handleCheckpointEvent(eventName, args);
});
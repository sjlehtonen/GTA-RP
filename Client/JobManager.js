class JobManager {
    constructor() {

    }

    handleJobEvent(eventName, args) {
        if (eventName == "EVENT_OPEN_TAKE_JOB_MENU") {
        }
        else if (eventName == "EVENT_CLOSE_TAKE_JOB_MENU") {
        }
    }

    createTakeJobMenu(jobId, jobName, salary, description) {

    }
}

let jobManager = new JobManager();
API.onServerEventTrigger.connect((eventName, args) => jobManager.handleJobEvent(eventName, args));
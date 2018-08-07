/**
 * Class for managing the jobs system.
 * Things like job menus should be put here.
 */
class JobManager {
    constructor() {
        this.jobId = null;
        this.jobMenu = null;
    }

    handleJobEvent(eventName, args) {
        if (eventName == "EVENT_OPEN_TAKE_JOB_MENU") {
            this.createTakeJobMenu(args[0], args[1], args[2], args[3]);
        }
        else if (eventName == "EVENT_CLOSE_TAKE_JOB_MENU") {
            if (this.jobMenu != null) this.closeJobMenu();
        }
    }

    createTakeJobMenu(jobId, jobName, salary, description) {
        this.jobId = jobId;

        let menu = API.createMenu(jobName, "", 0, 0, 6);
        this.jobMenu = menu;

        let item1 = API.createColoredItem("Accept job", description, "#53a828", "#69d831");
        let item2 = API.createColoredItem("Decline job", description, "#b52f20", "#e03a28");
        let item3 = API.createMenuItem("Salary", description);
        item3.SetRightLabel(salary);

        item2.Activated.connect((menu, sender) => this.closeJobMenu());
        item1.Activated.connect((menu, sender) => this.acceptJob());

        menu.AddItem(item3);
        menu.AddItem(item1);
        menu.AddItem(item2);

        menu.Visible = true;
    }

    closeJobMenu() {
        this.jobMenu.Visible = false;
    }

    acceptJob() {
        API.triggerServerEvent("EVENT_ACCEPT_JOB", this.jobId);
        this.closeJobMenu();
    }
}

let jobManager = new JobManager();
API.onServerEventTrigger.connect((eventName, args) => jobManager.handleJobEvent(eventName, args));
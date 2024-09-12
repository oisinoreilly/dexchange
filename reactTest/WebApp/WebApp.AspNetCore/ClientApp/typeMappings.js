export const SortByStatus = (elementA, elementB) => {
    const severityA = StatusMapping.get(elementA.status.Status).severity;
    const severityB = StatusMapping.get(elementB.status.Status).severity;
    return severityB - severityA;
};
const GREEN_ICON = 'thumbs-up';
const AMBER_ICON = 'hourglass';
const RED_ICON = 'thumbs-down';
//severity of 0 is least severe
const StatusMapping = new Map([
    ["Approved_e", {
            statusColour: "green",
            displayText: "Approved",
            severity: 0,
            iconType: GREEN_ICON
        }],
    ["InformationRequired_e", {
            statusColour: "amber",
            displayText: "InformationRequired",
            severity: 1,
            iconType: AMBER_ICON
        }],
    ["Pending_e", {
            statusColour: "amber",
            displayText: "Pending",
            severity: 1,
            iconType: AMBER_ICON
        }],
    ["Rejected_e", {
            statusColour: "red",
            displayText: "Rejected",
            severity: 2,
            iconType: RED_ICON
        }],
    ["InheritFromChild_e", {
            statusColour: "amber",
            displayText: "InheritFromChild",
            severity: 1,
            iconType: AMBER_ICON
        }],
    ["NotSet_e", {
            statusColour: "amber",
            displayText: "Not set",
            severity: 1,
            iconType: AMBER_ICON
        }]
]);
export default StatusMapping;
//# sourceMappingURL=typeMappings.js.map
<template>
    <div class="ui raised segment" style="padding: 0px">
        <div :id="gridName" style="min-width: 60vw; overflow: hidden; text-align: center;" v-bind:style="styleData"></div>
    </div>
</template>

<script>
export default {
    props: ['gridName', 'gridSize', 'gridData'],

    data() {
        return {
            styleData: {
                'height': (this.gridSize == 'large' ? '75vh' : '30vh')
            }
        }
    },

    mounted() {
        const dataTableName = 'dt_' + this.gridName;
        this.createDataGrid(this.gridName, dataTableName);
    },

    methods: {
        createDataGrid(gridContainer, gridName) {
            const self = this;

            const totalDevelopmentEstimate = self.gridData.totals.developmentEstimate;
            const totalTestingEstimate = self.gridData.totals.testingEstimate;
            const totalTimeSpent = self.gridData.totals.timeSpent;
            const totalTasks = self.gridData.rows.length;

            webix.ready(function () {
                self.grid = webix.ui({
                    container: gridContainer,
                    view: "treetable",
                    id: gridName,
                    columns: [
                        {
                            id: "tagsShortName",
                            header: "Group",
                            width: 60,
                            template: '<span class="tag-#tagsShortName#">#tagsShortName#</span>',
                            tooltip: "#tags#"
                        },
                        {
                            id: "key",
                            header: "Key",
                            sort: 'string',
                            width: 100,
                            template: "<a href='https://testhost.jira.local/browse/#key#' target='_blank'>#key#</a>",
                            footer: { text: "Totals:", css: "totals" }
                        },
                        {
                            id: "issueTypeName",
                            header: "T",
                            sort: 'string',
                            width: 30,
                            template: "<img src='#issueTypeIconUrl#' alt='#issueTypeName#' class='jira-icon'/>",
                            tooltip: "#issueTypeName#"
                        },
                        {
                            id: "summary",
                            header: "Summary",
                            footer: { text: `${totalTasks} task(s)`, css: "totals" },
                            sort: 'string',
                            width: 400,
                            fillspace: true
                        },
                        { id: "assignee", header: "Assignee", sort: 'string', width: 104 },
                        {
                            id: "priorityName",
                            header: "P",
                            sort: 'string',
                            width: 24,
                            template: "<img src='#priorityIconUrl#' alt='#priorityName#' class='jira-icon'/>",
                            tooltip: "#priorityName#"

                        },
                        { id: "statusName", header: "Status", sort: 'string', width: 100 },
                        {
                            id: "includedInSprintDate",
                            header: { text: "Included in Sprint", height: 50, css: "multiline" },
                            sort: 'date', width: 100
                        },
                        {
                            id: "developmentEstimate",
                            header: { text: "Plan estimate", height: 50, css: "multiline" },
                            footer: { text: totalDevelopmentEstimate, css: "totals" },
                            template: function (row) {
                                const css = row.developmentEstimate === '0h' ? 'warn-estimate' : '';
                                return '<span class="' + css + '">' + row.developmentEstimate + '</span>';
                            },
                            sort: 'number',
                            width: 100
                        },
                        { id: "timeSpent", header: [{ text: "Time Spent", css: "multiline" }], footer: { text: totalTimeSpent, css: "totals" }, sort: 'number', width: 80 },
                        { id: "techAnalysisDate", header: { text: "Analyzed Date", height: 50, css: "multiline" }, sort: 'date', width: 100 },
                        { id: "developmentDate", header: { text: "Developed Date", height: 50, css: "multiline" }, sort: 'date', width: 100 },
                        { id: "codeReviewDate", header: { text: "Reviewed Date", height: 50, css: "multiline" }, sort: 'date', width: 100 },
                        { id: "testingDate", header: { text: "Tested Date", height: 50, css: "multiline" }, sort: 'date', width: 100 }
                    ],

                    select: "row",
                    hover: "grid-hover",
                    navigation: true,
                    minHeight: self.gridSize == 'large' ? 50 : 20,
                    autowidth: true,
                    footer: true,
                    tooltip: true,
                    resizeColumn: true,
                    scroll: true,
                    data: [],

                    on: {
                        onSelectChange: function () {
                            var text = "Selected: " + self.grid.getSelectedId(true).join();
                        }
                    }
                });

                self.refreshData();
            });
        },

        refreshData() {
            this.grid.showOverlay("Loading...");
        }
    }
}
</script>

<style>
    .webix_topcell {
        font-weight: bold;
        text-align: left;
    }

    .webix_tree_folder_open {
        margin-right: 12px;
    }

    .jira-icon {
        width: 14px;
        height: 14px;
        margin-left: -8px;
    }

    span.tag-D, span.tag-U, span.tag-S, span.tag-B {
        border-radius: 3px;
        color: #fff;
        display: block;
        font-family: Lato, "Helvetica Neue", Arial, Helvetica, sans-serif;
        font-weight: 700;
        font-size: 10px;
        line-height: 18px;
        margin: 10px 5px 0 0px;
        text-align: center;
    }

    span.tag-U {
        background-color: #ccc;
        border-color: #ccc;
    }

    span.tag-D {
        background-color: #2185d0;
        border-color: #2185d0;
    }

    span.tag-S {
        background-color: #00b5ad;
        border-color: #00b5ad;
    }

    span.tag-B {
        background-color: #fbbd08;
        border-color: #fbbd08;
    }

    span.warn-estimate {
        color: #db2828 !important;
    }

    .multiline {
        line-height: 20px !important;
        padding: 5px 10px;
        text-align: center;
    }
</style>

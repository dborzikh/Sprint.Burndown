<template>
    <div class="ui right floated main menu issues-toolbar">
        <a class="undo popup icon item" data-content="Refresh List" v-on:click="reloadClick()">
            <i class="undo icon" v-show="!recentlyUpdatedInProgress"></i>
            <div class="ui mini active inline loader" v-show="recentlyUpdatedInProgress"></div>
            <div class="floating ui red label counter" v-show="(recentlyUpdatedIssues > 0)">{{recentlyUpdatedIssues}}</div>
        </a>

        <a class="item" data-content="Show issues" v-on:click="changeVisibityClick('sprintTimeline')">
            <i class="calendar alternate outline icon" v-bind:class="{ 'blue': hasSprintTimeline }"></i>
        </a>

        <a class="item" data-content="Show issues" v-on:click="changeVisibityClick('taskStatusSummary')">
            <i class="chart pie icon" v-bind:class="{ 'blue': hasTaskStatusSummary }"></i>
        </a>

        <a class="item" data-content="Show issues" v-on:click="changeVisibityClick('issuesGrid')">
            <i class="table icon" v-bind:class="{ 'blue': hasIssuesGrid }"></i>
        </a>

        <a class="item" data-content="Show issues" v-on:click="changeVisibityClick('burnDownCharts')">
            <i class="chart line icon" v-bind:class="{ 'blue': hasBurnDownCharts }"></i>
        </a>

        <a class="item" data-content="Clear filters" v-on:click="clearFiltersClick()">
            <i class="filter icon" v-bind:class="[ hasFilters ? 'red': 'grey' ]"></i>
        </a>

        <div class="ui pointing dropdown link item" data-content="More Options">
            <i class="ellipsis horizontal icon"></i>
            <div class="ui menu">
                <div class="item" v-on:click="exportToExcel()">Export to Excel</div>
                <div class="item" v-on:click="exportToPdf()">Export to PDF</div>
            </div>
        </div>
    </div>
</template>

<script>
    import router from '../router'
    import { mapGetters } from 'vuex';
    import { CLEAR_FILTERS, CHANGE_VISIBILITY_SPRINT_TIMELINE, CHANGE_VISIBILITY_TASK_STATUS, CHANGE_VISIBILITY_ISSUES, CHANGE_VISIBILITY_BURNDOWN, RELOAD_ISSUES } from '../store/actions/viewState'

    export default {

        data() {
            return {
            }
        },

        computed: {
            sprintId() {
                return this.$store.getters.getSprintSummary.sprintId;
            },

            ...mapGetters([
                'hasFilters',

                'hasSprintTimeline',
                'hasTaskStatusSummary',
                'hasIssuesGrid',
                'hasBurnDownCharts',

                'recentlyUpdatedInProgress',
                'recentlyUpdatedIssues',
            ])
        },

        methods: {
            reloadClick() {
                this.$store.commit(RELOAD_ISSUES);
            },

            // Can be simplfied using mutation names as parameters
            changeVisibityClick(partName) {
                switch (partName) {
                    case 'sprintTimeline':
                        this.$store.commit(CHANGE_VISIBILITY_SPRINT_TIMELINE);
                        break;

                    case 'taskStatusSummary':
                        this.$store.commit(CHANGE_VISIBILITY_TASK_STATUS);
                        break;

                    case 'issuesGrid':
                        this.$store.commit(CHANGE_VISIBILITY_ISSUES);
                        break;

                    case 'burnDownCharts':
                        this.$store.commit(CHANGE_VISIBILITY_BURNDOWN);
                        break;
                }
            },

            clearFiltersClick() {
                this.$store.commit(CLEAR_FILTERS);
            },

            exportToExcel() {
                // TBD
            },

            exportToPdf() {
                // TBD
            }
        }
    }
</script>

<style>
    div.ui.issues-toolbar {
        margin-bottom: -25px;
    }

    .ui.pointing.dropdown .ui.menu {
        margin-left: -70%;
    }

    div.ui.issues-search {
        display: inline-block;
        float: right;
        margin-top: 4px;
        margin-right: 8px;
    }

    .issues-toolbar.ui.main.menu .item {
        padding: 4px 8px 4px 12px;
    }

    .issues-toolbar.ui.main.menu .item i {
        font-size: 15px;
    }

</style>

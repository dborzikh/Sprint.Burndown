<template>
    <div>
        <top-menu></top-menu>
        <div class="ui middle aligned center aligned grid application-container" id="pageContent">
            <router-view></router-view>
        </div>
    </div>
</template>

<script>
    import Vue from 'vue'
    import TopMenu from './top-menu'
    import SprintCard from './sprint-card'
    import TaskStatusCard from './task-status-card'
    import TaskStatusSummary from './task-status-summary'
    import PlanEstimates from './plan-estimates'
    import ScopeEstimates from './scope-estimates'
    
    import BoardCard from './board-card'
    import Message from './message'
    import Modal from './modal'
    
    import PageMenu from './page-menu'

    import IssuesGrid from './issues-grid'
    import IssuesViewSwitch from './issues-view-switch'
    import IssuesToolbar from './issues-toolbar'
    import IssuesSearch from './issues-search'
    import IssuesVersions from './issues-versions'
    import IssuesReporters from './issues-reporters'
    import IssuesPhases from './issues-phases'
    import IssuesCollapseSubtasks from './issues-collapse-subtasks'

    import SprintSummary from './sprint-summary'
    import SprintGantt from './sprint-gantt'

    import BurndownChart from './burndown-chart'

    import Breadcrumb from './breadcrumb'
    import LoadingPanel from './loading-panel'

    import { USER_REQUEST } from '../store/actions/user'
    
    Vue.component('top-menu', TopMenu);
    Vue.component('sprint-card', SprintCard);
    Vue.component('task-status-card', TaskStatusCard);
    Vue.component('task-status-summary', TaskStatusSummary);
    Vue.component('plan-estimates', PlanEstimates);
    Vue.component('scope-estimates', ScopeEstimates);

    Vue.component('board-card', BoardCard);
    Vue.component('message', Message);
    Vue.component('modal', Modal);
    
    Vue.component('page-menu', PageMenu);

    Vue.component('issues-grid', IssuesGrid);
    Vue.component('issues-view-switch', IssuesViewSwitch);
    Vue.component('issues-toolbar', IssuesToolbar);
    Vue.component('issues-search', IssuesSearch);
    Vue.component('issues-versions', IssuesVersions);
    Vue.component('issues-reporters', IssuesReporters);
    Vue.component('issues-phases', IssuesPhases);
    Vue.component('issues-collapse-subtasks', IssuesCollapseSubtasks);

    Vue.component('sprint-summary', SprintSummary);
    Vue.component('sprint-gantt', SprintGantt);

    Vue.component('burndown-chart', BurndownChart);

    Vue.component('breadcrumb', Breadcrumb);
    Vue.component('loading-panel', LoadingPanel);
    

export default {
    data() {
        return {
        }
    },
    created: function () {
        if (this.$store.getters.isAuthenticated) {
            this.$store.dispatch(USER_REQUEST)
        }
    }
}
</script>

<style>
    .ui.application-container {
        padding-left: 10px;
        padding-right: 10px;
    }
</style>
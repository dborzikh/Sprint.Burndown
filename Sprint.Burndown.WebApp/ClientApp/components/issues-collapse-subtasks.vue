<template>
    <div class="issues-collapse-subtasks">
        <div class="ui right floated main menu">
            <a class="item" data-content="Show issues" v-on:click="collapse('starred')">
                <i class="star icon" v-bind:class="{ 'blue': hasStarredDisplayed, 'outline': '!hasStarredDisplayed' }"></i>
            </a>

            <a class="item" data-content="Show issues" v-on:click="collapse('tasks')">
                <i class="th list icon" v-bind:class="{ 'blue': hasTasksCollapsed }"></i>
            </a>

            <a class="item" data-content="Show issues" v-on:click="collapse('bugs')">
                <i class="bug icon" v-bind:class="{ 'blue': hasBugsCollapsed }"></i>
            </a>
        </div>
    </div>
</template>

<script>

    import { mapGetters } from 'vuex';
    import { SET_COLLAPSING_SUBTASKS, SET_COLLAPSING_BUGS, FILTER_STARRED_ISSUES } from '../store/actions/viewState'

    export default {
        computed: {
            hasStarredDisplayed() {
                return this.$store.getters.displayStarred;
            },

            hasTasksCollapsed() {
                return this.$store.getters.collapsedSubtasks;
            },

            hasBugsCollapsed() {
                return this.$store.getters.collapsedBugs;
            },
        },

        watch: {
        },

        methods: {
            collapse(what) {
                if (what == 'tasks') {
                    this.$store.commit(SET_COLLAPSING_SUBTASKS);
                } else if (what == 'bugs') {
                    this.$store.commit(SET_COLLAPSING_BUGS);
                } else {
                    this.$store.commit(FILTER_STARRED_ISSUES);
                }
            }
        }
    }
</script>

<style>
    .issues-collapse-subtasks {
        display: inline-block;
        margin-top: 16px;
        padding-top: 0 !important;
        padding-bottom: 0 !important;
        text-align: right;
        width: auto;
    }

    .issues-collapse-subtasks .ui.checkbox label {
        color: #a0a0a0;
    }


    .issues-collapse-subtasks .ui.main.menu {
        min-height: 38px;
    }

    .issues-collapse-subtasks .ui.main.menu .item {
        padding: 4px 8px 4px 12px;
    }

    .issues-collapse-subtasks .ui.main.menu .item i {
        font-size: 15px;
    }
</style>

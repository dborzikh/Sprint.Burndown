<template>
    <div class="issues-phases">
        <div class="ui mini steps">
            <div v-for="phase in currentFilterByPhase" class="step" v-bind:class="{ active: phase.checked }" v-on:click="toggleState(phase.group)">
                <div class="content">
                    <div class="title ">{{phase.title}}</div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex';
    import { REMOVE_TASK_STATUS_FILTERS, ADD_TASK_STATUS_FILTERS } from '../store/actions/viewState'

    export default {
        computed: {
            ...mapGetters([
                'currentFilterByPhase',
                'lastUpdated'
            ])
        },

        watch: {
            lastUpdated(newValue) {
            },
        },

    methods: {
        toggleState(group) {
            const item = _.find(this.currentFilterByPhase, ['group', group]);

            if (item != undefined) {
                this.$store.commit(
                    item.checked ? REMOVE_TASK_STATUS_FILTERS : ADD_TASK_STATUS_FILTERS,
                    group);
            }
        }
    },
}
</script>

<style>
    .issues-phases {
        display: inline-block;
        margin-top: 16px;
        padding-top: 0 !important;
        padding-bottom: 0 !important;
        padding-right: 32px !important;
        text-align: right;
        width: auto;
    }

    .issues-phases .ui.steps .step {
        padding: 10px 2em;
        min-width: 10em;
        cursor: pointer;
    }

    .ui.steps .step:hover .title {
        color: #4183c4;
    }
</style>

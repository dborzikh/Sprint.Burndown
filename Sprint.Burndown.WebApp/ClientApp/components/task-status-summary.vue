<template>
    <div class="ui grid task-summary">
        <task-status-card v-for="summary in issuesSummaries" v-bind:group="summary.group" />
    </div>
</template>

<script>
    import { mapGetters } from 'vuex';

    export default {
        computed: {
            ...mapGetters([
                'issuesSummaries'
            ])
        }
    }
</script>

<style>
    .ui.task-summary.grid {
        min-height: 280px;
        padding: 24px 16px 16px 8px;
    }
</style>
